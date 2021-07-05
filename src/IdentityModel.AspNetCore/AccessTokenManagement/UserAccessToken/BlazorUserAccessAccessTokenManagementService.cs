namespace IdentityModel.AspNetCore.AccessTokenManagement
{
    using System;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using Client;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements basic token management logic
    /// </summary>
    // public class BlazorUserAccessAccessTokenManagementService : IUserAccessTokenManagementService
    // {
    //     private readonly IUserAccessTokenRequestSynchronization _sync;
    //     private readonly IUserAccessTokenStore _userAccessTokenStore;
    //     private readonly ISystemClock _clock;
    //     private readonly UserAccessTokenManagementOptions _options;
    //     private readonly ITokenEndpointService _tokenEndpointService;
    //     private readonly ILogger<UserAccessAccessTokenManagementService> _logger;
    //
    //     /// <summary>
    //     /// ctor
    //     /// </summary>
    //     /// <param name="sync"></param>
    //     /// <param name="userAccessTokenStore"></param>
    //     /// <param name="clock"></param>
    //     /// <param name="options"></param>
    //     /// <param name="tokenEndpointService"></param>
    //     /// <param name="logger"></param>
    //     public BlazorUserAccessAccessTokenManagementService(
    //         IUserAccessTokenRequestSynchronization sync,
    //         IUserAccessTokenStore userAccessTokenStore,
    //         ISystemClock clock,
    //         UserAccessTokenManagementOptions options,
    //         ITokenEndpointService tokenEndpointService,
    //         ILogger<UserAccessAccessTokenManagementService> logger)
    //     {
    //         this._sync = sync;
    //         this._userAccessTokenStore = userAccessTokenStore;
    //         this._clock = clock;
    //         this._options = options;
    //         this._tokenEndpointService = tokenEndpointService;
    //         this._logger = logger;
    //     }
    //     
    //     /// <inheritdoc/>
    //     public async Task<string> GetUserAccessTokenAsync(
    //         ClaimsPrincipal user, 
    //         UserAccessTokenParameters parameters = null, 
    //         CancellationToken cancellationToken = default)
    //     {
    //         parameters ??= new UserAccessTokenParameters();
    //         
    //         var userToken = await this._userAccessTokenStore.GetTokenAsync(null, parameters);
    //
    //         if (userToken == null)
    //         {
    //             this._logger.LogDebug("No token data found in user token store");
    //             return null;
    //         }
    //         
    //         if (userToken.AccessToken.IsPresent() && userToken.RefreshToken.IsMissing())
    //         {
    //             this._logger.LogDebug("No refresh token found in user token store for  resource {resource}. Returning current access token.", parameters.Resource ?? "default");
    //             return userToken.AccessToken;
    //         }
    //
    //         if (userToken.AccessToken.IsMissing() && userToken.RefreshToken.IsPresent())
    //         {
    //             this._logger.LogDebug(
    //                 "No access token found in user token store for resource {resource}. Trying to refresh.",
    //                  parameters.Resource ?? "default");
    //         }
    //
    //         var dtRefresh = DateTimeOffset.MinValue;
    //         if (userToken.Expiration.HasValue)
    //         {
    //             dtRefresh = userToken.Expiration.Value.Subtract(this._options.RefreshBeforeExpiration);
    //         }
    //         
    //         if (dtRefresh < this._clock.UtcNow || parameters.ForceRenewal)
    //         {
    //             this._logger.LogDebug("Token for user needs refreshing.");
    //
    //             try
    //             {
    //                 return await this._sync.Dictionary.GetOrAdd(userToken.RefreshToken, _ =>
    //                 {
    //                     return new Lazy<Task<string>>(async () =>
    //                     {
    //                         var refreshed = await this.RefreshUserAccessTokenAsync(user, parameters, cancellationToken);
    //                         return refreshed.AccessToken;
    //                     });
    //                 }).Value;
    //             }
    //             finally
    //             {
    //                 this._sync.Dictionary.TryRemove(userToken.RefreshToken, out _);
    //             }
    //         }
    //
    //         return userToken.AccessToken;
    //     }
    //
    //     /// <inheritdoc/>
    //     public async Task RevokeRefreshTokenAsync(
    //         ClaimsPrincipal user, 
    //         UserAccessTokenParameters parameters = null, 
    //         CancellationToken cancellationToken = default)
    //     {
    //         parameters ??= new UserAccessTokenParameters();
    //         var userToken = await this._userAccessTokenStore.GetTokenAsync(user, parameters);
    //
    //         if (!string.IsNullOrEmpty(userToken?.RefreshToken))
    //         {
    //             var response = await this._tokenEndpointService.RevokeRefreshTokenAsync(userToken.RefreshToken, parameters, cancellationToken);
    //
    //             if (response.IsError)
    //             {
    //                 this._logger.LogError("Error revoking refresh token. Error = {error}", response.Error);
    //             }
    //         }
    //     }
    //
    //     private async Task<TokenResponse> RefreshUserAccessTokenAsync(ClaimsPrincipal user, UserAccessTokenParameters parameters, CancellationToken cancellationToken = default)
    //     {
    //         var userToken = await this._userAccessTokenStore.GetTokenAsync(user, parameters);
    //         var response = await this._tokenEndpointService.RefreshUserAccessTokenAsync(userToken.RefreshToken, parameters, cancellationToken);
    //
    //         if (!response.IsError)
    //         {
    //             var expiration = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
    //
    //             await this._userAccessTokenStore.StoreTokenAsync(user, response.AccessToken, expiration, response.RefreshToken, parameters);
    //         }
    //         else
    //         {
    //             this._logger.LogError("Error refreshing access token. Error = {error}", response.Error);
    //         }
    //
    //         return response;
    //     }
    // }
}