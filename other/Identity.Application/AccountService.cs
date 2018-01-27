using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Http;

namespace Identity.Application
{
    public class AccountService
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(
            IIdentityServerInteractionService interaction,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _interaction = interaction;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<LogoutInfo> BuildLogoutInfoAsync(string logoutId)
        {
            var vm = new LogoutInfo { LogoutId = logoutId, ShowLogoutPrompt = true };

            var user = _httpContextAccessor.HttpContext.User;
            if (user?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        public async Task<LoggedOutInfo> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutInfo
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                LogoutId = logoutId
            };

            return vm;
        }

        public class LoggedOutInfo
        {
            public string PostLogoutRedirectUri { get; set; }
            public string ClientName { get; set; }
            public string LogoutId { get; set; }
        }

        public class LogoutInfo
        {
            public string LogoutId { get; set; }
            public bool ShowLogoutPrompt { get; set; }
        }

    }
}
