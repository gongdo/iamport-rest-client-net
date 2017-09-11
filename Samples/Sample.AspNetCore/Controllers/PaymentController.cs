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
    public class PaymentController : Controller
    {
        // 모바일 결제시 결제 후 돌아갈 앱의 스키마
        private const string AppScheme = "iamporttest";
        public const string Name = "Payment";

        private readonly IPaymentsApi paymentsApi;
        private readonly CheckoutRepository checkoutRepository;
        private readonly PaymentRepository paymentRepository;
        private readonly string iamportId;
        public PaymentController(
            IPaymentsApi paymentsApi,
            CheckoutRepository checkoutRepository,
            PaymentRepository paymentRepository,
            IamportHttpClientOptions clientOptions)
        {
            this.paymentsApi = paymentsApi ?? throw new ArgumentNullException(nameof(paymentsApi));
            this.checkoutRepository = checkoutRepository ?? throw new ArgumentNullException(nameof(checkoutRepository));
            this.paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }
            iamportId = clientOptions.IamportId;
        }

        /// <summary>
        /// 주어진 정보로 새 결제를 요청하고 아임포트 결제 요청 정보를 반환합니다.
        /// </summary>
        /// <param name="model">새 결제 요청 정보</param>
        /// <returns>아임포트 결제 요청 정보</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAjax([FromBody] RegisterPaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 구매 정보 확인
            var checkout = checkoutRepository.GetById(model.CheckoutId);
            if (checkout == null)
            {
                ModelState.AddModelError(nameof(model.CheckoutId), "Checkout Id가 존재하지 않습니다.");
                return BadRequest(ModelState);
            }

            // 새 결제 저장
            var payment = Models.Payment.Create(model.CheckoutId);
            paymentRepository.Add(payment);

            // 새 결제 내용을 아임포트에 등록
            await paymentsApi.PrepareAsync(new PaymentPreparation
            {
                Amount = checkout.Amount,
                TransactionId = payment.TransactionId,
            });

            // 아임포트로부터 결제 내용을 알림 받을 주소:
            var notificationUrl = Url.Link(
                null,
                new
                {
                    controller = "Payment",
                    action = nameof(OnNotification),
                    transactionId = payment.TransactionId,
                });
            // 결제 완료시 돌아올 URL.
            // PC와 모바일 웹 브라우저 동작 차이를 줄이기 위해
            // 항상 내부 사이트로 돌아오는 것이 좋습니다.
            var returnUrl = Url.Action(nameof(Refresh), "Payment", new { transactionId = payment.TransactionId });

            return Json(new PaymentRequest
            {
                Amount = checkout.Amount,
                AppScheme = AppScheme,
                Currency = "KRW",
                CustomerEmail = checkout.CustomerEmail,
                CustomerName = checkout.CustomerName,
                CustomerPhoneNumber = checkout.CustomerPhoneNumber,
                IsDigital = checkout.IsDigital,
                Method = checkout.PaymentMethod,
                NotificationUrl = notificationUrl,
                PaymentGateway = checkout.PaymentGateway,
                ReturnUrl = returnUrl,
                Title = checkout.Name,
                TransactionId = payment.TransactionId,
                UseEscrow = checkout.UseEscrow,
                VirtualBankExpiration = checkout.VirtualBankExpirationTime,
                // 다음은 설정할 필요 없음.
                //MerchantId = null,  // 상점 구분 ID
                //CustomData = null,
                //CustomerAddress = null,
                //CustomerPostCode = null,
            });
        }

        /// <summary>
        /// 주어진 거래 ID에 해당하는 결제 상태 페이지를 반환합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpGet("{transactionId}")]
        public IActionResult Index(string transactionId)
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

        /// <summary>
        /// 지정한 거래 ID의 결제 상태를 아임포트 서비스에 조회하여 업데이트하고
        /// 결과를 보여줄 페이지로 이동합니다.
        /// GET 메서드에서 상태 변경을 처리하는 것은 좋지 않은 방법이지만
        /// 결제 흐름상 반드시 거쳐야 오류가 적어지므로 이렇게 구현합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpGet("{transactionId}/refresh")]
        public async Task<IActionResult> Refresh(string transactionId)
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

            // 해당 결제 상태를 업데이트합니다.
            await RefreshPaymentAsync(payment);

            // 복귀합니다.
            var returnUrl = checkout.ReturnUrl;
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Link(
                    null,
                    new
                    {
                        controller = "Payment",
                        action = nameof(Index),
                        transactionId = payment.TransactionId,
                    });
            }
            return Redirect(returnUrl);
        }

        /// <summary>
        /// 지정한 거래 ID의 결제 상태를 아임포트 서비스에 조회하여 업데이트합니다.
        /// 이 엔드포인트는 또한 아임포트의 결제 알림 URL(notification url)입니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpPost("{transactionId}/notification")]
        public async Task<IActionResult> OnNotification(string transactionId)
        {
            var payment = paymentRepository.GetByTransactionId(transactionId);
            if (payment == null)
            {
                return NotFound();
            }
            await RefreshPaymentAsync(payment);

            return NoContent();
        }
        
        // TODO:
        // 이 로직은 비지니스 로직을 담고 있습니다.
        // 비즈니스 레이어로 옮기는 것이 좋습니다.
        private async Task RefreshPaymentAsync(Models.Payment payment)
        {
            var iamportPayment = await paymentsApi.GetByTransactionIdAsync(payment.TransactionId);
            if (iamportPayment == null)
            {
                throw new InvalidOperationException("Failed to get payment information from Iamport.");
            }

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
