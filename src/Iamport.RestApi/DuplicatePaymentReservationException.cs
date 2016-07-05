namespace Iamport.RestApi
{
    /// <summary>
    /// 중복된 결제 예약을 나타내는 예외입니다.
    /// </summary>
    public class DuplicatePaymentReservationException: IamportResponseException
    {
        /// <summary>
        /// 주어진 코드와 메시지로 예외를 초기화합니다.
        /// </summary>
        /// <param name="code">오류 코드</param>
        /// <param name="message">오류 메시지</param>
        public DuplicatePaymentReservationException(int code, string message)
            : base(code, message)
        {
        }
    }
}
