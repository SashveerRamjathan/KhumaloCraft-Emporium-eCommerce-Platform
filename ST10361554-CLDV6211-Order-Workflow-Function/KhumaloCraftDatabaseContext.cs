using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ST10361554_CLDV6211_Order_Workflow_Function.Models;

namespace ST10361554_CLDV6211_Order_Workflow_Function;

public partial class KhumaloCraftDatabaseContext : DbContext
{
    public KhumaloCraftDatabaseContext()
    {
    }

    public KhumaloCraftDatabaseContext(DbContextOptions<KhumaloCraftDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Artist> Artists { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Craftwork> Craftworks { get; set; }

    public virtual DbSet<CraftworkCategory> CraftworkCategories { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:st10361554-cldv6211-server-poe.database.windows.net,1433;Initial Catalog=KhumaloCraft_Database;Persist Security Info=False;User ID=ST10361554;Password=Sashveer10361554#;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Artist>(entity =>
        {
            entity.HasKey(e => e.ArtistId).HasName("PK__Artist__25706B70F54D7956");

            entity.ToTable("Artist");

            entity.Property(e => e.ArtistId).HasColumnName("ArtistID");
            entity.Property(e => e.ArtistDescription).IsUnicode(false);
            entity.Property(e => e.ArtistEmail).HasMaxLength(256);
            entity.Property(e => e.ArtistName).HasMaxLength(256);
            entity.Property(e => e.ArtistPictureUrl).HasColumnName("ArtistPictureURL");

            entity.HasOne(d => d.ArtistNavigation).WithOne(p => p.Artist)
                .HasForeignKey<Artist>(d => d.ArtistId)
                .HasConstraintName("FK__Artist__ArtistPi__4CA06362");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<AspNetUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Craftwork>(entity =>
        {
            entity.HasKey(e => e.CraftworkId).HasName("PK__Craftwor__7A817A83D6B36954");

            entity.ToTable("Craftwork");

            entity.Property(e => e.CraftworkId).HasColumnName("CraftworkID");
            entity.Property(e => e.ArtistId)
                .HasMaxLength(450)
                .HasColumnName("ArtistID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CraftworkDescription).IsUnicode(false);
            entity.Property(e => e.CraftworkName).HasMaxLength(450);
            entity.Property(e => e.CraftworkPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Artist).WithMany(p => p.Craftworks)
                .HasForeignKey(d => d.ArtistId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Craftwork_Artist");

            entity.HasOne(d => d.Category).WithMany(p => p.Craftworks)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Craftwork__Categ__5165187F");
        });

        modelBuilder.Entity<CraftworkCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Craftwor__19093A2B0977FD8E");

            entity.ToTable("CraftworkCategory");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(256);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Feedback__1788CCACB4F6A6FF");

            entity.ToTable("Feedback");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Comment).IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.UserRole).HasMaxLength(250);

            entity.HasOne(d => d.User).WithOne(p => p.Feedback)
                .HasForeignKey<Feedback>(d => d.UserId)
                .HasConstraintName("FK__Feedback__UserID__6754599E");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BAF1DB9D699");

            entity.Property(e => e.OrderId).HasColumnName("OrderID");
            entity.Property(e => e.OrderStatus)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.OrderTotalAmount).HasColumnType("decimal(20, 2)");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Orders__UserID__619B8048");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C1FF3252E");

            entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");
            entity.Property(e => e.CraftworkId).HasColumnName("CraftworkID");
            entity.Property(e => e.CraftworkStatus)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.OrderId).HasColumnName("OrderID");

            entity.HasOne(d => d.Craftwork).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.CraftworkId)
                .HasConstraintName("FK__OrderDeta__Craft__7B5B524B");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__OrderDeta__Order__7A672E12");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
