using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace RecipeReviews.Models
{
    public partial class RecipeReviewsContext : DbContext
    {
        public RecipeReviewsContext()
        {
        }

        public RecipeReviewsContext(DbContextOptions<RecipeReviewsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<RatingHistory> RatingHistory { get; set; }
        public virtual DbSet<Recipe> Recipe { get; set; }
        public virtual DbSet<RecipeTag> RecipeTag { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("nchar(80)");

                entity.Property(e => e.ImageFilename)
                    .HasColumnName("Image Filename")
                    .HasColumnType("nchar(255)");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Recipe)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("UserId");
            });

            modelBuilder.Entity<RatingHistory>(entity =>
            {
                entity.HasKey(e => new { e.AccountId, e.RecipeId })
                    .HasName("PK__RatingHistory");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("CAccountId");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Ratings)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("CRecipeId");
            });

            modelBuilder.Entity<RecipeTag>(entity =>
            {
                entity.HasKey(e => new { e.RecipeId, e.TagId })
                    .HasName("PK__RecipeTa__EBD59FFE4D3C1B27");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.RecipeTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CTagIdRT");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.RecipeTag)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("CRecipeIdRT");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("nchar(30)");
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.PictureFilename)
                    .HasColumnName("Picture Filename")
                    .HasColumnType("nchar(255)");

            });
        }
    }
}
