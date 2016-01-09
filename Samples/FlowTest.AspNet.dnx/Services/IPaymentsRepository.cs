namespace FlowTest.AspNet.dnx.Services
{
    /// <summary>
    /// 테스트용 Payment 저장소 인터페이스
    /// </summary>
    public interface IPaymentsRepository
    {
        Models.Payment Add(Models.Payment payment);
        Models.Payment GetByTransactionId(string transactionId);
        Models.Payment Update(Models.Payment payment);
    }
}
