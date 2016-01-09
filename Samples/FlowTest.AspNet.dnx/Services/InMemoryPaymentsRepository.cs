using System.Collections.Generic;
using System;

namespace FlowTest.AspNet.dnx.Services
{
    /// <summary>
    /// 테스트용 Models.Payment 저장소 구현체.
    /// 메모리 상에만 저장합니다.
    /// </summary>
    public class InMemoryPaymentsRepository
        : IPaymentsRepository
    {
        private Dictionary<string, Models.Payment> payments
            = new Dictionary<string, Models.Payment>();

        public Models.Payment Add(Models.Payment payment)
        {
            payment.TransactionId = Guid.NewGuid().ToString();
            payments.Add(payment.TransactionId, payment);
            return payment;
        }

        public Models.Payment GetByTransactionId(string transactionId)
        {
            Models.Payment payment;
            if (payments.TryGetValue(transactionId, out payment))
            {
                return payment;
            }
            return null;
        }

        public Models.Payment Update(Models.Payment payment)
        {
            payments[payment.TransactionId] = payment;
            return payment;
        }
    }
}
