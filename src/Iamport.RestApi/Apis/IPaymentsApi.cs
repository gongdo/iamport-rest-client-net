using Iamport.RestApi.Models;
using System.Threading.Tasks;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// 결제에 관한 API가 구현해야할 인터페이스입니다.
    /// </summary>
    public interface IPaymentsApi : IIamportApi
    {
        /// <summary>
        /// 주어진 정보로 결제를 준비합니다.
        /// 준비된 결제는 동일한 거래 ID에 대해 단 한번만 결제를 진행할 수 있으며,
        /// 결제 진행중 등록된 금액과 다를 경우 결제에 실패합니다.
        /// </summary>
        /// <param name="preparation">결제 준비 정보</param>
        /// <seealso>https://api.iamport.kr/#!/payments.validation/preparePayment</seealso>
        /// <returns>결제 준비 정보</returns>
        Task<PaymentPreparation> PrepareAsync(PaymentPreparation preparation);

        /// <summary>
        /// 주어진 거래 ID에 해당하는 결제 준비 정보를 반환합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments.validation/getPaymentPrepareByMerchantUid</seealso>
        /// <returns>결제 준비 정보</returns>
        Task<PaymentPreparation> GetPreparationAsync(string transactionId);

        /// <summary>
        /// 아임포트 고유 ID에 해당하는 결제 결과를 조회합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="iamportId">아임포트 고유 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentByImpUid</seealso>
        /// <returns>결제 결과</returns>
        Task<Payment> GetByIamportIdAsync(string iamportId);

        /// <summary>
        /// 거래 ID에 해당하는 결제 결과를 조회합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentByMerchantUid</seealso>
        /// <returns>결제 결과</returns>
        Task<Payment> GetByTransactionIdAsync(string transactionId);

        /// <summary>
        /// 주어진 조회 조건에 해당하는 결제 결과를 조회합니다.
        /// </summary>
        /// <param name="query">결제 조회 조건</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentsByStatus</seealso>
        /// <returns>결제 결과의 목록</returns>
        Task<PagedResult<Payment>> GetAsync(PaymentPageQuery query);

        /// <summary>
        /// 주어진 입력에 해당하는 결제를 취소하고 결제 결과를 반환합니다.
        /// 해당하는 결제 결과가 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="cancellation">취소 정보</param>
        /// <seealso>https://api.iamport.kr/#!/payments/cancelPayment</seealso>
        /// <returns>결제 결과</returns>
        Task<Payment> CancelAsync(PaymentCancellation cancellation);
    }
}
