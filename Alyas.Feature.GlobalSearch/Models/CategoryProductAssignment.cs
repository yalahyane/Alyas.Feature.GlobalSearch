using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class CategoryProductAssignment
    {
        [JsonPropertyName("CategoryID")]
        public string CategoryId { get; set; }
        [JsonPropertyName("ProductID")]
        public string ProductId { get; set; }
        public int? ListOrder { get; set; }
    }
}
