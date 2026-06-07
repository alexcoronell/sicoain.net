using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using sicoain.client.Abstractions;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Providers
{
    public class CustomAuthenticationStateProvider: AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
        }

        // We are trying to get the current user from the backend
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                UserDto userDto = await _authService.GetCurrentUserAsync();
                if(userDto != null && !string.IsNullOrEmpty(userDto.Email))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, userDto.Id.ToString()),
                        new Claim(ClaimTypes.Name, userDto.FullName ?? string.Empty),
                        new Claim(ClaimTypes.Email, userDto.Email)
                    };

                    var identity = new ClaimsIdentity(claims, "apiauth");
                    _currentUser = new ClaimsPrincipal(identity);
                }
                else
                {
                    _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
                }
            }
            catch(UnauthorizedAccessException)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            catch(Exception)
            {
                _currentUser = new(new ClaimsIdentity());
            }
            return new AuthenticationState(_currentUser);
        }

        // Method to notify that the user has logged in
        public void MarkUserAsAuthenticated(ClaimsPrincipal user)
        {
            _currentUser = user;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // Method to notify that the user has logged out
        public void MarkUserAsLoggedOut()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
