using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1
{
    [Table("URL")] 
    public class URL
    {
        public int Id { get; set; }
        public required string LongUrl { get; set; }
        public required string ShortCode { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime createAt { get; set; } = DateTime.UtcNow;
    }
}