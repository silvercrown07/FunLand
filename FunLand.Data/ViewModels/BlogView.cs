using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FunLand.Data.ViewModels
{
    public class BlogView
    {
        public Guid BlogId { get; set; }
        
        [MaxLength(256)]
        public string Title { get; set; }
        
        [MaxLength(102400)]
        public string Content { get; set; }
        
        public List<BlogAttachmentView> BlogAttachments { get; set; }
    }
}