using Iamport.RestApi;
using Iamport.RestApi.Apis;
using Microsoft.AspNetCore.Mvc;
using Sample.AspNetCore.Repositories;
using Sample.AspNetCore.ViewModels;
using System;

namespace Sample.AspNetCore.Controllers
{
    [Route("[Controller]")]
    public class CheckoutController : Controller
    {
        private readonly IPaymentsApi paymentsApi;
        public CheckoutController(
            IPaymentsApi paymentsApi)
        {
            if (paymentsApi == null)
            {
                throw new ArgumentNullException(nameof(paymentsApi));
            }
            this.paymentsApi = paymentsApi;
        }

        /// <summary>
        /// 결제 요청(Checkout; 혹은 구매) 페이지
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View(new RequestPaymentModel());
        }
    }
}
