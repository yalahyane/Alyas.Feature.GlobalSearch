using SolrNet.Attributes;

namespace Alyas.Feature.GlobalSearch.Models
{
    public class ProductDocument
    {
        [SolrUniqueKey("_uniqueid")] public string UniqueId { get; set; }
        [SolrField("_name")] public string Name { get; set; }
        [SolrField("_displayname")] public string DisplayName { get; set; }
        [SolrField("_language")] public string Language { get; set; }
        [SolrField("sxacontent_txm")] public List<string> SxaContent { get; set; }
        [SolrField("_indexname")] public string IndexName { get; set; }
        [SolrField("_fullpath")] public string FullPath { get; set; }
        [SolrField("_datasource")] public string Datasource { get; set; } = "OrderCloud";
        [SolrField("searchable_b")] public bool Searchable { get; set; } = true;
        [SolrField("_latestversion")] public bool LatestVersion { get; set; } = true;
        [SolrField("_path")] public string Path { get; set; } = "fe500b584a1f4004bc527a7ff1baa0e3"; // Your Home Page Item
    }
}
