using Iamport.RestApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.AspNetCore.ViewModels
{
    /// <summary>
    /// 아임포트에 결제 요청할 때 필요한 뷰모델
    /// </summary>
    public class IamportRequestViewModel
    {
        /// <summary>
        /// 결제 요청 정보
        /// </summary>
        public PaymentRequest Request { get; set; }
        /// <summary>
        /// 복귀 URL.
        /// </summary>
        public string ReturnUrl { get; set; }
        /// <summary>
        /// 결제 정보 업데이트 URL.
        /// </summary>
        public string NotificationUrl { get; set; }
    }
}
