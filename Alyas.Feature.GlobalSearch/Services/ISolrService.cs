using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;

namespace Alyas.Feature.GlobalSearch.Services
{
    public interface ISolrService
    {
        void InitializeSolr(SolrConfiguration solrConfiguration, string indexName);
        Task MergeOrAddDocumentsAsync(string indexName, IEnumerable<ProductDocument> products);
        Task DeleteDocumentAsync(string indexName, ProductDocument product);
    }
}
