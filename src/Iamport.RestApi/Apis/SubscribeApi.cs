namespace Iamport.RestApi.Apis
{
    /// <summary>
    /// Subscribe API의 기본 구현 클래스입니다.
    /// </summary>
    public class SubscribeApi : ISubscribeApi
    {
        private readonly IIamportClient client;

        /// <summary>
        /// Subscribe API의 기본 경로
        /// </summary>
        public string BasePath { get; } = "/subscribe/payments";

        /// <summary>
        /// 주어진 클라이언트로 API를 초기화합니다.
        /// </summary>
        /// <param name="client">아임포트 클라이언트</param>
        public SubscribeApi(IIamportClient client)
        {
            this.client = client;
        }

        // TODO: 실제 API는 아직 구현 안됨.
    }
}
