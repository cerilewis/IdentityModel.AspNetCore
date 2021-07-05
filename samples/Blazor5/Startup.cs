
namespace Blazor5
{
    using System;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Data;
    using System.IdentityModel.Tokens.Jwt;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.IdentityModel.Tokens;
    using Polly;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("cookie", options =>
                {
                    options.Cookie.Name = "blazorcode";

                    options.Events.OnSigningOut = async e =>
                    {
                        await e.HttpContext.RevokeUserRefreshTokenAsync();
                    };
                })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "https://demo.duendesoftware.com";

                    options.ClientId = "interactive.confidential.short";
                    options.ClientSecret = "secret";

                    // code flow + PKCE (PKCE is turned on by default)
                    options.ResponseType = "code";
                    options.UsePkce = true;

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("api");

                    // not mapped by default
                    options.ClaimActions.MapJsonKey("website", "website");

                    // keeps id_token smaller
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "name",
                        RoleClaimType = "role"
                    };
                });
            
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            // adds user and client access token management
            services.AddAccessTokenManagement(options =>
                {
                    // client config is inferred from OpenID Connect settings
                    // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
                    options.Client.DefaultClient.Scope = "api";
                })
                .ConfigureBackchannelHttpClient()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));
            
            // registers HTTP client that uses the managed user access token
            services.AddUserAccessTokenHttpClient("user_client", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
            });

            // registers HTTP client that uses the managed client access token
            services.AddClientAccessTokenHttpClient("client", configureClient: client =>
            {
                client.BaseAddress = new Uri("https://demo.duendesoftware.com/api/");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
