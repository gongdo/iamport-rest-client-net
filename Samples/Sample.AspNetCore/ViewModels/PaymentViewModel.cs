using System;
using Iamport.RestApi.Models;

namespace Sample.AspNetCore.ViewModels
{
    public class PaymentViewModel
    {
        internal static PaymentViewModel Create(Models.Payment payment, Models.Checkout checkout)
        {
            return new PaymentViewModel
            {
                Name = checkout.Name,
                Amount = checkout.Amount,
                PaymentMethod = checkout.PaymentMethod,
                State = payment.State,
                VirtualBankAccount = payment.VirtualBankAccount,
                VirtualBankAccountHolder = payment.VirtualBankAccountHolder,
                VirtualBankExpirationTime = checkout.VirtualBankExpirationTime,
                VirtualBankName = payment.VirtualBankName,
            };
        }

        public string Name { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public DateTimeOffset? VirtualBankExpirationTime { get; private set; }
        public string VirtualBankName { get; private set; }
        public string VirtualBankAccount { get; private set; }
        public string VirtualBankAccountHolder { get; private set; }
        public Models.PaymentState State { get; private set; }
    }
}
