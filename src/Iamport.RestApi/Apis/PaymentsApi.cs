using System;
using System.Threading.Tasks;
using Iamport.RestApi.Models;
using System.Net.Http;
using Iamport.RestApi.Extensions;

namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Payments API의 기본 구현 클래스입니다.
    /// </summary>
    public class PaymentsApi : IPaymentsApi
    {
        private readonly IIamportHttpClient client;
        public PaymentsApi(IIamportHttpClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            this.client = client;
        }

        public string BasePath { get; } = "/payments";

        public async Task<Payment> CancelAsync(PaymentCancellation cancellation)
        {
            if (cancellation == null)
            {
                throw new ArgumentNullException(nameof(cancellation));
            }
            var request = new IamportRequest<PaymentCancellation>
            {
                ApiPathAndQueryString = GetPathAndQuerystring("cancel"),
                Content = cancellation,
                Method = HttpMethod.Post
            };
            return await SendRequestAsync<PaymentCancellation, Payment>(request);
        }

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

        public async Task<Payment> GetByTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = GetPathAndQuerystring($"find/{transactionId}"),
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, Payment>(request);
        }

        public async Task<PaymentPreparation> GetPreparationAsync(string transactionId)
        {
            if (string.IsNullOrEmpty(transactionId))
            {
                throw new ArgumentNullException(nameof(transactionId));
            }
            var request = new IamportRequest
            {
                ApiPathAndQueryString = GetPathAndQuerystring($"prepare/{transactionId}"),
                Method = HttpMethod.Get,
            };
            return await SendRequestAsync<object, PaymentPreparation>(request);
        }

        public async Task<PaymentPreparation> PrepareAsync(PaymentPreparation preparation)
        {
            if (preparation == null)
            {
                throw new ArgumentNullException(nameof(preparation));
            }
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
            if (response.Code != Iamport.ResponseSuccessCode)
            {
                throw new IamportResponseException(response.Code, response.Message);
            }
            return response.Content;
        }
    }
}
