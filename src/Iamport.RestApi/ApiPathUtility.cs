using System;

namespace Iamport.RestApi
{
    /// <summary>
    /// API에 관한 경로를 조작하는 유틸리티 클래스입니다.
    /// </summary>
    public static class ApiPathUtility
    {
        private static readonly string[] PathSeparators = new[] { "/" };

        /// <summary>
        /// 주어진 기본 경로 혹은 URL에 추가 경로 혹은 URL을 붙이거나 덮어써서 반환합니다.
        /// 추가 경로가 scheme으로 시작할 경우 baseUrlOrPath 전체를 무시합니다.
        /// 추가 경로가 ~/로 시작할 경우 baseUrlOrPath의 path부분을 무시합니다.
        /// 추가 경로가 그 외일 경우 baseUrlOrPath에 추가합니다.
        /// </summary>
        /// <param name="baseUrlOrPath">기본 경로 혹은 URL</param>
        /// <param name="addedUrlOrPath">추가 경로 혹은 URL</param>
        /// <returns>완성된 URL</returns>
        public static string Build(string baseUrlOrPath, string addedUrlOrPath)
        {
            if (string.IsNullOrEmpty(baseUrlOrPath))
            {
                return addedUrlOrPath;
            }
            if (string.IsNullOrEmpty(addedUrlOrPath))
            {
                return baseUrlOrPath;
            }
            if (addedUrlOrPath.IndexOf("://") > 0)
            {
                return addedUrlOrPath;
            }
            if (addedUrlOrPath.StartsWith("~/"))
            {
                return baseUrlOrPath
                    .Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries)[0]
                    + addedUrlOrPath.Substring(1);
            }
            return baseUrlOrPath.Trim('/') + "/" + addedUrlOrPath.TrimStart('/');
        }
    }
}
