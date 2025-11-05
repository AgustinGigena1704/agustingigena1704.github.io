namespace CCC.Api.Data.Entities
{
    public class Producto : GenericEntity
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty; 
    }
}
