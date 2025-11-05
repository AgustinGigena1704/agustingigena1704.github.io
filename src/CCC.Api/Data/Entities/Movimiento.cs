using System.ComponentModel.DataAnnotations;

namespace CCC.Api.Data.Entities
{
    public class Movimiento : GenericEntity
    {
        [Required]
        public int TipoMovimientoId { get; set; }
        public TipoMovimiento TipoMovimiento { get; set; } = null!;
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;
        [Required]
        public string NombreCredito { get; set; } = string.Empty;
        [Required]
        public string NombreDebito { get; set; } = string.Empty;
        [Required]
        public bool Escaneado { get; set; } = false;
    }
}
