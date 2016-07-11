using Iamport.RestApi;
using Iamport.RestApi.Apis;
using Iamport.RestApi.Models;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;
using System.Threading.Tasks;

namespace Sample.AspNetCore.Controllers
{
    [Route("[Controller]")]
    public class CheckoutController : Controller
    {
        // TODO: 설정으로 옮길 것.
        private const string appScheme = "iamporttest";

        private readonly IPaymentsApi paymentsApi;
        private readonly PaymentRepository paymentRepository;
        private readonly IamportHttpClientOptions options;
        public CheckoutController(
            IPaymentsApi paymentsApi,
            PaymentRepository paymentRepository,
            IamportHttpClientOptions options)
        {
            if (paymentsApi == null)
            {
                throw new ArgumentNullException(nameof(paymentsApi));
            }
            if (paymentRepository == null)
            {
                throw new ArgumentNullException(nameof(paymentRepository));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            this.paymentsApi = paymentsApi;
            this.paymentRepository = paymentRepository;
            this.options = options;
        }

        /// <summary>
        /// 결제 요청(Checkout; 혹은 구매) 페이지
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(new RequestCheckoutViewModel());
        }

        /// <summary>
        /// 결제 요청처리
        /// </summary>
        /// <param name="model">결제 요청 정보</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RequestCheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // 입력값을 저장할 결제 엔터티로 변환
            var payment = model.ToPayment();
            // 해당 결제 내용을 아임포트에 등록(예약)
            await PreparationAsync(model.Amount, payment.TransactionId);
            // 결제 엔터티를 저장소에 저장
            paymentRepository.Add(payment);
            // 결제 요청 정보를 반환
            var request = GetRequestViewModel(payment);
            return Ok(request);
        }

        private RequestPaymentViewModel GetRequestViewModel(Models.Payment payment)
        {
            // notification url은 결제 성공/실패시 아임포트로부터 호출되는 URL입니다.
            var notificationUrl = Url.Action(
                "Report",
                "Payment",
                new { transactionId = payment.TransactionId });
            // return url은 웹뷰에서 결제 플러그인이 성공/실패한 후 이동할 URL입니다.
            var returnUrl = Url.Action(
                "Index",
                "Payment",
                new { transactionId = payment.TransactionId });
            return RequestPaymentViewModel.Create(
                payment,
                options.IamportId,
                notificationUrl,
                returnUrl,
                appScheme);
        }

        private async Task PreparationAsync(decimal amount, string transactionId)
        {
            var preparation = await paymentsApi.PrepareAsync(new PaymentPreparation
            {
                Amount = amount,
                TransactionId = transactionId,
            });
            if (preparation == null)
            {
                throw new Exception("Failed to prepare payment.");
            }
        }
    }
}
