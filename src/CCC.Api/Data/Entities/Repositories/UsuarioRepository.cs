using CCC.Shared;
using CCC.Api.Attributes;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;

namespace CCC.Api.Data.Entities.Repositories
{
    [Repository(entityType: typeof(Usuario), repositoryType: typeof(UsuarioRepository))]
    public class UsuarioRepository : GenericRepository<Usuario>
    {
        public UsuarioRepository(CCCDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Usuario?> RegisterUserAsync(UserRecord userRecord)
        {
            try
            {
                if(await dbSet.FirstOrDefaultAsync(u => u.UId == userRecord.Uid) != null)
                {
                    throw new ControledException("El usuario ya esta registrado en la base de datos.");
                }
                Usuario user = new Usuario
                {
                    Email = userRecord.Email,
                    UId = userRecord.Uid,
                    PhotoUrl = userRecord.PhotoUrl ?? "",
                    Nombre = userRecord.DisplayName,

                };
                dbSet.Add(user);
                await context.SaveChangesAsync();
                return user;
            }
            catch(Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        } 
    }
}
