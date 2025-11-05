using CCC.Api.Data;
using CCC.Api.Data.Entities;
using CCC.Api.Attributes;
using Microsoft.EntityFrameworkCore;

namespace CCC.Api.Data.Entities.Repositories
{
    [Repository(entityType: typeof(Movimiento), repositoryType: typeof(MovimientoRepository))]
    public class MovimientoRepository : GenericRepository<Movimiento>
    {
        public MovimientoRepository(CCCDbContext context) : base(context) { }

        public bool GetTest()
        {
            return true;
        }
    }

    
}
