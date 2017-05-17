using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using System.Net.Http;
using Iamport.RestApi.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Payments API의 기본 구현 클래스입니다.
    /// </summary>
    public class PaymentsApi : IPaymentsApi
    {
        private readonly IIamportClient client;

        /// <summary>
        /// 주어진 클라이언트로 API를 초기화합니다.
        /// </summary>
        /// <param name="client">아임포트 클라이언트</param>
        public PaymentsApi(IIamportClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            this.client = client;
        }

        /// <summary>
        /// Payments API의 기본 경로
        /// </summary>
        public string BasePath { get; } = "/payments";

        /// <summary>
        /// 주어진 입력에 해당하는 결제를 취소하고 결제 결과를 반환합니다.
        /// 해당하는 결제 결과가 없으면 null을 반환합니다.
        /// </summary>
        /// <param name="cancellation">취소 정보</param>
        /// <seealso>https://api.iamport.kr/#!/payments/cancelPayment</seealso>
        /// <returns>결제 결과</returns>
        public async Task<Payment> CancelAsync(PaymentCancellation cancellation)
        {
            if (cancellation == null)
            {
                throw new ArgumentNullException(nameof(cancellation));
            }
            ValidateObject(cancellation);
            var request = new IamportRequest<PaymentCancellation>
            {
                ApiPathAndQueryString = GetPathAndQuerystring("cancel"),
                Content = cancellation,
                Method = HttpMethod.Post
            };
            return await SendRequestAsync<PaymentCancellation, Payment>(request);
        }

        /// <summary>
        /// 주어진 조회 조건에 해당하는 결제 결과를 조회합니다.
        /// </summary>
        /// <param name="query">결제 조회 조건</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentsByStatus</seealso>
        /// <returns>결제 결과의 목록</returns>
        public async Task<PagedResult<Payment>> GetAsync(PaymentPageQuery query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            var pathAndQuerystring = GetPathAndQuerystring($"status/{query.State.GetMemberValue()}");
            if (query.Page > 0)
            {
                pathAndQuerystring += $"?page={query.Page}";
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = pathAndQuerystring,
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, PagedResult<Payment>>(request);
        }

        /// <summary>
        /// 아임포트 고유 ID에 해당하는 결제 결과를 조회합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="iamportId">아임포트 고유 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentByImpUid</seealso>
        /// <returns>결제 결과</returns>
        public async Task<Payment> GetByIamportIdAsync(string iamportId)
        {
            if (string.IsNullOrEmpty(iamportId))
            {
                throw new ArgumentNullException(nameof(iamportId));
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = GetPathAndQuerystring(iamportId),
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, Payment>(request);
        }

        /// <summary>
        /// 거래 ID에 해당하는 결제 결과를 조회합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments/getPaymentByMerchantUid</seealso>
        /// <returns>결제 결과</returns>
        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = GetPathAndQuerystring($"find/{WebUtility.UrlEncode(transactionId)}"),
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, Payment>(request);
        }

        /// <summary>
        /// 주어진 거래 ID에 해당하는 결제 준비 정보를 반환합니다.
        /// 존재하지 않을 경우 null을 반환합니다.
        /// </summary>
        /// <param name="transactionId">거래 ID</param>
        /// <seealso>https://api.iamport.kr/#!/payments.validation/getPaymentPrepareByMerchantUid</seealso>
        /// <returns>결제 준비 정보</returns>
        public async Task<PaymentPreparation> GetPreparationAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = GetPathAndQuerystring($"prepare/{WebUtility.UrlEncode(transactionId)}"),
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, PaymentPreparation>(request);
        }

        /// <summary>
        /// 주어진 정보로 결제를 준비합니다.
        /// 준비된 결제는 동일한 거래 ID에 대해 단 한번만 결제를 진행할 수 있으며,
        /// 결제 진행중 등록된 금액과 다를 경우 결제에 실패합니다.
        /// </summary>
        /// <param name="preparation">결제 준비 정보</param>
        /// <seealso>https://api.iamport.kr/#!/payments.validation/preparePayment</seealso>
        /// <returns>결제 준비 정보</returns>
        public async Task<PaymentPreparation> PrepareAsync(PaymentPreparation preparation)
        {
            if (preparation == null)
            {
                throw new ArgumentNullException(nameof(preparation));
            }
            ValidateObject(preparation);
            var request = new IamportRequest<PaymentPreparation>
            {
                ApiPathAndQueryString = GetPathAndQuerystring($"prepare"),
                Method = HttpMethod.Post,
                Content = preparation
            };
            return await SendRequestAsync<PaymentPreparation, PaymentPreparation>(request);
        }

        private string GetPathAndQuerystring(string pathAndQuerystring)
        {
            return ApiPathUtility.Build(BasePath, pathAndQuerystring);
        }

        private async Task<TResult> SendRequestAsync<TRequest, TResult>(IamportRequest<TRequest> request)
        {
            var response = await client.RequestAsync<TRequest, TResult>(request);
            if (response.Code != Constants.ResponseSuccessCode)
            {
                throw new IamportResponseException(response.Code, response.Message);
            }
            return response.Content;
        }

        private void ValidateObject(object value)
        {
            var context = new ValidationContext(value);
            Validator.ValidateObject(value, context, true);
        }
    }
}
