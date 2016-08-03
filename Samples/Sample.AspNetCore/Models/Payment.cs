using Iamport.RestApi.Models;
using System;

namespace Sample.AspNetCore.Models
{
    /// <summary>
    /// 결제 처리 상태
    /// </summary>
    public enum PaymentState
    {
        /// <summary>
        /// 준비됨; 결제 고유 ID와 금액을 확정함(imp_uid는 알 수 없음)
        /// </summary>
        Prepared,
        /// <summary>
        /// 진행중; 결제 요청이 시작됨(imp_uid는 알 수 없음)
        /// </summary>
        InProgress,
        /// <summary>
        /// 입금 대기중; 현재 결제 수단에 대한 지불 대기중(imp_uid를 알 수 있음)
        /// </summary>
        Awating,
        /// <summary>
        /// 결제 성공; imp_uid도 기록됨
        /// </summary>
        Paid,
        /// <summary>
        /// 결제 실패; imp_uid도 기록됨
        /// </summary>
        Failed,
        /// <summary>
        /// 결제 취소; 성공한 결제를 취소 처리
        /// </summary>
        Cancelled,
    }

    /// <summary>
    /// 특정 구매 정보에 대한 결제 처리 엔터티
    /// </summary>
    public class Payment
    {
        public static Payment Create(
            Guid checkoutId)
        {
            // TODO:
            // 다음의 TransactionId 생성은 중요한 비즈니스 로직입니다.
            // 비즈니스 영역에서 Id를 관리할 수 있도록 합니다.
            return new Payment
            {
                CheckoutId = checkoutId,
                TransactionId = Guid.NewGuid().ToString().Replace("-", ""),
                CreatedTime = DateTimeOffset.UtcNow,
                State = PaymentState.Prepared,
            };
        }

        /// <summary>
        /// 결제할 구매 정보의 ID
        /// </summary>
        public Guid CheckoutId { get; private set; }
        /// <summary>
        /// 결제 요청 시각
        /// </summary>
        public DateTimeOffset CreatedTime { get; private set; }
        /// <summary>
        /// 결제를 고유하게 구분하는 거래 ID.
        /// </summary>
        public string TransactionId { get; private set; }
        /// <summary>
        /// 결제 처리를 수행하는 대리자에서 발급한 고유 ID.
        /// 이 샘플 애플리케이션에서는 아임포트의 imp_uid를 의미합니다.
        /// </summary>
        public string PaymentDelegateId { get; private set; }
        /// <summary>
        /// 결제 진행상태.
        /// </summary>
        public PaymentState State { get; private set; }
        
        /*
         * 다음은 결제 진행과정에 따라 결제사로부터 얻을 수 있는 정보입니다.
         * 편의상 옵셔널한 정보를 모두 포함합니다.
         * 필요에 따라 세부 클래스를 나눌 수도 있습니다.
         */

        /// <summary>
        /// PG사 승인정보
        /// </summary>
        public string PaymentGatewayTransactionId { get; private set; }

        /// <summary>
        /// 카드사 승인정보(계좌이체 / 가상계좌는 값 없음)
        /// </summary>
        public string ApplyNumber { get; private set; }
        /// <summary>
        /// 카드사 명칭
        /// </summary>
        public string CreditCardCompanyName { get; private set; }
        /// <summary>
        /// 할부개월 수(0이면 일시불)
        /// </summary>
        public int InstallmentPlanPeriod { get; private set; }

        /// <summary>
        /// 입금받을 가상계좌 은행명
        /// </summary>
        public string VirtualBankName { get; private set; }
        /// <summary>
        /// 입금받을 가상계좌 계좌번호
        /// </summary>
        public string VirtualBankAccount { get; private set; }
        /// <summary>
        /// 입금받을 가상계좌 예금주
        /// </summary>
        public string VirtualBankAccountHolder { get; private set; }

        /// <summary>
        /// 결제 성공 시각
        /// </summary>
        public DateTimeOffset? PaidTime { get; private set; }
        
        /// <summary>
        /// 결제 실패 시각
        /// </summary>
        public DateTimeOffset? FailedTime { get; private set; }
        /// <summary>
        /// 결제실패 사유
        /// </summary>
        public string FailedReason { get; private set; }

        /// <summary>
        /// 결제 취소 시각
        /// </summary>
        public DateTimeOffset? CancelledTime { get; private set; }
        /// <summary>
        /// 결제취소 사유
        /// </summary>
        public string CancelledReason { get; private set; }
        /// <summary>
        /// 결제취소금액
        /// </summary>
        public decimal CancelledAmount { get; private set; }


        /*
         * 비즈니스 로직
         */

        public void SetInProgress(string paymentDelegateId)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.InProgress;
        }

        public void SetPaidForCreditCard(
            string paymentDelegateId,
            DateTimeOffset? paidTime,
            string paymentGatewayTransactionId,
            string applyNumber,
            string creditCardCompanyName,
            int installmentPlanPeriod)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Paid;
            PaidTime = paidTime.HasValue ? paidTime.Value : DateTimeOffset.UtcNow;
            PaymentGatewayTransactionId = paymentGatewayTransactionId;
            ApplyNumber = applyNumber;
            CreditCardCompanyName = creditCardCompanyName;
        }

        public void SetAwatingForVirtualBank(
            string paymentDelegateId,
            string paymentGatewayTransactionId,
            string bankName,
            string account,
            string accountHolder)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Awating;
            PaymentGatewayTransactionId = paymentGatewayTransactionId;
            VirtualBankName = bankName;
            VirtualBankAccount = account;
            VirtualBankAccountHolder = accountHolder;
        }

        public void SetPaiedForVirtualBank(
            string paymentDelegateId,
            DateTimeOffset? paidTime,
            string paymentGatewayTransactionId,
            string bankName,
            string account,
            string accountHolder)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Paid;
            PaidTime = paidTime.HasValue ? paidTime.Value : DateTimeOffset.UtcNow;
            PaymentGatewayTransactionId = paymentGatewayTransactionId;
            VirtualBankName = bankName;
            VirtualBankAccount = account;
            VirtualBankAccountHolder = accountHolder;
        }

        public void SetPaidForOthers(
            string paymentDelegateId,
            DateTimeOffset? paidTime,
            string paymentGatewayTransactionId)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Paid;
            PaidTime = paidTime.HasValue ? paidTime.Value : DateTimeOffset.UtcNow;
            PaymentGatewayTransactionId = paymentGatewayTransactionId;
        }

        public void SetFailed(
            string paymentDelegateId,
            DateTimeOffset? failedTime,
            string failedReason)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Failed;
            FailedTime = failedTime.HasValue ? failedTime.Value : DateTimeOffset.UtcNow;
            FailedReason = failedReason;
        }

        public void SetCancelled(
            string paymentDelegateId,
            DateTimeOffset? cancelledTime,
            string cancelledReason,
            decimal cancelledAmount)
        {
            PaymentDelegateId = paymentDelegateId;
            State = PaymentState.Failed;
            CancelledTime = cancelledTime.HasValue ? cancelledTime.Value : DateTimeOffset.UtcNow;
            CancelledReason = cancelledReason;
            CancelledAmount = cancelledAmount;
        }
    }
}
