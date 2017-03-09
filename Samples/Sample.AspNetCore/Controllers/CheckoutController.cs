using Iamport.RestApi;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;

namespace Sample.AspNetCore.Controllers
{
    /// <summary>
    /// 구매 정보 관련 컨트롤러
    /// </summary>
    [Route("[Controller]")]
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
        public IActionResult Index()
        {
            return View(new RegisterCheckoutModel());
        }

        /// <summary>
        /// 구매 정보를 등록하고 결제 페이지로 이동합니다.
        /// </summary>
        /// <param name="model">구매 정보</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RegisterCheckoutModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
        [HttpGet("{id}/gateway")]
        public IActionResult Gateway(Guid id)
        {
            var checkout = checkoutRepository.GetById(id);
            if (checkout == null)
            {
                return NotFound();
            }

            var viewModel = RequestPaymentViewModel.Create(
                checkout: checkout,
                iamportId: clientOptions.IamportId,
                paymentUrl: Url.Action(nameof(PaymentController.CreateAjax), "Payment"));
            return View(viewModel);
        }

        [HttpGet("{id}/PageNotFound")]
        public IActionResult PageNotFound(Guid id)
        {
            ViewData["Id"] = id;
            return View();
        }
    }
}
