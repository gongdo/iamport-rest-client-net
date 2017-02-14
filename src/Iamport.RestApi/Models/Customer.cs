using Iamport.RestApi.JsonConverters;
using Newtonsoft.Json;
using System;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 빌링키를 사용하는 구매자 정보
    /// </summary>
    /// <seealso href="http://api.iamport.kr/#!/subscribe.customer"/>
    public class Customer
    {
        /// <summary>
        /// 고객 고유번호(빌링키에 대응하는 고객 식별 문자열)
        /// </summary>
        [JsonProperty("customer_uid ")]
        public string Id { get; set; }
        /// <summary>
        /// 등록된 카드의 카드사 이름
        /// </summary>
        [JsonProperty("card_name")]
        public string CardName { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 관리용 이름
        /// </summary>
        [JsonProperty("customer_name")]
        public string Name { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 전화번호
        /// </summary>
        [JsonProperty("customer_tel")]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 이메일
        /// </summary>
        [JsonProperty("customer_email")]
        public string Email { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 주소
        /// </summary>
        [JsonProperty("customer_addr")]
        public string Address { get; set; }
        /// <summary>
        /// (optional)고객(카드소지자) 우편번호
        /// </summary>
        [JsonProperty("customer_postcode")]
        public string PostCode { get; set; }
        /// <summary>
        /// 빌키가 등록된 시각 UNIX timestamp(UTC)
        /// </summary>
        [JsonProperty("inserted")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime InsertedTime { get; set; }
        /// <summary>
        /// 빌키가 업데이트된 시각 UNIX timestamp(UTC)
        /// </summary>
        [JsonProperty("updated")]
        [JsonConverter(typeof(UnixDateTimeJsonConverter))]
        public DateTime UpdatedTime { get; set; }
    }
}
