using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;
using SolrNet;

namespace Alyas.Feature.GlobalSearch.Services
{
    public class SolrService  : ISolrService
    {
        private readonly ILogger<SolrService> _logger;
        public SolrService(ILogger<SolrService> logger)
        {
            this._logger = logger;
        }
        public void InitializeSolr(SolrConfiguration solrConfiguration, string indexName)
        {
            try
            {
                SolrIndexManager.InitContainer();
                SolrIndexManager.Init<ProductDocument>(solrConfiguration.SolrUrl, indexName);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to Initialize Solr for Full Indexing: {e}");
            }
        }

        public async Task MergeOrAddDocumentsAsync(string indexName, IEnumerable<ProductDocument> products)
        {
            var solrOperations = SolrIndexManager.Container.GetInstance<ISolrOperations<ProductDocument>>();
            await solrOperations.AddRangeAsync(products);
            await solrOperations.CommitAsync();
        }

        public async Task DeleteDocumentAsync(string indexName, ProductDocument product)
        {
            var solrOperations = SolrIndexManager.Container.GetInstance<ISolrOperations<ProductDocument>>();
            await solrOperations.DeleteAsync(product);
            await solrOperations.CommitAsync();
        }
    }
}
