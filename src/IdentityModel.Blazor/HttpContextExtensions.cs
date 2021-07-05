namespace IdentityModel.Blazor
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;

    public static class HttpContextExtensions
    {
        public static async Task<InitialAuthenticationState> GetInitialAuthenticationState(this HttpContext context)
        {
            var accessToken = await context.GetTokenAsync("access_token");
            var refreshToken = await context.GetTokenAsync("refresh_token");

            return new InitialAuthenticationState(accessToken, refreshToken);
        }
    }
}