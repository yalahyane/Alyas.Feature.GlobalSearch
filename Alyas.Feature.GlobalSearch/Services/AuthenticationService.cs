using System.Text.Json;
using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;
using RestSharp;

namespace Alyas.Feature.GlobalSearch.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public async Task<AuthResponseModel> GetToken(OrderCloudConfiguration config)
        {
            var authResponseModel = new AuthResponseModel();
            using var client = new RestClient(config.BaseUrl);
            var request = new RestRequest("oauth/token", Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.AddParameter("client_id", config.ApiClientId);
            request.AddParameter("client_secret", config.ApiClientSecret);
            request.AddParameter("scope", "FullAccess");
            var result = await client.ExecuteAsync(request);
            if (!result.IsSuccessful || result.Content == null)
            {
                authResponseModel.IsSuccess = false;
                authResponseModel.ErrorMessage = $"Failed to Get Token: {result.ErrorMessage}";
                return authResponseModel;
            }

            authResponseModel = JsonSerializer.Deserialize<AuthResponseModel>(result.Content);
            if (authResponseModel == null)
            {
                return new AuthResponseModel
                {
                    IsSuccess = false,
                    ErrorMessage = $"Failed to deserialize Auth Response: {result.Content}"
                };
            }

            authResponseModel.IsSuccess = true;

            return authResponseModel;
        }
    }
}
