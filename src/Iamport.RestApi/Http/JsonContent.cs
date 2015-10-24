using Newtonsoft.Json;
using System.Text;

namespace System.Net.Http
{
    /// <summary>
    /// Provides HTTP content based on a string.
    /// </summary>
    public class JsonContent : StringContent
    {
        private const string JsonContentType = "application/json";

        /// <summary>
        /// Initializes a new instance of the System.Net.Http.JsonContent class
        /// with default settings.
        /// </summary>
        /// <param name="content">The content to used to initialize the instance.</param>
        public JsonContent(object content)
            : base(JsonConvert.SerializeObject(content), Encoding.UTF8, JsonContentType) { }
        /// <summary>
        /// Initializes a new instance of the System.Net.Http.JsonContent class
        /// with given serialization settings.
        /// </summary>
        /// <param name="content">The content to used to initialize the instance.</param>
        /// <param name="settings">The settings to used to serialize the content.</param>
        public JsonContent(object content, JsonSerializerSettings settings)
            : base(JsonConvert.SerializeObject(content, settings), Encoding.UTF8, JsonContentType) { }
        /// <summary>
        /// Initializes a new instance of the System.Net.Http.JsonContent class
        /// with given json content and default settings.
        /// </summary>
        /// <param name="json">The json content to used to initialize the instance.</param>
        public JsonContent(string json) : base(json, Encoding.UTF8, JsonContentType) { }
        /// <summary>
        /// Initializes a new instance of the System.Net.Http.JsonContent class
        /// with given parameters.
        /// </summary>
        /// <param name="json">The json content to used to initialize the instance.</param>
        /// <param name="encoding">The encoding to used to encode the content.</param>
        public JsonContent(string json, Encoding encoding) : base(json, encoding, JsonContentType) { }
    }
}
