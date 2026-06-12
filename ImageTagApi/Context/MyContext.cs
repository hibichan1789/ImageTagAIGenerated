using ImageTagApi.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTagApi.Context
{
    public class MyContext:DbContext
    {
        public MyContext(DbContextOptions<MyContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<DbFile> Files { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<CssStyle> CssStyles { get; set; }
        public DbSet<FileTag> FileTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // DbFiles
            modelBuilder.Entity<DbFile>()
                .Property(f => f.Status)
                .HasConversion<int>();

            modelBuilder.Entity<DbFile>()
                .HasOne(f => f.User)
                .WithMany(u => u.Files)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tags
            modelBuilder.Entity<Tag>()
                .HasIndex(t => t.Value)
                .IsUnique();


            // CssStyle
            modelBuilder.Entity<CssStyle>()
                .HasIndex(c => c.BgColor)
                .IsUnique();

            modelBuilder.Entity<CssStyle>()
                .Property(c => c.TailwindColor)
                .HasConversion<int>();


            // FileTag(中間テーブル)
            modelBuilder.Entity<FileTag>()
                .HasKey(ft => new { ft.FileId, ft.TagId, ft.CssStyleId });

            modelBuilder.Entity<FileTag>()
                .HasOne(ft => ft.File)
                .WithMany(f => f.FileTags)
                .HasForeignKey(ft => ft.FileId);

            modelBuilder.Entity<FileTag>()
                .HasOne(ft => ft.Tag)
                .WithMany(t => t.FileTags)
                .HasForeignKey(ft => ft.TagId);

            modelBuilder.Entity<FileTag>()
                .HasOne(ft => ft.CssStyle)
                .WithMany(c => c.FileTags)
                .HasForeignKey(ft => ft.CssStyleId);
        }
    }
}
