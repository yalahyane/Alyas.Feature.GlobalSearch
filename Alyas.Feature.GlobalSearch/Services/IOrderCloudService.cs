using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;

namespace Alyas.Feature.GlobalSearch.Services
{
    public interface IOrderCloudService
    {
        Task<GetProductsResponseModel> GetAllProductsAsync(OrderCloudConfiguration config, int pageNumber, int pageSize);
        Task<GetProductResponseModel> GetProductAsync(OrderCloudConfiguration config, string productId);
        Task<GetCategoryProductAssignmentsResponseModel> GetCategoryProductAssignmentsAsync(OrderCloudConfiguration config, string catalogId, string productId);
    }
}
