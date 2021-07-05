namespace Blazor5.Pages
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class LoginModel : PageModel
    {
        public async Task OnGet(string redirectUri)
        {
            var localRedirectUrl =
                string.IsNullOrEmpty(redirectUri) ||
                string.Equals("login", redirectUri, StringComparison.InvariantCultureIgnoreCase)
                    ? "/"
                    : redirectUri;
            
            await this.HttpContext.ChallengeAsync("oidc", new AuthenticationProperties {RedirectUri = localRedirectUrl});
        }
    }
}