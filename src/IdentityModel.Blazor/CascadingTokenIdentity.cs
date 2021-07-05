namespace IdentityModel.Blazor
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;
    using AspNetCore.AccessTokenManagement;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Components.CompilerServices;
    using Microsoft.AspNetCore.Components.Rendering;

    public class CascadingTokenIdentity : ComponentBase
    {
        [Inject] private IUserAccessTokenStore UserAccessTokenStore { get; set; }
        [Inject] private IUserAccessTokenManagementService UserAccessTokenManagementService { get; set; }
        [Inject] private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<CascadingValue<IUserAccessTokenManagementService>>(0);
            builder.AddAttribute(1, "Value", RuntimeHelpers.TypeCheck(this.UserAccessTokenManagementService));
            builder.AddAttribute(2, "ChildContent", new RenderFragment(this.ChildContent));
            builder.CloseComponent();
            base.BuildRenderTree(builder);
        }
        
        [Parameter]
        public RenderFragment ChildContent { get; set; }
    
        [Parameter]
        public InitialAuthenticationState InitialAuthenticationState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await this.AuthenticationStateProvider.GetAuthenticationStateAsync();
            if ((authState.User?.Identity?.IsAuthenticated ?? false) && !string.IsNullOrEmpty(InitialAuthenticationState.AccessToken))
            {
                var token = new JwtSecurityToken(this.InitialAuthenticationState.AccessToken);

                await this.UserAccessTokenStore.StoreTokenAsync(authState.User, InitialAuthenticationState.AccessToken, token.ValidTo, InitialAuthenticationState.RefreshToken);
            }
        
            await base.OnInitializedAsync();
        }
    }
}