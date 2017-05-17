using System.Net;
using Xunit;

namespace Iamport.RestApi.Tests
{
    public class ApiPathUtilTest
    {
        [Theory]
        [InlineData(null, "second")]
        [InlineData("", "second")]
        public void Returns_secondary_when_base_is_null_or_empty(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal(secondPath, actual);
        }
        [Theory]
        [InlineData("base", null)]
        [InlineData("base", "")]
        public void Returns_base_when_secondary_is_null_or_empty(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal(basePath, actual);
        }
        [Theory]
        [InlineData("base", "second://bbb")]
        [InlineData("base", "http://aaa")]
        public void Returns_secondary_when_secondary_contains_scheme(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal(secondPath, actual);
        }
        [Theory]
        [InlineData("base/api", "~/second")]
        [InlineData("base/main/1/2/3", "~/second")]
        public void Returns_aggregated_path_when_secondary_starts_with_trail_slash(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal("base/second", actual);
        }
        [Theory]
        [InlineData("base/api/", "/second")]
        [InlineData("base/api", "second")]
        [InlineData("base/api/", "second")]
        [InlineData("base/api", "/second")]
        public void Returns_concated_path(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal("base/api/second", actual);
        }
        [Theory]
        [InlineData("base/api/", "/가나다/あいう#?test테스트")]
        [InlineData("base/api", "가나다/あいう#?test테스트")]
        [InlineData("base/api/", "가나다/あいう#?test테스트")]
        [InlineData("base/api", "/가나다/あいう#?test테스트")]
        public void Returns_url_encoded_path(string basePath, string secondPath)
        {
            var actual = ApiPathUtility.Build(basePath, secondPath);
            Assert.Equal($"base/api/{WebUtility.UrlEncode("가나다/あいう#?test테스트")}", actual);
        }
    }
}
