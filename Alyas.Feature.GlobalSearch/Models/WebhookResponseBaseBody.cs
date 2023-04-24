using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class WebhookResponseBaseBody
    {
        [JsonPropertyName("ID")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}