using CCC.Shared;
using CCC.Services.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection.Metadata;

namespace CCC.Services
{
    public interface IAuthService
    {
        public Task<SessionDTO> LoginAsync(LoginDTO loginDTO);
        public Task<SessionDTO> LoginWithGoogleAsync();
        public Task<SessionDTO> RegisterAsync(LoginDTO loginDTO);
        public Task<UsuarioDTO?> GetCurrentUserAsync();
        public Task<UsuarioDTO?> GetCurrentUserByToken(string token);
        public Task<UsuarioDTO?> LinkPasswordAccount(string email, string password);
        public Task<UsuarioDTO?> LinkGoogleAccount();
        public Task<bool> VerifiExistingEmail(string email);
        public Task LogoutAsync();
        public Func<Task<AuthenticationState>> AuthChanged { get; set; }
    }
}
