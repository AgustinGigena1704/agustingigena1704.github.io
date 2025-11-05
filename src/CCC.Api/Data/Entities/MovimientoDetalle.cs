namespace CCC.Api.Data.Entities
{
    public class MovimientoDetalle : GenericEntity
    {
        public required int MovimientoId { get; set; }
        public required Movimiento Movimiento { get; set; }
        public required int ProductoId { get; set; }
        public required Producto Producto { get; set; }
        public string Observacion { get; set; } = string.Empty;
        public required int Cantidad { get; set; } = 0;
        public decimal PrecioUnitario { get; set; } = 0;
    }
}
