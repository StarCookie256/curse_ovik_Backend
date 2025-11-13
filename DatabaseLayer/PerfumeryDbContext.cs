using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PerfumeryBackend.DatabaseLayer.Models;

namespace PerfumeryBackend.DatabaseLayer;

public partial class PerfumeryDbContext : DbContext
{
    //private readonly IConfiguration _configuration = null!;
    //public PerfumeryDbContext()
    //{
    //}

    public PerfumeryDbContext(DbContextOptions<PerfumeryDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketItem> BasketItems { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductVariation> ProductVariations { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlite(_configuration.GetConnectionString("DefaultConnection"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.ToTable("Baskets");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.Date).HasColumnName("Date");
            entity.Property(e => e.Status).HasColumnName("Status");
            entity.Property(e => e.TotalPrice).HasColumnName("TotalPrice");

            entity.HasOne(d => d.Customer).WithMany(p => p.Baskets).HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.ToTable("BasketItems");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BasketId).HasColumnName("BasketID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Quantity).HasColumnName("Quantity");
            entity.Property(e => e.Volume).HasColumnName("Volume");
            entity.Property(e => e.Price).HasColumnName("Price");

            entity.HasOne(d => d.Basket).WithMany(p => p.BasketItems).HasForeignKey(d => d.BasketId);

            entity.HasOne(d => d.Product).WithMany(p => p.BasketItems).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brands");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasColumnName("Name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasColumnName("Name");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Countries");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasColumnName("Name");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasIndex(e => e.Email, "IX_Customers_Email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name).HasColumnName("Name");
            entity.Property(e => e.Image).HasColumnName("Image");
            entity.Property(e => e.Email).HasColumnName("Email");
            entity.Property(e => e.Phone).HasColumnName("Phone");
            entity.Property(e => e.Address).HasColumnName("Address");
            entity.Property(e => e.Password).HasColumnName("Password");
            entity.Property(e => e.PasswordSalt).HasColumnName("PasswordSalt");

            entity.OwnsOne(e => e.RefreshToken, owned =>
            {
                owned.Property(rt => rt.Token)
                    .HasColumnName("RefreshToken_Token");
                owned.HasIndex(e => e.Token, "IX_Customers_RefreshToken_Token").IsUnique();

                owned.Property(rt => rt.Expires)
                    .HasColumnName("RefreshToken_Expires");
            });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandId).HasColumnName("BrandID");
            entity.Property(e => e.CountryId).HasColumnName("CountryID");
            entity.Property(e => e.Name).HasColumnName("Name");
            entity.Property(e => e.Description).HasColumnName("Description");
            entity.Property(e => e.Image).HasColumnName("Image");
            entity.Property(e => e.Gender).HasColumnName("Gender");
            entity.Property(e => e.ManufactureYear).HasColumnName("ManufactureYear");
            entity.Property(e => e.ExpirationDate).HasColumnName("ExpirationDate");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products).HasForeignKey(d => d.BrandId);

            entity.HasOne(d => d.Country).WithMany(p => p.Products).HasForeignKey(d => d.CountryId);
        });

        modelBuilder.Entity<ProductVariation>(entity =>
        {
            entity.ToTable("ProductVariations");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Volume).HasColumnName("Volume");
            entity.Property(e => e.Price).HasColumnName("Price");
            entity.Property(e => e.Stock).HasColumnName("Stock");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductVariations).HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariations).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Reviews");
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Rating).HasColumnName("Rating");
            entity.Property(e => e.Comment).HasColumnName("Comment");
            entity.Property(e => e.Date).HasColumnName("Date");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews).HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews).HasForeignKey(d => d.ProductId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
