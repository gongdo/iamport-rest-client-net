using Iamport.RestApi;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;
using System.Net;
using System.Web.Mvc;

namespace Sample.AspNet.Controllers
{
    /// <summary>
    /// 구매 정보 관련 컨트롤러
    /// </summary>
    public class CheckoutController : Controller
    {
        private readonly CheckoutRepository checkoutRepository;
        private readonly IamportHttpClientOptions clientOptions;
        public CheckoutController(
            CheckoutRepository checkoutRepository,
            IamportHttpClientOptions clientOptions)
        {
            if (checkoutRepository == null)
            {
                throw new ArgumentNullException(nameof(checkoutRepository));
            }
            if (clientOptions == null)
            {
                throw new ArgumentNullException(nameof(clientOptions));
            }
            this.checkoutRepository = checkoutRepository;
            this.clientOptions = clientOptions;
        }

        /// <summary>
        /// 결제 요청(Checkout; 혹은 구매) 페이지
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View(new RegisterCheckoutModel());
        }

        /// <summary>
        /// 구매 정보를 등록하고 결제 페이지로 이동합니다.
        /// </summary>
        /// <param name="model">구매 정보</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(RegisterCheckoutModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 새 구매 정보 저장
            var checkout = model.ToCheckout();
            checkoutRepository.Add(checkout);

            // 결제 페이지로 이동
            return RedirectToAction(nameof(Gateway), new { id = checkout.Id });
        }

        /// <summary>
        /// 해당 구매 정보를 결제하는 결제 게이트웨이 페이지.
        /// </summary>
        /// <param name="id">구매 정보 Id</param>
        /// <returns></returns>
        //[HttpGet("{id}/gateway")]
        public ActionResult Gateway(Guid id)
        {
            var checkout = checkoutRepository.GetById(id);
            if (checkout == null)
            {
                return PageNotFound(id);
            }

            var viewModel = RequestPaymentViewModel.Create(
                checkout: checkout,
                iamportId: clientOptions.IamportId,
                paymentUrl: Url.Action(nameof(PaymentController.CreateAjax), "Payment"));
            return View(viewModel);
        }

        //[HttpGet("{id}/PageNotFound")]
        public ActionResult PageNotFound(Guid id)
        {
            ViewData["Id"] = id;
            return View();
        }
    }
}