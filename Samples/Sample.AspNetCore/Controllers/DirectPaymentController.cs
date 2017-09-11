using Iamport.RestApi.Apis;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using Iamport.RestApi;

namespace Sample.AspNetCore.Controllers
{
    [Route("[Controller]")]
    public class DirectPaymentController : Controller
    {
        private readonly IPaymentsApi paymentsApi;
        private readonly ISubscribeApi subscribeApi;
        private readonly CheckoutRepository checkoutRepository;
        private readonly PaymentRepository paymentRepository;
        private readonly string iamportId;
        public DirectPaymentController(
            IPaymentsApi paymentsApi,
            ISubscribeApi subscribeApi,
            CheckoutRepository checkoutRepository,
            PaymentRepository paymentRepository,
            IamportHttpClientOptions clientOptions)
        {
            this.paymentsApi = paymentsApi ?? throw new ArgumentNullException(nameof(paymentsApi));
            this.subscribeApi = subscribeApi ?? throw new ArgumentNullException(nameof(subscribeApi));
            this.checkoutRepository = checkoutRepository ?? throw new ArgumentNullException(nameof(checkoutRepository));
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }
            iamportId = clientOptions.IamportId;
        }

        /// <summary>
        /// 비인증 결제 페이지
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View(new RequestDirectPaymentModel());
        }

        /// <summary>
        /// 비인증 결제 요청
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RequestDirectPaymentModel input)
        {
            if (ModelState.IsValid == false)
            {
                return View(nameof(Index), input);
            }

            // 비인증 구매는 구매 요청을 즉시 처리 가능.
            var checkout = Models.Checkout.Create(
                name: input.Name,
                amount: input.Amount,
                paymentGateway: input.PaymentGateway,
                isDigital: false,
                useEscrow: false,
                paymentMethod: PaymentMethod.CreditCard,
                customerName: input.CustomerName,
                customerEmail: input.CustomerEmail,
                customerPhoneNumber: input.CustomerPhoneNumber,
                returnUrl: null,
                virtualBankExpirationTime: DateTimeOffset.UtcNow);
            checkoutRepository.Add(checkout);

            var payment = Models.Payment.Create(checkout.Id);

            var request = new DirectPaymentRequest
            {
                CustomerId = null,  // 등록된 사용자 빌링키 없이 한번 진행
                TransactionId = payment.TransactionId,
                Vat = input.Vat,
                Amount = input.Amount,
                Title = input.Name,
                CardNumber = input.CardNumber,
                AuthenticationNumber = input.AuthenticationNumber,
                Expiry = input.Expiry,
                PartialPassword = input.PartialPassword,
                InstallmentMonths = 0,
                BuyerName = input.CustomerName,
                Email = input.CustomerEmail,
                PhoneNumber = input.CustomerPhoneNumber,
                Address = input.CustomerAddress,
                PostCode = input.CustomerPostCode,
            };

            Payment iamportPayment = null;
            try
            {
                iamportPayment = await subscribeApi.DoDirectPaymentAsync(request);
            }
            catch (IamportResponseException ex)
            {
                ModelState.AddModelError("iamport", ex.Message);
                return View(nameof(Index), input);
            }
            
            paymentRepository.Add(payment);
            RefreshPayment(iamportPayment, payment);

            return RedirectToAction(nameof(Result), new { transactionId = payment.TransactionId });
        }

        /// <summary>
        /// 구매 결과 페이지
        /// </summary>
        /// <param name="transactionId">거래 고유 ID</param>
        /// <returns></returns>
        [HttpGet("{transactionId}")]
        public IActionResult Result(string transactionId)
        {
            var payment = paymentRepository.GetByTransactionId(transactionId);
            if (payment == null)
            {
                return PageNotFound(transactionId);
            }
            var checkout = checkoutRepository.GetById(payment.CheckoutId);
            if (checkout == null)
            {
                return PageNotFound(transactionId);
            }

            var viewModel = PaymentViewModel.Create(payment, checkout);
            return View(viewModel);
        }

        /// <summary>
        /// 지정한 거래 ID의 결제가 발견되지 않았습니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpGet("{transactionId}/pagenotfound")]
        public IActionResult PageNotFound(string transactionId)
        {
            ViewData["TransactionId"] = transactionId;
            return View(nameof(PageNotFound));
        }

        // TODO:
        // 이 로직은 비지니스 로직을 담고 있습니다.
        // 비즈니스 레이어로 옮기는 것이 좋습니다.

        private void RefreshPayment(Payment iamportPayment, Models.Payment payment)
        {
            // 기존 상태(payment.State)와 최신 상태(iamportPayment.Status)를 비교합니다.
            switch (iamportPayment.Status)
            {
                case PaymentStatus.Ready:
                    if (iamportPayment.Method == PaymentMethod.VirtualBank
                        && !string.IsNullOrEmpty(iamportPayment.VirtualBankAccount))
                    {
                        payment.SetAwatingForVirtualBank(
                            iamportPayment.IamportId,
                            iamportPayment.PaymentGatewayTransactionId,
                            iamportPayment.VirtualBankName,
                            iamportPayment.VirtualBankAccount,
                            iamportPayment.VirtualBankAccountHolder);
                    }
                    else
                    {
                        payment.SetInProgress(iamportPayment.IamportId);
                    }
                    break;
                case PaymentStatus.Paid:
                    switch (iamportPayment.Method)
                    {
                        case PaymentMethod.CreditCard:
                            payment.SetPaidForCreditCard(
                                iamportPayment.IamportId,
                                iamportPayment.PaidAtUtc,
                                iamportPayment.PaymentGatewayTransactionId,
                                iamportPayment.ApplyNumber,
                                iamportPayment.CreditCardCompanyName,
                                iamportPayment.InstallmentPlanPeriod);
                            break;
                        case PaymentMethod.VirtualBank:
                            payment.SetPaiedForVirtualBank(
                                iamportPayment.IamportId,
                                iamportPayment.PaidAtUtc,
                                iamportPayment.PaymentGatewayTransactionId,
                                iamportPayment.VirtualBankName,
                                iamportPayment.VirtualBankAccount,
                                iamportPayment.VirtualBankAccountHolder);
                            break;
                        case PaymentMethod.Transfer:
                        case PaymentMethod.Mobile:
                        case PaymentMethod.CultureLand:
                        case PaymentMethod.SmartCulture:
                        case PaymentMethod.HappyMoney:
                        default:
                            payment.SetPaidForOthers(
                                iamportPayment.IamportId,
                                iamportPayment.PaidAtUtc,
                                iamportPayment.PaymentGatewayTransactionId);
                            break;
                    }
                    break;
                case PaymentStatus.Cancelled:
                    payment.SetCancelled(
                        iamportPayment.IamportId,
                        iamportPayment.CancelledAtUtc,
                        iamportPayment.CancelledReason,
                        iamportPayment.CancelledAmount);
                    break;
                case PaymentStatus.Failed:
                    payment.SetFailed(
                        iamportPayment.IamportId,
                        iamportPayment.FailedAtUtc,
                        iamportPayment.FailedReason);
                    break;
                default:
                    break;
            }
            paymentRepository.Update(payment);
        }
    }
}
