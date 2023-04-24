using System.Text.Json.Serialization;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class GetCategoryProductAssignmentsResponseModel
    {
        [JsonPropertyName("Items")]
        public IEnumerable<CategoryProductAssignment> CategoryProductAssignments { get; set; } = new List<CategoryProductAssignment>();
    }
}
