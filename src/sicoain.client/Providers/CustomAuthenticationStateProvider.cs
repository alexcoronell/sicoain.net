using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using sicoain.client.Abstractions;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IAuthService authService)
        {
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // Try to get the current user with the existing access token
                UserDto userDto = await _authService.GetCurrentUserAsync();
                if (userDto != null && !string.IsNullOrEmpty(userDto.Email))
                {
                    SetAuthenticatedUser(userDto);
                    return new AuthenticationState(_currentUser);
                }

                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
            catch (UnauthorizedAccessException)
            {
                // Access token expired — try to refresh before giving up
                return await TryRefreshAndGetUserAsync();
            }
            catch (Exception)
            {
                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
        }

        private async Task<AuthenticationState> TryRefreshAndGetUserAsync()
        {
            try
            {
                var refreshed = await _authService.RefreshTokenAsync();
                if (!refreshed)
                {
                    _currentUser = new(new ClaimsIdentity());
                    return new AuthenticationState(_currentUser);
                }

                // Refresh succeeded, now retry getting the user
                UserDto userDto = await _authService.GetCurrentUserAsync();
                if (userDto != null && !string.IsNullOrEmpty(userDto.Email))
                {
                    SetAuthenticatedUser(userDto);
                    return new AuthenticationState(_currentUser);
                }

                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
            catch (UnauthorizedAccessException)
            {
                // Refresh token also expired or invalid — real logout
                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
            catch (Exception)
            {
                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
        }

        private void SetAuthenticatedUser(UserDto userDto)
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
