using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class Product
    {
        [JsonPropertyName("ID")]
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonPropertyName("xp")]
        public object Xp { get; set; }
        public string Description { get; set; }
        public PriceSchedule? PriceSchedule { get; set; }
    }
}
