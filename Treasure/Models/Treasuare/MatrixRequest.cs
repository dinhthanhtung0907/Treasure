using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Treasure.Models.Treasuare
{
    public class MatrixRequest
    {
        [Comment("row")]
        [JsonPropertyName("n")]
        public int N { get; set; }

        [Comment("Column")]
        [JsonPropertyName("m")]
        public int M { get; set; }

        [Comment("treasuare")]
        [JsonPropertyName("p")]
        public int P { get; set; }

        [JsonPropertyName("matrix")]
        public int[][] Matrix { get; set; }

        [JsonPropertyName("id")]
        public int? Id { get; set; }
    }
}
