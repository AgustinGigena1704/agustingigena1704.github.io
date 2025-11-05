using CCC.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CCC.Api.Data
{
    public class CCCDbContext : DbContext
    {
        public CCCDbContext(DbContextOptions<CCCDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoMovimiento> TiposMovimiento { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<MovimientoDetalle> MovimientoDetalles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            SetAudithoryForeignKeys(modelBuilder);


            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.UId)
                .IsUnique();
            modelBuilder.Entity<Usuario>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();
            modelBuilder.Entity<TipoMovimiento>()
                .HasIndex(tm => tm.Codigo)
                .IsUnique();
            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.TipoMovimiento)
                .WithMany()
                .HasForeignKey(m => m.TipoMovimientoId);
            modelBuilder.Entity<MovimientoDetalle>()
                .HasOne(md => md.Movimiento)
                .WithMany()
                .HasForeignKey(md => md.MovimientoId);
            modelBuilder.Entity<MovimientoDetalle>()
                .HasOne(md => md.Producto)
                .WithMany()
                .HasForeignKey(md => md.ProductoId);


            base.OnModelCreating(modelBuilder);
        }







        private void SetAudithoryForeignKeys(ModelBuilder modelBuilder)
        {
            var baseModel = typeof(GenericEntity);
            // Obtener todas las entidades que heredan de GenericEntity
            // y configurar las claves foráneas para las propiedades de auditoría
            var derivedEntities = modelBuilder.Model.GetEntityTypes()
                .Where(et => baseModel.IsAssignableFrom(et.ClrType) && et.ClrType != baseModel);
            foreach (var entityType in derivedEntities)
            {
                modelBuilder.Entity(entityType.ClrType).HasOne(typeof(Usuario), "CreatedByUser")
                    .WithMany()
                    .HasForeignKey("CreatedBy")
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired();
                modelBuilder.Entity(entityType.ClrType).HasOne(typeof(Usuario), "UpdatedByUser")
                    .WithMany()
                    .HasForeignKey("UpdatedBy")
                    .OnDelete(DeleteBehavior.Restrict);
                modelBuilder.Entity(entityType.ClrType).HasOne(typeof(Usuario), "DeletedByUser")
                    .WithMany()
                    .HasForeignKey("DeletedBy")
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}