using CCC.Api.Interfaces;
using CCC.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenericController : ControllerBase
    {
        protected readonly IAuthService authService;
        protected readonly EntityManager entityManager;
        public GenericController(
            IAuthService authService,
            EntityManager entityManager)
        {
            this.authService = authService;
            this.entityManager = entityManager;
        }
    }
}
