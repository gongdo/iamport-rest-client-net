using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 페이지된 결과를 조회할 때 입력할 정보를 정의하는 클래스입니다.
    /// </summary>
    public class PageQuery
    {
        /// <summary>
        /// 조회할 페이지의 번호
        /// </summary>
        [DataMember(Name = "page")]
        [JsonProperty(PropertyName = "page")]
        public int Page { get; set; }
    }
}
