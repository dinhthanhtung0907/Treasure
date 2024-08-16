using System.ComponentModel.DataAnnotations;

namespace Treasure.Data.Entitites
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        [MaxLength(2000)]
        public string? Description { get; set; } = string.Empty;
    }
}
