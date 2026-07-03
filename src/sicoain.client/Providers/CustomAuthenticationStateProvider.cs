using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using sicoain.client.Abstractions;
using sicoain.shared.DTOs.Users;

namespace sicoain.client.Providers
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string AuthFlagKey = "sicoain_auth";
        private readonly IAuthService _authService;
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private bool _hasCheckedLocal;

        public CustomAuthenticationStateProvider(IAuthService authService, IJSRuntime jsRuntime)
        {
            _authService = authService;
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Fast path: check localStorage flag to skip HTTP calls when logged out
            if (!_hasCheckedLocal)
            {
                _hasCheckedLocal = true;
                try
                {
                    var hasFlag = await _jsRuntime.InvokeAsync<bool>("eval",
                        $"localStorage.getItem('{AuthFlagKey}') === 'true'");
                    if (!hasFlag)
                    {
                        _currentUser = new(new ClaimsIdentity());
                        return new AuthenticationState(_currentUser);
                    }
                }
                catch
                {
                    // eval failed — proceed to HTTP check
                }
            }

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
                await SetAuthFlagAsync(false);
                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
            catch (Exception)
            {
                _currentUser = new(new ClaimsIdentity());
                return new AuthenticationState(_currentUser);
            }
        }

        private async Task SetAuthFlagAsync(bool loggedIn)
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("eval",
                    $"localStorage.setItem('{AuthFlagKey}', '{loggedIn.ToString().ToLower()}')");
            }
            catch
            {
                // Silently ignore — localStorage not available
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
        public async void MarkUserAsAuthenticated(ClaimsPrincipal user)
        {
            await SetAuthFlagAsync(true);
            _currentUser = user;
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        // Method to notify that the user has logged out
        public async void MarkUserAsLoggedOut()
        {
            await SetAuthFlagAsync(false);
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
