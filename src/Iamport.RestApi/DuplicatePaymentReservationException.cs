namespace Iamport.RestApi
{
    /// <summary>
    /// 중복된 결제 예약을 나타내는 예외입니다.
    /// </summary>
    public class DuplicatePaymentReservationException: IamportResponseException
    {
        public DuplicatePaymentReservationException(int code, string message) : base(code, message)
        {
        }
    }
}
