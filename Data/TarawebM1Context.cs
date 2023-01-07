using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Taraweb.Models.TarawebM1;
using System.Reflection.Emit;

namespace Taraweb.Data
{
    public partial class TarawebM1Context : DbContext
    {
        public TarawebM1Context()
        {
        }

        public TarawebM1Context(DbContextOptions<TarawebM1Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gallery>(entity =>
            {
                entity.ToTable("Gallery");

               
                entity.Property(e => e.DateCreate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.FileExtension)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.FileType)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.ToTable("Language");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(10);
            });

            modelBuilder.Entity<PageCategory>(entity =>
            {
                entity.ToTable("PageCategory");

                entity.Property(e => e.CategoryCode)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.DateCreate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Link)
                    .HasMaxLength(500)
                    .HasColumnName("link");

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.PageCategories)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_1");
            });

            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("Post");

                entity.Property(e => e.DateCreate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DateFinish).HasColumnType("datetime");

                entity.Property(e => e.DateStart).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.HasOne(d => d.Gallery)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.GalleryId)
                    .HasConstraintName("FK_Gallery_post");

                entity.HasOne(d => d.PageCategory)
                    .WithMany(p => p.Posts)
                    .HasForeignKey(d => d.PageCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PageCat_post");
            });

            modelBuilder.Entity<PostContent>(entity =>
            {
                entity.ToTable("PostContent");

                entity.Property(e => e.Content).HasColumnType("ntext");

                entity.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.PostContents)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostContent_Language");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostContents)
                    .HasForeignKey(d => d.PostId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PostContent_Post");
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
        public DbSet<Taraweb.Models.TarawebM1.Gallery> Galleries { get; set; }

        public DbSet<Taraweb.Models.TarawebM1.Language> Languages { get; set; }

        public DbSet<Taraweb.Models.TarawebM1.PageCategory> PageCategories { get; set; }

        public DbSet<Taraweb.Models.TarawebM1.Post> Posts { get; set; }
        public DbSet<Taraweb.Models.TarawebM1.PostContent> PostContents { get; set; }
    }
}