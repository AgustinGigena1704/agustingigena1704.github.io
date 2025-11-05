using CCC.Api.Data.Entities.Repositories;
using CCC.Api.Interfaces;
using CCC.Api.Services;
using CCC.Shared;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CCC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;
        private readonly EntityManager entityManager;
        public AuthController(IAuthService _authService, EntityManager entityManager)
        {
            authService = _authService;
            this.entityManager = entityManager;
        }

        [HttpGet]
        [Route("GetUserByToken/{token}")]
        public async Task<IActionResult> GetUserByToken(string token)
        {
            var user = await authService.GetUserByToken(token);
            if (user == null)
            {
                return Unauthorized();
            }
            AuthValidationDTO authValidationDTO = new AuthValidationDTO() { User = user, IdToken = token };
            return Ok(authValidationDTO);
        }

        [HttpGet]
        [Route("GetUserByUId/{uid}")]
        public async Task<IActionResult> GetUserByUId(string uid)
        {
            var user = await authService.GetUserByUid(uid);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpGet]
        [Route("EmailAccountExists{email}")]
        public async Task<IActionResult> EmailAccountExists(string email)
        {
            if(authService is FireBaseService)
            {
                var fireBase = (FireBaseService)authService;
                var user = await fireBase.GetUserByEmail(email);
                return Ok(user is UsuarioDTO);
            }
            return Ok(false);
        }

        [HttpGet]
        [Route("RegisterNewUser/{UId}")]
        public async Task<IActionResult> RegisterNewUser(string UId)
        {
            try
            {
                if (authService is not FireBaseService)
                {
                    return StatusCode(StatusCodes.Status501NotImplemented, "El servicio de autenticacion no soporta el registro de usuarios.");
                }
                if (string.IsNullOrEmpty(UId))
                {
                    return BadRequest();
                }
                var newUser = await ((FireBaseService)authService).GetUserRecordByUId(UId);
                if (newUser == null)
                    return BadRequest("El usuario no es valido.");
                var registeredUser = await entityManager.GetRepository<UsuarioRepository>().RegisterUserAsync(newUser);
                if (registeredUser == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error registering user.");
                }
                var user = await authService.GetUserByUid(registeredUser.UId);
                if (user == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving registered user.");
                }
                return Created(UId, user);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                if(ex is ControledException)
                {
                    return StatusCode(StatusCodes.Status208AlreadyReported);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
