using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GestWeb.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _user;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = _user?.Identity ?? new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            return Task.FromResult(new AuthenticationState(user));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsAuthenticated(string token)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, token)
            }, "apiauth_type");

            _user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged();
        }

        public void MarkUserAsLoggedOut()
        {
            _user = null;
            NotifyAuthenticationStateChanged();
        }
    }
}
