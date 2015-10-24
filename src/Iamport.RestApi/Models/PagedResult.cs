using Newtonsoft.Json;
using System.Collections.Generic;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 주어진 타입에 대한 페이지된 결과
    /// </summary>
    public class PagedResult<T> where T : class, new()
    {
        /// <summary>
        /// 결과의 총 개수.
        /// </summary>
        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
        /// <summary>
        /// 이전 페이지의 번호.
        /// 존재하지 않을 경우(즉, 현재 페이지가 첫 페이지일 경우) 0입니다.
        /// </summary>
        [JsonProperty(PropertyName = "previous")]
        public int Previous { get; set; }
        /// <summary>
        /// 다음 페이지의 번호.
        /// 존재하지 않을 경우(즉, 현재 페이지가 마지막 페이지일 경우) 0입니다.
        /// </summary>
        [JsonProperty(PropertyName = "next")]
        public int Next { get; set; }
        /// <summary>
        /// 조회한 현재 페이지의 결과 목록.
        /// </summary>
        [JsonProperty(PropertyName = "list")]
        public IEnumerable<T> List { get; set; }
    }
}
