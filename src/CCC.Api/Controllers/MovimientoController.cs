using CCC.Api.Data.Entities.Repositories;
using CCC.Api.Interfaces;
using CCC.Api.Services;
using CCC.Api.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CCC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
     // Require authentication for all actions in this controller
    public class MovimientoController : GenericController
    {
        public MovimientoController(IAuthService authService, EntityManager entityManager) : base(authService, entityManager)
        {
            
        }

        [HttpGet]
        [Route("Test")]
        public IActionResult GetTest()
        {
            var result = entityManager.GetRepository<MovimientoRepository>().GetTest();
            return Ok(result);
        }
    }
}
