// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using AspNetCore.Authentication;
    using Extensions;
    using global::IdentityModel.AspNetCore;
    using global::IdentityModel.AspNetCore.AccessTokenManagement;

    public static class BlazorTokenManagementServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the token management services to DI
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureAction"></param>
        /// <returns></returns>
        public static TokenManagementBuilder AddBlazorAccessTokenManagement(this IServiceCollection services,
            Action<AccessTokenManagementOptions> configureAction = null)
        {
            CheckConfigMarker(services);
            
            var options = new AccessTokenManagementOptions();
            configureAction?.Invoke(options);
            
            services.AddSingleton(options.Client);
            services.AddSingleton(options.User);

            services.AddBlazorUserAccessTokenManagementInternal();
            services.AddClientAccessTokenManagementInternal();
            
            return new TokenManagementBuilder(services);
        }
                
        private static TokenManagementBuilder AddBlazorUserAccessTokenManagementInternal(this IServiceCollection services)
        {
            // necessary ASP.NET plumbing
            services.AddAuthentication();
            
            services.AddSharedServices();
            
            services.TryAddScoped<IUserAccessTokenManagementService, UserAccessAccessTokenManagementService>();
            services.TryAddScoped<IUserAccessTokenStore, ScopedUserTokenStore>();
            services.TryAddScoped<IUserAccessTokenRequestSynchronization, AccessTokenRequestSynchronization>();
            
            return new TokenManagementBuilder(services);
        }
                
        private static TokenManagementBuilder AddClientAccessTokenManagementInternal(this IServiceCollection services)
        {
            // necessary ASP.NET plumbing
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<ISystemClock, SystemClock>();
            services.TryAddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>();
            
            services.AddSharedServices();
            
            services.TryAddTransient<IClientAccessTokenManagementService, ClientAccessTokenManagementService>();
            services.TryAddTransient<IClientAccessTokenCache, ClientAccessTokenCache>();
            services.TryAddSingleton<IClientAccessTokenRequestSynchronization, AccessTokenRequestSynchronization>();
            
            return new TokenManagementBuilder(services);
        }

        private static void AddSharedServices(this IServiceCollection services)
        {
            services.TryAddTransient<ITokenClientConfigurationService, DefaultTokenClientConfigurationService>();
            services.TryAddTransient<ITokenEndpointService, TokenEndpointService>();
            
            services.AddHttpClient(AccessTokenManagementDefaults.BackChannelHttpClientName);
        }

        private static void CheckConfigMarker(IServiceCollection services)
        {
            var marker = services.FirstOrDefault(s => s.ServiceType == typeof(ConfigMarker));
            if (marker == null)
            {
                services.AddSingleton(new ConfigMarker());
                return;
            }

            throw new InvalidOperationException(
                "Call 'AddAccessTokenManagement' to add support for both client and user access tokens. Or call 'AddUserAccessTokenManagement' or 'AddClientAccessTokenManagement' respectively. You cannot mix them. Nor can you call them multiple times.");
        }

        private class ConfigMarker
        {
        }
    }
}