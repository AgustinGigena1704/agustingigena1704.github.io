namespace CCC.Api.Data.Entities
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public abstract class GenericEntity
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [DefaultValue(null)]
        public DateTime? UpdatedAt { get; set; }
        [DefaultValue(null)]
        public DateTime? DeletedAt { get; set; }
        [Required]
        public required int CreatedBy { get; set; }
        public required Usuario CreatedByUser { get; set; }
        [DefaultValue(null)]
        public int? UpdatedBy { get; set; }
        public Usuario? UpdatedByUser { get; set; }
        [DefaultValue(null)]
        public int? DeletedBy { get; set; }
        public Usuario? DeletedByUser { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;

    }
}
