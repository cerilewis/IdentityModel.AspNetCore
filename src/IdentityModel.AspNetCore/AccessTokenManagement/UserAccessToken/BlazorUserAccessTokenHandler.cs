namespace IdentityModel.AspNetCore.AccessTokenManagement
{
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Delegating handler that injects the current access token into an outgoing request
    /// </summary>
    public class BlazorUserAccessTokenHandler : DelegatingHandler
    {
        private readonly IUserAccessTokenManagementService _userAccessTokenManagementService;
        private readonly UserAccessTokenParameters _parameters;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="userAccessTokenManagementService"></param>
        /// <param name="parameters"></param>
        public BlazorUserAccessTokenHandler(IUserAccessTokenManagementService userAccessTokenManagementService, UserAccessTokenParameters parameters = null)
        {
            this._userAccessTokenManagementService = userAccessTokenManagementService;
            this._parameters ??= new UserAccessTokenParameters();
        }

        /// <inheritdoc/>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await this.SetTokenAsync(request, forceRenewal: false);
            var response = await base.SendAsync(request, cancellationToken);

            // retry if 401
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                response.Dispose();

                await this.SetTokenAsync(request, forceRenewal: true);
                return await base.SendAsync(request, cancellationToken);
            }

            return response;
        }

        /// <summary>
        /// Set an access token on the HTTP request
        /// </summary>
        /// <param name="request"></param>
        /// <param name="forceRenewal"></param>
        /// <returns></returns>
        protected virtual async Task SetTokenAsync(HttpRequestMessage request, bool forceRenewal)
        {
            var parameters = new UserAccessTokenParameters
            {
                SignInScheme = this._parameters.SignInScheme,
                ChallengeScheme = this._parameters.ChallengeScheme,
                Resource = this._parameters.Resource,
                ForceRenewal = forceRenewal,
                Context =  this._parameters.Context
            };
              
            var token = await this._userAccessTokenManagementService.GetUserAccessTokenAsync(null, parameters);

            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}