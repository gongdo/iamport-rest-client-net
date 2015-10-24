using System.Collections.Generic;
using System.Linq;

namespace Iamport.RestApi.Models
{
    /// <summary>
    /// 금융사(은행)의 이름과 2자리 코드번호를 매핑하는 클래스입니다.
    /// </summary>
    public class FinancialCompanies
    {
        private const string DefaultName = "Unknown";
        private static Dictionary<string, string> CodeToNames = new Dictionary<string, string>
        {
            ["03"] = "기업은행",
            ["04"] = "국민은행",
            ["05"] = "외환은행",
            ["07"] = "수협중앙회",
            ["11"] = "농협중앙회",
            ["20"] = "우리은행",
            ["23"] = "SC제일은행",
            ["31"] = "대구은행",
            ["32"] = "부산은행",
            ["34"] = "광주은행",
            ["37"] = "전북은행",
            ["39"] = "경남은행",
            ["53"] = "한국씨티은행",
            ["71"] = "우체국",
            ["81"] = "하나은행",
            ["88"] = "통합신한은행 (신한,조흥은행)",
            ["D1"] = "동양종합금융증권",
            ["D2"] = "현대증권",
            ["D3"] = "미래에셋증권",
            ["D4"] = "한국투자증권",
            ["D5"] = "우리투자증권",
            ["D6"] = "하이투자증권",
            ["D7"] = "HMC투자증권",
            ["D8"] = "SK증권",
            ["D9"] = "대신증권",
            ["DA"] = "하나대투증권",
            ["DB"] = "굿모닝신한증권",
            ["DC"] = "동부증권",
            ["DD"] = "유진투자증권",
            ["DE"] = "메리츠증권",
            ["DF"] = "신영증권",
        };
        private static readonly Dictionary<string, string> NameToCodes;

        static FinancialCompanies()
        {
            NameToCodes = CodeToNames.ToDictionary(o => o.Value, o => o.Key);
        }

        /// <summary>
        /// 금융사 코드로 금융사의 이름을 반환합니다.
        /// </summary>
        /// <param name="code">코드</param>
        /// <param name="defaultName">코드가 발견되지 않았을 때 기본 이름</param>
        /// <returns>금융사의 이름</returns>
        public static string CodeToName(string code, string defaultName = DefaultName)
        {
            string name;
            if (CodeToNames.TryGetValue(code, out name))
            {
                return name;
            }
            return defaultName;
        }

        /// <summary>
        /// 금융사의 이름으로 금융사 코드를 반환합니다.
        /// </summary>
        /// <param name="name">금융사의 이름</param>
        /// <returns>발급사 코드, 발견되지 않으면 null</returns>
        public static string NameToCode(string name)
        {
            string code;
            if (NameToCodes.TryGetValue(name, out code))
            {
                return code;
            }
            return null;
        }
    }
}
