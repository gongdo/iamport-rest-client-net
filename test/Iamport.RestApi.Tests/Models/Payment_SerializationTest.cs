using Iamport.RestApi.Models;
using Newtonsoft.Json;
using Xunit;

namespace Iamport.RestApi.Tests.Models
{
    public class Payment_SerializationTest
    {
        private const string EmptyPaymentJson = "{\"imp_uid\":null,\"merchant_uid\":null,\"pay_method\":\"card\",\"pg_provider\":\"inicis\",\"pg_tid\":null,\"escrow \":false,\"apply_num\":null,\"card_name\":null,\"card_quota\":0,\"vbank_name\":null,\"vbank_num\":null,\"vbank_holder\":null,\"vbank_date\":null,\"name\":null,\"amount\":0.0,\"cancel_amount\":0.0,\"currency\":null,\"buyer_name\":null,\"buyer_email\":null,\"buyer_tel\":null,\"buyer_addr\":null,\"buyer_postcode\":null,\"custom_data\":null,\"user_agent\":null,\"status\":\"ready\",\"paid_at\":null,\"failed_at\":null,\"cancelled_at\":null,\"fail_reason\":null,\"cancel_reason\":null,\"receipt_url\":null,\"cancel_receipt_urls\":null}";

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
            Assert.Equal(null, payment.ApplyNumber);
            Assert.Equal(null, payment.BuyerAddress);
            Assert.Equal(null, payment.BuyerEmail);
            Assert.Equal(null, payment.BuyerName);
            Assert.Equal(null, payment.BuyerPhoneNumber);
            Assert.Equal(null, payment.BuyerPostCode);
            Assert.Equal(0, payment.CancelledAmount);
            Assert.Equal(null, payment.CancelledAtUtc);
            Assert.Equal(null, payment.CancelledReason);
            Assert.Equal(null, payment.CancelledReceiptUrls);
            Assert.Equal(null, payment.CreditCardCompanyName);
            Assert.Equal(null, payment.Currency);
            Assert.Equal(null, payment.CustomData);
            Assert.Equal(false, payment.Escrow);
            Assert.Equal(null, payment.FailedAtUtc);
            Assert.Equal(null, payment.FailedReason);
            Assert.Equal(null, payment.IamportId);
            Assert.Equal(0, payment.InstallmentPlanPeriod);
            Assert.Equal(PaymentMethod.CreditCard, payment.Method);
            Assert.Equal(null, payment.PaidAtUtc);
            Assert.Equal(PaymentGateway.Inicis, payment.PaymentGateway);
            Assert.Equal(null, payment.PaymentGatewayTransactionId);
            Assert.Equal(null, payment.ReceiptUrl);
            Assert.Equal(PaymentStatus.Ready, payment.Status);
            Assert.Equal(null, payment.Title);
            Assert.Equal(null, payment.TransactionId);
            Assert.Equal(null, payment.UserAgent);
            Assert.Equal(null, payment.VirtualBankAccount);
            Assert.Equal(null, payment.VirtualBankAccountHolder);
            Assert.Equal(null, payment.VirtualBankExpirationUtc);
            Assert.Equal(null, payment.VirtualBankName);
        }
    }
}
