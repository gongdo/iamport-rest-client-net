using Iamport.RestApi.Models;
using Newtonsoft.Json;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class Payment_SerializationTest
    {
        private const string EmptyPaymentJson = "{\"success\":false,\"error_code\":null,\"error_msg\":null,\"imp_uid\":null,\"merchant_uid\":null,\"pay_method\":\"card\",\"paid_amount\":0.0,\"status\":\"ready\",\"name\":null,\"pg_provider\":\"inicis\",\"pg_tid\":null,\"buyer_name\":null,\"buyer_email\":null,\"buyer_tel\":null,\"buyer_addr\":null,\"buyer_postcode\":null,\"custom_data\":null,\"paid_at\":null,\"receipt_url\":null,\"failed_at\":null,\"cancelled_at\":null,\"fail_reason\":null,\"cancel_reason\":null,\"apply_num\":null,\"card_name\":null,\"card_quota\":0,\"vbank_name\":null,\"vbank_num\":null,\"vbank_holder\":null,\"vbank_date\":null,\"cancel_amount\":0,\"user_agent\":null}";

        [Fact]
        public void Serializes_empty_Payment()
        {
            var expected = EmptyPaymentJson;
            var payment = new Payment();
            var json = JsonConvert.SerializeObject(payment);
            Assert.Equal(expected, json);
        }

        [Fact]
        public void Deserialize_empty_Payment()
        {
            var payment = JsonConvert.DeserializeObject<Payment>(EmptyPaymentJson);
            Assert.Equal(0, payment.Amount);
            Assert.Null(payment.ApplyNumber);
            Assert.Null(payment.BuyerAddress);
            Assert.Null(payment.BuyerEmail);
            Assert.Null(payment.BuyerName);
            Assert.Null(payment.BuyerPhoneNumber);
            Assert.Null(payment.BuyerPostCode);
            Assert.Equal(0, payment.CancelledAmount);
            Assert.Null(payment.CancelledAtUtc);
            Assert.Null(payment.CancelledReason);
            Assert.Null(payment.CreditCardCompanyName);
            Assert.Null(payment.CustomData);
            Assert.Null(payment.ErrorCode);
            Assert.Null(payment.ErrorMessage);
            Assert.Null(payment.FailedAtUtc);
            Assert.Null(payment.FailedReason);
            Assert.Null(payment.IamportId);
            Assert.Equal(0, payment.InstallmentPlanPeriod);
            Assert.False(payment.IsSuccess);
            Assert.Equal(PaymentMethod.CreditCard, payment.Method);
            Assert.Equal(PaymentGateway.Inicis, payment.PaymentGateway);
            Assert.Null(payment.PaidAtUtc);
            Assert.Null(payment.PaymentGatewayTransactionId);
            Assert.Null(payment.ReceiptUrl);
            Assert.Equal(PaymentStatus.Ready, payment.Status);
            Assert.Null(payment.Title);
            Assert.Null(payment.TransactionId);
            Assert.Null(payment.UserAgent);
            Assert.Null(payment.VirtualBankAccount);
            Assert.Null(payment.VirtualBankAccountHolder);
            Assert.Null(payment.VirtualBankExpirationUtc);
            Assert.Null(payment.VirtualBankName);
        }
    }
}
