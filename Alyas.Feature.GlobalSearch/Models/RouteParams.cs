using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class RouteParams
    {
        [JsonPropertyName("catalogID")]
        public string? CatalogId { get; set; }
        [JsonPropertyName("categoryID")]
        public string? CategoryId { get; set; }
        [JsonPropertyName("productID")]
        public string? ProductId { get; set; }
    }
}
