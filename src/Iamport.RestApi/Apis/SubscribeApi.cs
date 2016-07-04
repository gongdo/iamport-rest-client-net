namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Subscribe API의 기본 구현 클래스입니다.
    /// </summary>
    public class SubscribeApi : ISubscribeApi
    {
        private readonly IIamportClient client;

        public string BasePath { get; } = "/subscribe/payments";

        public SubscribeApi(IIamportClient client)
        {
            this.client = client;
        }

        // TODO: 실제 API는 아직 구현 안됨.
    }
}
