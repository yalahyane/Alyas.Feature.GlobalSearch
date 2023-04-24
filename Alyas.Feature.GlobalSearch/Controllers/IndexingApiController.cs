using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;
using Alyas.Feature.GlobalSearch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Alyas.Feature.GlobalSearch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexingApiController : Controller
    {
        private readonly IOrderCloudService _orderCloudService;
        private readonly ISolrService _solrService;
        private readonly OrderCloudConfiguration _orderCloudConfiguration;
        private readonly SolrConfiguration _solrConfiguration;

        public IndexingApiController(IOrderCloudService orderCloudService, ISolrService solrService, IOptions<OrderCloudConfiguration> orderCloudConfigurationAccessor, IOptions<SolrConfiguration> solrConfiguration)
        {
            this._orderCloudService = orderCloudService;
            _solrService = solrService;
            this._solrConfiguration = solrConfiguration.Value;
            this._orderCloudConfiguration = orderCloudConfigurationAccessor.Value;
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<JsonResult> FullProductIndexing(string indexName)
        {
            var page = 1;
            var productsBatch = await this._orderCloudService.GetAllProductsAsync(this._orderCloudConfiguration, page, 100);
            this._solrService.InitializeSolr(this._solrConfiguration, indexName);
            var productDocuments = new List<ProductDocument>();
            while (productsBatch.Products.Any())
            {
                page++;
                foreach (var product in productsBatch.Products)
                {
                    productDocuments.Add(await GenerateProductDocument(product, indexName));
                }
                await this._solrService.MergeOrAddDocumentsAsync(indexName, productDocuments);
                productsBatch = await this._orderCloudService.GetAllProductsAsync(this._orderCloudConfiguration, page, 100);
            }

            return new JsonResult(new {IsSuccess = true, TotalDocumentsIndexed = page == 1? 0 : productDocuments.Count + 100 * (page - 2)});
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<JsonResult> IncrementalProductIndexing(IncrementalProductIndexingRequest request)
        {
            var productId = request.RouteParams.ProductId;
            var indexName = request.ConfigData.IndexName; 
            this._solrService.InitializeSolr(this._solrConfiguration, indexName);

            if (request.Verb.Equals("Delete", StringComparison.OrdinalIgnoreCase) && request.Route.Equals("v1/products/{productID}", StringComparison.OrdinalIgnoreCase))
            {
               await this._solrService.DeleteDocumentAsync(indexName, new ProductDocument {UniqueId = productId});
            }
            else if ((request.Verb.Equals("PUT", StringComparison.OrdinalIgnoreCase) || request.Verb.Equals("PATCH", StringComparison.OrdinalIgnoreCase)) &&
                     request.Route.Equals("v1/products/{productID}", StringComparison.OrdinalIgnoreCase))
            {
                var document = await GenerateProductDocument(request.Response.Body, indexName);
                await this._solrService.MergeOrAddDocumentsAsync(indexName, new List<ProductDocument>{document});
            }
            else if (request.Verb.Equals("POST", StringComparison.OrdinalIgnoreCase) && request.Route.Equals("v1/catalogs/{catalogID}/categories/productassignments", StringComparison.OrdinalIgnoreCase))
            {
                var productResponse = await this._orderCloudService.GetProductAsync(this._orderCloudConfiguration, request.Request.Body.ProductId);
                if (!string.IsNullOrEmpty(productResponse.Product.Id))
                {
                    var document = await GenerateProductDocument(productResponse.Product, indexName, request.Request.Body.CategoryId);
                    await this._solrService.MergeOrAddDocumentsAsync(indexName, new List<ProductDocument> { document });
                }
            }
            else if (request.Verb.Equals("DELETE", StringComparison.OrdinalIgnoreCase) && request.Route.Equals("v1/catalogs/{catalogID}/categories/{categoryID}/productassignments/{productID}", StringComparison.OrdinalIgnoreCase))
            {
                var categoryProductAssignmentsResponse =
                    await this._orderCloudService.GetCategoryProductAssignmentsAsync(this._orderCloudConfiguration, this._orderCloudConfiguration.DefaultCatalogId, request.RouteParams.ProductId);
                if (categoryProductAssignmentsResponse.CategoryProductAssignments.All(x => x.CategoryId.Equals(request.RouteParams.CategoryId, StringComparison.OrdinalIgnoreCase)))
                {
                    await this._solrService.DeleteDocumentAsync(indexName, new ProductDocument { UniqueId = productId });
                }
            }

            return new JsonResult(new { IsSuccess = true });
        }

        private async Task<ProductDocument> GenerateProductDocument(Product product, string indexName, string? categoryId = null)
        {
            if (categoryId == null)
            {
                var categoryProductAssignmentsResponse = await this._orderCloudService.GetCategoryProductAssignmentsAsync(this._orderCloudConfiguration, this._orderCloudConfiguration.DefaultCatalogId, product.Id);
                var firstCategory = categoryProductAssignmentsResponse.CategoryProductAssignments.FirstOrDefault();
                categoryId = firstCategory?.CategoryId;
            }


            var productDocument = new ProductDocument
            {
                UniqueId = product.Id,
                Name = product.Id,
                DisplayName = product.Name,
                Language = "en",
                SxaContent = new List<string> { product.Id, product.Name, product.Description },
                IndexName = indexName,
                FullPath = $"shop/{categoryId}/{product.Id}"
            };

            return productDocument;
        }
    }
}
