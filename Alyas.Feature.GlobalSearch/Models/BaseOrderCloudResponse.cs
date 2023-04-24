using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class BaseOrderCloudResponse
    {
        [JsonIgnore]
        public string ErrorMessage { get; set; }

        [JsonIgnore]
        public bool IsSuccess { get; set; } = true;
    }
}
