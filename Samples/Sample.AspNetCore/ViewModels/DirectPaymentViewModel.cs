using System;
using Iamport.RestApi.Models;

namespace Sample.AspNetCore.ViewModels
{
    public class DirectPaymentViewModel
    {
        internal static DirectPaymentViewModel Create(Models.Payment payment, Models.Checkout checkout)
        {
            return new DirectPaymentViewModel
            {
                Name = checkout.Name,
                Amount = checkout.Amount,
                State = payment.State,
            };
        }

        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public Models.PaymentState State { get; private set; }
    }
}
