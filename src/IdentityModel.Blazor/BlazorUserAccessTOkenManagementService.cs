namespace IdentityModel.Blazor
{
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using AspNetCore.AccessTokenManagement;

    public class BlazorUserAccessTokenManagementService : IUserAccessTokenManagementService
    {
        public Task<string> GetUserAccessTokenAsync(
            ClaimsPrincipal user,
            UserAccessTokenParameters parameters = null,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task RevokeRefreshTokenAsync(
            ClaimsPrincipal user,
            UserAccessTokenParameters parameters = null,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}