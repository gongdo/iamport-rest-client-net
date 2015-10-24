namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 아임포트 API에서 응답에 사용하는 모델을 정의하는 클래스입니다.
    /// </summary>
    /// <typeparam name="T">실제 응답 내용의 타입</typeparam>
    public class IamportResponse<T>
    {
        /// <summary>
        /// 응답 코드, 0이면 성공 그렇지 않으면 실패를 의미합니다.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 응답 메시지, 응답 코드가 0이 아닐 경우 오류 메시지입니다.
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 응답 내용, 요청에 따른 응답 내용입니다.
        /// </summary>
        public T Response { get; set; }
    }
}
