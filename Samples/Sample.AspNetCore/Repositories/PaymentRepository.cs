using Sample.AspNetCore.Models;
using System.Collections.Concurrent;

namespace Sample.AspNetCore.Repositories
{
    /// <summary>
    /// 결제를 저장하는 리포지터리 샘플
    /// </summary>
    public class PaymentRepository
    {
        private ConcurrentDictionary<string, Payment> payments
            = new ConcurrentDictionary<string, Payment>();

        /// <summary>
        /// 주어진 결제를 새로 저장합니다.
        /// </summary>
        /// <param name="payment">결제</param>
        /// <exception cref="DuplicatedKeyException">해당 결제의 TransactionId가 이미 저장되어 있습니다.</exception>
        public void Add(Payment payment)
        {
            if (!payments.TryAdd(payment.TransactionId, payment))
            {
                throw new DuplicatedKeyException($"Duplicated transaction id {payment.TransactionId} detected.");
            }
        }

        /// <summary>
        /// 주어진 결제 정보를 업데이트합니다.
        /// </summary>
        /// <param name="payment">결제</param>
        public void Update(Payment payment)
        {
            payments.AddOrUpdate(
                payment.TransactionId, 
                payment, 
                (transactionId, old) => payment);
        }

        /// <summary>
        /// 주어진 Id의 결제를 반환합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="transactionId">결제의 TransactionId</param>
        /// <returns>해당 Id의 주문 정보</returns>
        public Payment GetByTransactionId(string transactionId)
        {
            Payment payment = null;
            payments.TryGetValue(transactionId, out payment);
            return payment;
        }
    }
}
