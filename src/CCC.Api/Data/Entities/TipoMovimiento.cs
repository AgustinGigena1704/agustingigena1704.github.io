using System.ComponentModel.DataAnnotations;

namespace CCC.Api.Data.Entities
{

    public class TipoMovimiento : GenericEntity
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        [Required]
        public string Codigo { get; set; } = string.Empty;
    }
}
