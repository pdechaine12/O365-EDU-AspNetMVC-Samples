using Newtonsoft.Json;

namespace Microsoft.Education
{
    public class ArrayResult<T>
    {
        public T[] Value { get; set; }
        [JsonProperty("odata.nextLink")]
        public string NextLink { get; set; }
    }
}