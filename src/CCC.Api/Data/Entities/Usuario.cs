using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCC.Api.Data.Entities
{
    public class Usuario
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public required string UId { get; set; }
        [Required]
        public required string Email { get; set; }
        public string Nombre { get; set; } = string.Empty;  
        public string PhotoUrl { get; set; } = string.Empty;
    }
}
