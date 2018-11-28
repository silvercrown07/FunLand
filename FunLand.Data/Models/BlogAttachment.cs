using System.ComponentModel.DataAnnotations;

namespace FunLand.Data.Models
{
    public class BlogAttachment
    {
        [Key]
        public int BlogAttachmentId { get; set; }

        public string Path { get; set; }
    }
}