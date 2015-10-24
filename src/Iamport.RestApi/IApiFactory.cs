namespace Iamport.RestApi
{
    /// <summary>
    /// API를 생산하는 팩터리가 구현해야할 인터페이스.
    /// </summary>
    public interface IApiFactory
    {
        /// <summary>
        /// 주어진 타입의 API 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="T">API 클래스의 타입</typeparam>
        /// <returns>API 인스턴스</returns>
        T GetApi<T>() where T : class, IIamportApi;
    }
}
