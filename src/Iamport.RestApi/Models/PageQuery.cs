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
        /// <summary>
        /// 조회할 페이지의 크기
        /// *현재 API에는 구현되어 있지 않습니다.
        /// </summary>
        //[DataMember(Name = "pageSize")]
        //public int PageSize { get; set; }
    }
}
