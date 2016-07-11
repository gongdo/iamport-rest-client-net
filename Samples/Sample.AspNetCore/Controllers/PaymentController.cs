using Iamport.RestApi.Apis;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;

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
        public PaymentController(
            IPaymentsApi paymentsApi,
            PaymentRepository paymentRepository)
        {
            if (paymentsApi == null)
            {
                throw new ArgumentNullException(nameof(paymentsApi));
            }
            if (paymentRepository == null)
            {
                throw new ArgumentNullException(nameof(paymentRepository));
            }
            this.paymentsApi = paymentsApi;
            this.paymentRepository = paymentRepository;
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
        /// 지정한 거래 ID의 결제 상태를 아임포트 서비스에 조회하여 업데이트합니다.
        /// 이 엔드포인트는 또한 아임포트의 결제 알림 URL(notification url)입니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <returns></returns>
        [HttpPost("{transactionId}/refresh")]
        public async Task<IActionResult> Refresh(string transactionId)
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
