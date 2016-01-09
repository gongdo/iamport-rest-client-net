using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Iamport.RestApi.Models;
using FlowTest.AspNet.dnx.ViewModels;
using Iamport.RestApi.Apis;
using FlowTest.AspNet.dnx.Services;
using System.Globalization;

namespace FlowTest.Mvc6.dnx.Controllers
{
    [Route("")]
    [Route("[Controller]")]
    public class PaymentController : Controller
    {
        private readonly Iamport.RestApi.Iamport iamport;
        private readonly IPaymentsRepository paymentsRepository;
        public PaymentController(
            Iamport.RestApi.Iamport iamport,
            IPaymentsRepository paymentsRepository)
        {
            if (iamport == null)
            {
                throw new ArgumentNullException(nameof(iamport));
            }
            if (paymentsRepository == null)
            {
                throw new ArgumentNullException(nameof(paymentsRepository));
            }
            this.iamport = iamport;
            this.paymentsRepository = paymentsRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            /*
            주문 또는 결제할 항목을 선택하는 페이지입니다.
            */
            return View(new PaymentViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(PaymentViewModel input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            /*
            일반적인 주문 로직을 작성합니다.
            1. 사용자의 주문이 유효한지 검증(상품 및 금액 등)
            2. 사용자의 주문을 DB에 저장하고 해당 주문에 대한 고유 거래 ID를 발급
            */
            // *위의 로직이 처리되었다고 가정한 코드
            var payment = new AspNet.dnx.Models.Payment
            {
                Amount = input.Amount,
                CustomerAddress = input.CustomerAddress,
                CustomerEmail = input.CustomerEmail,
                CustomerName = input.CustomerName,
                CustomerPhoneNumber = input.CustomerPhoneNumber,
                CustomerPostCode = input.CustomerPostCode,
                Method = input.Method,
                Title = input.Title,
                PaymentGateway = input.PaymentGateway,
                State = PaymentState.Ready,
                AppScheme = input.AppScheme,
                UseEscrow = input.UseEscrow,
            };
            if (input.Method == PaymentMethod.VirtualBank)
            {
                payment.VirtualBankExpiration
                    = DateTimeOffset.ParseExact(
                        input.VirtualBankExpiration,
                        "yyyyMMdd",
                        CultureInfo.InvariantCulture.DateTimeFormat);
            }
            paymentsRepository.Add(payment);

            /*
            준비된 결제내용을 아임포트 서버에 등록하여 예약합니다.
            */
            var preparation = new PaymentPreparation
            {
                Amount = payment.Amount,
                TransactionId = payment.TransactionId,
            };
            var api = iamport.Api<IPaymentsApi>();
            await api.PrepareAsync(preparation);

            /*
            등록에 성공하면 해당 트랜잭션ID에 대한 체크아웃 페이지로 이동합니다.
            */
            return RedirectToAction("Checkout");
        }

        [HttpGet("checkout/{transactionId}")]
        public IActionResult Checkout(string transactionId)
        {
            /*
            1. 주어진 거래 ID에 해당하는 결제 내용을 조회하고
            2. 체크아웃(PG사에 결제 요청) 페이지를 표시합니다.
            */
            var payment = paymentsRepository
                .GetByTransactionId(transactionId);
            if (payment == null)
            {
                return HttpNotFound();
            }

            // 모바일일 경우를 대비하여 되돌아올 URL을 지정해둡니다.
            var returnUrl = Url.Action("Result");
            var paymentRequest = new PaymentRequest
            {
                Amount = payment.Amount,
                AppScheme = payment.AppScheme,
                CustomerAddress = payment.CustomerAddress,
                CustomerEmail = payment.CustomerEmail,
                CustomerName = payment.CustomerName,
                CustomerPhoneNumber = payment.CustomerPhoneNumber,
                CustomerPostCode = payment.CustomerPostCode,
                Method = payment.Method,
                PaymentGateway = payment.PaymentGateway,
                Title = payment.Title,
                TransactionId= payment.TransactionId,
                UseEscrow = payment.UseEscrow,
                VirtualBankExpiration = payment.VirtualBankExpiration,
                ReturnUrl = returnUrl,
            };
            return View(paymentRequest);
        }

        [HttpGet("result")]
        public async Task<IActionResult> Result([FromQuery]PaymentIdentity identity)
        {
            /*
            1. 주어진 거래 ID에 해당하는 결제 내용을 조회하고
            2. 해당 결제 내용을 아임포트의 최신 정보로 갱신한 후
            3. 결과를 표시합니다.
            */
            var payment = paymentsRepository
                .GetByTransactionId(identity.TransactionId);
            if (payment == null)
            {
                return HttpNotFound();
            }

            var api = iamport.Api<IPaymentsApi>();
            var result = await api.GetByIamportIdAsync(identity.IamportId);
            // 또는 transactionId로도 조회 가능합니다.
            //var result = await api.GetByTransactionIdAsync(identity.TransactionId);

            if (result == null)
            {
                return HttpNotFound();
            }

            // Iamport 결제 결과를 애플리케이션 결제 데이터에 반영합니다.
            payment.State = result.State;
            payment.IamportId = result.IamportId;
            paymentsRepository.Update(payment);

            /*
            이 외에도 추가로 저장하거나 보여주고 싶은 정보가 있다면
            데이터 모델에 반영하도록 합니다.
            예제에서는 보다 많은 정보를 포함하는
            아임포트의 데이터로 페이지를 표시합니다.
            */
            return View(result);
        }
    }
}
