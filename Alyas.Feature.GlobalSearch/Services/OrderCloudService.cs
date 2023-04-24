using System.Text.Json;
using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;
using RestSharp;

namespace Alyas.Feature.GlobalSearch.Services
{
    public class OrderCloudService : IOrderCloudService
    {
        private readonly IAuthenticationService _authenticationService;
        public OrderCloudService(IAuthenticationService authenticationService)
        {
            this._authenticationService = authenticationService;
        }
        public async Task<GetProductsResponseModel> GetAllProductsAsync(OrderCloudConfiguration config, int pageNumber, int pageSize)
        {
            var response = new GetProductsResponseModel();
            var tokenResponse = await this._authenticationService.GetToken(config);
            if (tokenResponse.IsSuccess)
            {
                using var client = new RestClient($"{config.BaseUrl}/{config.ApiVersion}");
                var request = new RestRequest($"products?pageSize={pageSize}&page={pageNumber}&sortBy=ID");
                request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

                var ocResponse = await client.ExecuteAsync(request);
                if (!ocResponse.IsSuccessful || ocResponse?.Content == null)
                {
                    return response;
                }
                response = JsonSerializer.Deserialize<GetProductsResponseModel>(ocResponse.Content);
            }
            return response;
        }

        public async Task<GetProductResponseModel> GetProductAsync(OrderCloudConfiguration config, string productId)
        {
            var response = new GetProductResponseModel();
            var tokenResponse = await this._authenticationService.GetToken(config);
            if (tokenResponse.IsSuccess)
            {
                using var client = new RestClient($"{config.BaseUrl}/{config.ApiVersion}");
                var request = new RestRequest($"products/{productId}");
                request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

                var ocResponse = await client.ExecuteAsync(request);
                if (!ocResponse.IsSuccessful || ocResponse?.Content == null)
                {
                    return response;
                }
                response = JsonSerializer.Deserialize<GetProductResponseModel>(ocResponse.Content);
            }
            return response;
        }

        public async Task<GetCategoryProductAssignmentsResponseModel> GetCategoryProductAssignmentsAsync(OrderCloudConfiguration config, string catalogId, string productId)
        {
            var response = new GetCategoryProductAssignmentsResponseModel();
            var tokenResponse = await this._authenticationService.GetToken(config);
            if (tokenResponse.IsSuccess)
            {
                using var client = new RestClient($"{config.BaseUrl}/{config.ApiVersion}");
                var request = new RestRequest($"catalogs/{catalogId}/categories/productassignments?productID={productId}");
                request.AddParameter("Authorization", string.Format("Bearer " + tokenResponse.AccessToken), ParameterType.HttpHeader);

                var ocResponse = await client.ExecuteAsync(request);
                if (!ocResponse.IsSuccessful || ocResponse?.Content == null)
                {
                    return response;
                }
                response = JsonSerializer.Deserialize<GetCategoryProductAssignmentsResponseModel>(ocResponse.Content);
            }
            return response;
        }
    }
}
