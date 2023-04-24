using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class AuthResponseModel : BaseOrderCloudResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
