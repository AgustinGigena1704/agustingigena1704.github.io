using Microsoft.AspNetCore.Components.Authorization;
using CCC.Services.Auth;
using System.Security.Claims;
using static CCC.Shared.Constants;
using CCC.Shared;
using Microsoft.AspNetCore.Components;
using CCC.Services;

namespace CCC.Services.Auth
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly IAuthService _authService;
        private readonly CookieService _cookieService;
        private readonly NavigationManager _navigationManager;

        private AuthenticationState _cachedAuthenticationState = new AuthenticationState(new ClaimsPrincipal());

        public AuthStateProvider(
            IAuthService authService, 
            CookieService cookieService, 
            NavigationManager navigationManager)
        {
            _authService = authService;
            _cookieService = cookieService;
            _navigationManager = navigationManager;

            _authService.AuthChanged = GetAuthenticationStateAsync;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var principal = new ClaimsPrincipal(); 
            string? currentPath = _navigationManager.Uri; 


            bool isAuthenticatedBefore = _cachedAuthenticationState.User.Identity?.IsAuthenticated ?? false;

            try
            {
                var user = await _authService.GetCurrentUserAsync();

                if (user != null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Uid),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                        new Claim(ClaimTypes.Thumbprint, user.PhotoUrl ?? string.Empty)
                    };

                    foreach (var provider in user.Providers ?? Enumerable.Empty<string>())
                    {
                        claims.Add(new Claim("ProviderId", provider));
                    }

                    var identity = new ClaimsIdentity(claims, "Firebase");
                    principal = new ClaimsPrincipal(identity);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await _authService.LogoutAsync();
                principal = new ClaimsPrincipal(); // Asegurar estado no autenticado
            }

            var newState = new AuthenticationState(principal);
            _cachedAuthenticationState = newState;

            // --- Lógica de redirección ---
            bool isAuthenticatedNow = newState.User.Identity?.IsAuthenticated ?? false;

            if (isAuthenticatedBefore && !isAuthenticatedNow)
            {
                // El usuario ha cerrado sesión
                if (!currentPath.Contains("/login", StringComparison.OrdinalIgnoreCase))
                {
                    _navigationManager.NavigateTo("/login");
                }
            }
            else if (!isAuthenticatedBefore && isAuthenticatedNow)
            {
                // El usuario se ha logueado
                if (currentPath.Contains("/login", StringComparison.OrdinalIgnoreCase))
                {
                    _navigationManager.NavigateTo("/home");
                }
            }
            

            return newState;
        }
    }
}