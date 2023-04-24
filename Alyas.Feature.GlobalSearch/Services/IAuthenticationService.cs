using Alyas.Feature.GlobalSearch.Configuration;
using Alyas.Feature.GlobalSearch.Models;

namespace Alyas.Feature.GlobalSearch.Services
{
    public interface IAuthenticationService
    {
        Task<AuthResponseModel> GetToken(OrderCloudConfiguration config);
    }
}
