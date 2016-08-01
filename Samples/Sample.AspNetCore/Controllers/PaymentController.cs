using Iamport.RestApi.Apis;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using Sample.AspNetCore.Models;
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
        private readonly PaymentRepository paymentRepository;
        private readonly string iamportId;
        public PaymentController(
            IPaymentsApi paymentsApi,
            PaymentRepository paymentRepository,
            IamportHttpClientOptions clientOptions)
        {
            if (paymentsApi == null)
            {
                throw new ArgumentNullException(nameof(paymentsApi));
            }
            if (paymentRepository == null)
            {
                throw new ArgumentNullException(nameof(paymentRepository));
            }
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }
            this.paymentsApi = paymentsApi;
            this.paymentRepository = paymentRepository;
            iamportId = clientOptions.IamportId;
        }

        /// <summary>
        /// 주어진 정보로 새 결제를 요청하고 결제 페이지로 이동합니다.
        /// </summary>
        /// <param name="model">결제 요청 정보</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestPaymentModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 새 결제 저장
            var payment = model.ToPayment();
            paymentRepository.Add(payment);

            // 새 결제 내용을 아임포트에 등록
            await paymentsApi.PrepareAsync(new PaymentPreparation
            {
                Amount = payment.Amount,
                TransactionId = payment.TransactionId,
            });

            // 결제 페이지로 이동
            return RedirectToAction(
                nameof(Gateway), 
                new { transactionId = payment.TransactionId });
        }

        /// <summary>
        /// 실제 결제 처리를 수행하는 페이지를 반환합니다.
        /// 만약 해당 거래 ID가 이미 처리되었다면 결과 페이지로 자동으로 이동합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpGet("{transactionId}/gateway")]
        public IActionResult Gateway(string transactionId)
        {
            // 결제 내용 조회
            var payment = paymentRepository.GetByTransactionId(transactionId);
            if (payment == null)
            {
                return PageNotFound(transactionId);
            }

            // 이미 결제가 진행중이면 결과 페이지로 이동
            if (payment.State != PaymentState.Prepared
                && payment.State != PaymentState.InProgress)
            {
                return RedirectToAction(
                    nameof(Index), 
                    new { transactionId = transactionId });
            }

            // 아임포트로부터 결제 내용을 알림 받을 주소:
            var notificationUrl = Url.Link(
                null,
                new
                {
                    controller = "Payment",
                    action = nameof(RefreshResult),
                    transactionId = transactionId,
                });
            // 결제 완료시 돌아올 URL.
            // PC와 모바일 웹 브라우저 동작 차이를 줄이기 위해
            // 항상 내부 사이트로 돌아오는 것이 좋습니다.
            var returnUrl = Url.Action(nameof(Refresh), new { transactionId = transactionId });

            // 결제 내용을 아임포트 결제 요청으로 변환
            var viewModel = RequestPaymentViewModel.Create(
                payment: payment,
                iamportId: iamportId,
                notificationUrl: notificationUrl,
                returnUrl: returnUrl,
                appScheme: AppScheme);
            return View(viewModel);
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
            var viewModel = PaymentViewModel.Create(payment);
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
            await RefreshPaymentAsync(payment);

            return Redirect(GetReturnUrl(payment));
        }

        /// <summary>
        /// 지정한 거래 ID의 결제 상태를 아임포트 서비스에 조회하여 업데이트합니다.
        /// 이 엔드포인트는 또한 아임포트의 결제 알림 URL(notification url)입니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpPost("{transactionId}/refresh")]
        public async Task<IActionResult> RefreshResult(string transactionId)
        {
            var payment = paymentRepository.GetByTransactionId(transactionId);
            if (payment == null)
            {
                return NotFound();
            }
            await RefreshPaymentAsync(payment);

            return NoContent();
        }

        /// <summary>
        /// 주어진 결제 정보에 대한 ReturnUrl을 반환합니다.
        /// </summary>
        /// <param name="payment">결제 정보</param>
        /// <returns>ReturnUrl</returns>
        private string GetReturnUrl(Models.Payment payment)
        {
            var returnUrl = payment.ReturnUrl;
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
            return returnUrl;
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
