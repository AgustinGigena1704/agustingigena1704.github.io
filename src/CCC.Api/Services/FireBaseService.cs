using CCC.Api.Interfaces;
using CCC.Shared;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace CCC.Api.Services
{
    public class FireBaseService : IAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FireBaseService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UsuarioDTO?> GetUserByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException("El token no puede estar vacio.");
            }
            FirebaseToken firebaseToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            if (firebaseToken == null)
            {
                throw new ArgumentException("El token no es valido.");
            }
            UserRecord fireBaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(firebaseToken.Uid);
            if (fireBaseUser == null)
            {
                throw new ArgumentException("El usuario no es valido.");
            }
            List<string> providers = new List<string>();
            foreach (var provider in fireBaseUser.ProviderData)
            {
                if(!string.IsNullOrEmpty(provider.ProviderId))
                {
                    providers.Add(provider.ProviderId);
                }
            }
            UsuarioDTO auth = new UsuarioDTO
            {
                Uid = fireBaseUser.Uid,
                Email = fireBaseUser.Email,
                Name = fireBaseUser.DisplayName,
                PhotoUrl = fireBaseUser.PhotoUrl,
                Providers = providers
            };
            return auth;
        }

        public async Task<UsuarioDTO?> GetUserByUid(string uid)
        {
            if (string.IsNullOrEmpty(uid))
            {
                throw new ArgumentException("El uid no puede estar vacio.");
            }
            UserRecord fireBaseUser = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
            if (fireBaseUser == null)
            {
                throw new ArgumentException("El usuario no es valido.");
            }
            List<string> providers = new List<string>();
            foreach (var provider in fireBaseUser.ProviderData)
            {
                if (!string.IsNullOrEmpty(provider.ProviderId))
                {
                    providers.Add(provider.ProviderId);
                }
            }
            return new UsuarioDTO
            {
                Uid = fireBaseUser.Uid,
                Email = fireBaseUser.Email,
                Name = fireBaseUser.DisplayName,
                PhotoUrl = fireBaseUser.PhotoUrl,
                Providers = providers
            };
        }
            
        public async Task<UserRecord?> GetUserRecordByUId(string Uid)
        {
            if (string.IsNullOrEmpty(Uid))
            {
                throw new ArgumentException("El uid no puede estar vacio.");
            }
            return await FirebaseAuth.DefaultInstance.GetUserAsync(Uid);
        }

        public async Task<UsuarioDTO?> GetUserByEmail(string email)
        {
            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("El email no puede estar vacio.");
            }
            UserRecord fireBaseUser = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
            if (fireBaseUser == null)
            {
                throw new ArgumentException("El usuario no es valido.");
            }
            List<string> providers = new List<string>();
            foreach (var provider in fireBaseUser.ProviderData)
            {
                if (!string.IsNullOrEmpty(provider.ProviderId))
                {
                    providers.Add(provider.ProviderId);
                }
            }
            return new UsuarioDTO
            {
                Uid = fireBaseUser.Uid,
                Email = fireBaseUser.Email,
                Name = fireBaseUser.DisplayName,
                PhotoUrl = fireBaseUser.PhotoUrl,
                Providers = providers
            };
        }

        public async Task<UsuarioDTO?> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }
            var user = httpContext.User;
            if (user == null)
            {
                return null;
            }
            var uid = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (uid == null)
            {
                return null;
            }
            return await GetUserByUid(uid);
        }
    }
}
