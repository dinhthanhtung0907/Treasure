using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treasure.Data.Entitites
{
    [Table("MatrixInputHistory")]
    public class MatrixInputHistory : BaseEntity
    {
        public int? RowMatrix { get; set; }
        public int? ColumnMatrix { get; set; }
        public int? Treasure { get; set; }
        [MaxLength(5000)]
        public string? DataMatrix { get; set; } = string.Empty;
    }
}
