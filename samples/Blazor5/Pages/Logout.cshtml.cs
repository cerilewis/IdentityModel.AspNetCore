namespace Blazor5.Pages
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await this.HttpContext.SignOutAsync();

            return this.Redirect("/");
        }
    }
}