using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iamport.RestApi
{
    /// <summary>
    /// 아임포트 API 클래스를 설명하는 어트리뷰트입니다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ApiAttribute : Attribute
    {
        public ApiAttribute(string basePath)
        {
            BasePath = basePath;
        }

        /// <summary>
        /// 이 API의 기본 경로를 지정합니다.
        /// </summary>
        /// <remarks>
        /// 기본 경로가 "/"로 시작하지 않을 경우 IamportClient의 기본 URL 뒤에 경로를 붙입니다.
        /// 기본 경로가 "/"로 시작할 경우 IamportClient의 기본 URL에 포함된 경로를 덮어씁니다.
        /// 기본 경로가 scheme로 시작할 경우 IamportClient의 기본 URL 전체를 덮어씁니다.
        /// </remarks>
        public string BasePath { get; set; }
    }
}
