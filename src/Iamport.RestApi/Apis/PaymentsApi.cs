using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Payments API의 기본 구현 클래스입니다.
    /// </summary>
    public class PaymentsApi : IPaymentsApi
    {
        public string BasePath { get; } = "/payments";

        public async Task<Payment> CancelAsync(PaymentCancellation cancellation)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<Payment>> GetAsync(PaymentPageQuery query)
        {
            throw new NotImplementedException();
        }

        public async Task<Payment> GetByIamportIdAsync(string iamportId)
        {
            throw new NotImplementedException();
        }

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentPreparation> GetPreparationAsync(string transactionId)
        {
            throw new NotImplementedException();
        }

        public async Task<PaymentPreparation> PrepareAsync(PaymentPreparation preparation)
        {
            throw new NotImplementedException();
        }
    }
}
