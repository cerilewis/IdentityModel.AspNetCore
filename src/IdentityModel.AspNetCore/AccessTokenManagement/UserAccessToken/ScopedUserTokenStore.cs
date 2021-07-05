namespace IdentityModel.AspNetCore.AccessTokenManagement
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    public class ScopedUserTokenStore : IUserAccessTokenStore
    {
        private readonly ILogger<ScopedUserTokenStore> _logger;
        private string _accessToken;
        private DateTimeOffset _expiration;
        private string _refreshToken;

        public ScopedUserTokenStore(ILogger<ScopedUserTokenStore> logger)
        {
            this._logger = logger;
            this._logger.LogDebug($"{nameof(ScopedUserTokenStore)} ctor");
        }

        public Task StoreTokenAsync(
            ClaimsPrincipal user,
            string accessToken,
            DateTimeOffset expiration,
            string refreshToken = null,
            UserAccessTokenParameters parameters = null)
        {
            this._accessToken = accessToken;
            this._expiration = expiration;
            this._refreshToken = refreshToken;
            
            return Task.CompletedTask;
        }

        public Task<UserAccessToken> GetTokenAsync(ClaimsPrincipal user, UserAccessTokenParameters parameters = null)
        {
            return !string.IsNullOrEmpty(this._accessToken)
                ? Task.FromResult(new UserAccessToken
                {
                    AccessToken = this._accessToken,
                    Expiration = this._expiration,
                    RefreshToken = this._refreshToken
                })
                : Task.FromResult<UserAccessToken>(null);
        }

        public Task ClearTokenAsync(ClaimsPrincipal user, UserAccessTokenParameters parameters = null)
        {
            this._accessToken = null;
            this._refreshToken = null;
            
            return Task.CompletedTask;
        }
    }
}