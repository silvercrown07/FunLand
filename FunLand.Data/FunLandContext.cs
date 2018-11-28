using FunLand.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FunLand.Data
{
    public class FunLandContext : DbContext
    {
        public FunLandContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogAttachment> BlogAttachments { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<BlogAttachment>()
                .Property(model => model.BlogAttachmentId)
                .ValueGeneratedOnAdd();
        }
    }
}