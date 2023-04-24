using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class GetProductsResponseModel
    {
        [JsonPropertyName("Items")]
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}
