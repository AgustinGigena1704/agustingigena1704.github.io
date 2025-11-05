using CCC.Shared;

namespace CCC.Api.Interfaces
{
    public interface IAuthService
    {
        public Task<UsuarioDTO?> GetUserByToken(string token);
        public Task<UsuarioDTO?> GetUserByUid(string uid);
        public Task<UsuarioDTO?> GetCurrentUser();
    }
}
