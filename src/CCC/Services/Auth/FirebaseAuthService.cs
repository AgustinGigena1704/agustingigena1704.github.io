using CCC.Services;
using CCC.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Sentry.Protocol;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using static CCC.Shared.Constants;

namespace CCC.Services.Auth
{
    public class FirebaseAuthService : IAuthService
    {
        private readonly IJSRuntime _js;
        private readonly HttpClient backEndApi;
        private DotNetObjectReference<FirebaseAuthService>? _dotNetRef;
        private readonly CookieService _cookieService;

        public Func<Task<AuthenticationState>> AuthChanged { get; set; }

        public FirebaseAuthService(IJSRuntime js, IHttpClientFactory clientFactory, CookieService cookieService)
        {
            backEndApi = clientFactory.CreateClient(BACK_END_API_NAME);
            _js = js;
            _dotNetRef = DotNetObjectReference.Create(this);
            _js.InvokeVoidAsync("registerAuthStateDotNetRef", _dotNetRef);
            _cookieService = cookieService;

            AuthChanged = () => Task.FromResult(new AuthenticationState(new ClaimsPrincipal()));
        }

        [JSInvokable]
        public async Task RegisterNewUser(string? Uid)
        {
            if(string.IsNullOrEmpty(Uid))
            {
                throw new ControledException("Ocurrio un erro al registrar el usuario en la Base de datos.");
            }
            var backendResponse = await backEndApi.GetAsync($"api/Auth/RegisterNewUser/{Uid}");
            if (!backendResponse.IsSuccessStatusCode)
            {
                throw new ControledException("Ocurrio un error al registrar el usuario en el sistema.");
            }
            return;
        }

        [JSInvokable]
        public async Task FireBaseOnAuthStateChanged(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                await _cookieService.SetCookie(FIREBASE_AUTH_TOKEN, token, DateTime.UtcNow.AddHours(12));
            }
            else
            {
                await _cookieService.DeleteCookie(FIREBASE_AUTH_TOKEN);
            }
            await AuthChanged.Invoke();
        }

        public async Task<SessionDTO> LoginAsync(LoginDTO loginDTO)
        {
            var json = (await _js.InvokeAsync<object>("firebaseLogin", loginDTO.Email, loginDTO.Password)).ToString();
            if (json == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion.");
            }
            var response = JsonSerializer.Deserialize<FireBaseAuthResponse>(json);

            if (response == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion.");
            }

            if (!response.Success || response.User == null || string.IsNullOrEmpty(response.IdToken))
            {
                throw new ControledException(ParseErrorMessage(response.ErrorMessage));
            }
            var provs = response.User?.ProviderData?.ToList();
            List<string> providers = new List<string>();
            if (provs != null)
            {
                foreach (ProviderData provider in provs)
                {
                    if (!string.IsNullOrEmpty(provider.ProviderId))
                    {
                        providers.Add(provider.ProviderId);
                    }
                }
            }
            var user = await GetCurrentUserByToken(response.IdToken);
            if (user == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion.");
            }
            if (user.Uid != response.User?.Uid)
            {
                throw new ControledException("El token no coincide con el usuario.");
            }

            DateTime ExpireTime = loginDTO.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddHours(12);

            await _cookieService.SetCookie(FIREBASE_AUTH_TOKEN, response.IdToken, ExpireTime);
            return new SessionDTO
            {
                UId = user.Uid,
                Email = user.Email,
                Providers = providers,
                IdToken = response.IdToken
            };

            throw new ControledException("Ocurrio un error al iniciar sesion.");
        }

        public async Task<SessionDTO> LoginWithGoogleAsync()
        {
            var objeto = (await _js.InvokeAsync<object>("signInWithGoogle")).ToString();
            if (objeto == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion con Google.");
            }
            var respuesta = JsonSerializer.Deserialize<FireBaseAuthResponse>(objeto);
            if (respuesta == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion con Google.");
            }
            if (!respuesta.Success || respuesta.User == null || string.IsNullOrEmpty(respuesta.IdToken))
            {
                throw new ControledException(ParseErrorMessage(respuesta.ErrorMessage));
            }
            var provs = respuesta.User?.ProviderData?.ToList();
            List<string> providers = new List<string>();
            if (provs != null)
            {
                foreach (ProviderData provider in provs)
                {
                    if (!string.IsNullOrEmpty(provider.ProviderId))
                    {
                        providers.Add(provider.ProviderId);
                    }
                }
            }
            var user = await GetCurrentUserByToken(respuesta.IdToken);
            if (user == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion con Google.");
            }
            if (user.Uid != respuesta.User?.Uid || respuesta.IdToken == null)
            {
                throw new ControledException("El token de Google no coincide con el usuario.");
            }
            await _cookieService.SetCookie(FIREBASE_AUTH_TOKEN, respuesta.IdToken, DateTime.UtcNow.AddDays(30));
            return new SessionDTO
            {
                UId = user.Uid,
                Email = user.Email,
                Providers = providers,
                IdToken = respuesta.IdToken
            };
        }

        public async Task<SessionDTO> RegisterAsync(LoginDTO loginDTO)
        {
            var json = (await _js.InvokeAsync<object>("firebaseRegister", loginDTO.Email, loginDTO.Password)).ToString();
            if (json == null)
            {
                throw new ControledException("Error inesperado al registrarse.");
            }
            var response = JsonSerializer.Deserialize<FireBaseAuthResponse>(json);

            if (response == null)
            {
                throw new ControledException("Ocurrio un error al registrarse.");
            }

            if (!response.Success || response.User == null || response.IdToken == null)
            {
                throw new ControledException(ParseErrorMessage(response.ErrorMessage));
            }
            var provs = response.User?.ProviderData?.ToList();
            List<string> providers = new List<string>();
            if (provs != null)
            {
                foreach (ProviderData provider in provs)
                {
                    if (!string.IsNullOrEmpty(provider.ProviderId))
                    {
                        providers.Add(provider.ProviderId);
                    }
                }
            }
            var user = await GetCurrentUserByToken(response.IdToken);
            if (user == null)
            {
                throw new ControledException("Ocurrio un error al iniciar sesion con Google.");
            }
            if (user.Uid != response.User?.Uid || response.IdToken == null)
            {
                throw new ControledException("El token de Google no coincide con el usuario.");
            }

            return new SessionDTO
            {
                UId = user.Uid,
                Email = user.Email,
                Providers = providers,
                IdToken = response.IdToken
            };
        }

        public async Task<UsuarioDTO?> GetCurrentUserAsync()
        {
            var cookie = await _cookieService.GetCookie(FIREBASE_AUTH_TOKEN);
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            var response = await GetCurrentUserByToken(cookie);

            if (response == null) throw new ControledException("Ocurrio un error al obtener el usuario actual.");

            return response;
        }

        public async Task LogoutAsync()
        {
            await _cookieService.DeleteCookie(FIREBASE_AUTH_TOKEN);
            await _js.InvokeVoidAsync("firebaseLogout");
        }

        public async Task<bool>VerifiExistingEmail(string email)
        {
            try
            {
                var result = await backEndApi.GetAsync($"api/Auth/EmailAccountExists?{email}");
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception(result.ReasonPhrase);
                }
                var response = await result.Content.ReadAsStringAsync();
                if (response != "true")
                {
                    return false;
                }
                return true;
                
            }
            catch(Exception e)
            {
                SentrySdk.CaptureException(e);
                return false;
            }
        }

        private string ParseErrorMessage(string? message)
        {
            string respuesta = "Ocurrio un error inesperrado.";
            switch (message)
            {
                case "Firebase: Error (auth/invalid-email).":
                    respuesta = "El email ingresado no se encuenta registrado en el sistema.";
                    break;
                case "Firebase: Error (auth/invalid-credential).":
                    respuesta = "La contraseña ingresada es incorrecta.";
                    break;
                case "Firebase: Error (auth/email-already-in-use).":
                    respuesta = "Ya existe una cuenta con el email ingresado.";
                    break;
                default:
                    respuesta = "Ocurrio un error inesperrado.";
                    break;
            }

            return respuesta;
        }

        public async Task<UsuarioDTO?> GetCurrentUserByToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new ControledException("Token inválido.");
                }
                var encoded = Uri.EscapeDataString(token);
                var response = await backEndApi.GetAsync($"api/Auth/GetUserByToken/{encoded}");
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                var decoded = await response.Content.ReadFromJsonAsync<AuthValidationDTO>();
                return decoded?.User;
            }
            catch(Exception e)
            {
                SentrySdk.CaptureException(e);
                return null;
            }
        }

        public async Task<UsuarioDTO?> LinkPasswordAccount(string email, string password)
        {
            var json = (await _js.InvokeAsync<object>("linkPasswordAccount", email, password)).ToString();
            var response = JsonSerializer.Deserialize<FireBaseAuthResponse>(json ?? "");
            if (response == null || !response.Success)
            {
                throw new ControledException("Error inesperado al vincular la cuenta.");
            }
            var provs = response.User?.ProviderData?.ToList();
            List<string> providers = new List<string>();
            if (provs != null)
            {
                foreach (ProviderData provider in provs)
                {
                    if (!string.IsNullOrEmpty(provider.ProviderId))
                    {
                        providers.Add(provider.ProviderId);
                    }
                }
            }
            var user = await GetCurrentUserByToken(response.IdToken ?? "");
            if (user == null)
            {
                throw new ControledException("Ocurrio un error al vincular la cuenta.");
            }
            if (user.Uid != response.User?.Uid || response.IdToken == null)
            {
                throw new ControledException("El token no coincide con el usuario.");
            }
            return user;
        }

        public async Task<UsuarioDTO?> LinkGoogleAccount()
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
                throw new ControledException("Para vincular una cuenta de Google, primero debes iniciar sesión.");
            var json = (await _js.InvokeAsync<object>("linkGoogleAccount")).ToString();
            var response = JsonSerializer.Deserialize<FireBaseAuthResponse>(json ?? "");
            if (response == null || !response.Success)
            {
                throw new ControledException("Error inesperado al vincular la cuenta.");
            }
            var provs = response.User?.ProviderData?.ToList();
            List<string> providers = new List<string>();
            if (provs != null)
            {
                foreach (ProviderData provider in provs)
                {
                    if (!string.IsNullOrEmpty(provider.ProviderId))
                    {
                        providers.Add(provider.ProviderId);
                    }
                }
            }
            var user = await GetCurrentUserByToken(response.IdToken ?? "");
            if (user == null)
            {
                throw new ControledException("Ocurrio un error al vincular la cuenta.");
            }
            if (user.Uid != response.User?.Uid || response.IdToken == null)
            {
                throw new ControledException("El token no coincide con el usuario.");
            }
            return user;

        }
    }

    class FireBaseAuthResponse
    {
        [JsonPropertyName("user")]
        public FireBaseUser? User { get; set; }
        [JsonPropertyName("token")]
        public string? IdToken { get; set; }
        [JsonPropertyName("error")]
        public string? ErrorMessage { get; set; }
        public bool Success
        {
            get => ErrorMessage == null && User != null && IdToken != null;
        }

    }

    public class FireBaseUser
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; } = string.Empty;
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }
        [JsonPropertyName("isAnonymous")]
        public bool IsAnonymous { get; set; }
        [JsonPropertyName("providerData")]
        public List<ProviderData>? ProviderData { get; set; }
        [JsonPropertyName("stsTokenManager")]
        public StsTokenManager? StsTokenManager { get; set; }
        [JsonPropertyName("createdAt")]
        public string? CreatedAt { get; set; }
        [JsonPropertyName("lastLoginAt")]
        public string? LastLoginAt { get; set; }
        [JsonPropertyName("apiKey")]
        public string? ApiKey { get; set; }
        [JsonPropertyName("appName")]
        public string? AppName { get; set; }
    }

    public class ProviderData
    {
        [JsonPropertyName("providerId")]
        public string? ProviderId { get; set; }
        [JsonPropertyName("uid")]
        public string? Uid { get; set; }
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("phoneNumber")]
        public string? PhoneNumber { get; set; }
        [JsonPropertyName("photoURL")]
        public string? PhotoURL { get; set; }
    }

    public class StsTokenManager
    {
        [JsonPropertyName("accessToken")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("expirationTime")]
        public long ExpirationTime { get; set; }
    }
}
