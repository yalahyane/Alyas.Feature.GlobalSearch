namespace Alyas.Feature.GlobalSearch.Models
{
    public class PriceSchedule
    {
        public IEnumerable<PriceBreak> PriceBreaks { get; set; } = new List<PriceBreak>();
    }
}
