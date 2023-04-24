namespace Alyas.Feature.GlobalSearch.Models
{
    public class IncrementalProductIndexingRequest
    {
        public string? Route { get; set; }
        public RouteParams? RouteParams { get; set; }
        public string? Verb { get; set; }
        public WebhookRequest? Request { get; set; }
        public WebhookResponse? Response { get; set; }
        public ConfigData? ConfigData { get; set; }
    }
}
