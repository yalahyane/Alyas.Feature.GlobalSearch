using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class WebhookRequestBody
    {
        [JsonPropertyName("CategoryID")]
        public string? CategoryId { get; set; }
        [JsonPropertyName("ProductID")]
        public string? ProductId { get; set; }
    }
}