using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace test1.Models;

public partial class RentCarContext : IdentityDbContext<ApplicationUser>
{
    public RentCarContext()
    {
    }

    public RentCarContext(DbContextOptions<RentCarContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAd> TbAds { get; set; }

    public virtual DbSet<TbBrand> TbBrands { get; set; }

    public virtual DbSet<TbCar> TbCars { get; set; }

    public virtual DbSet<TbReview> TbReviews { get; set; }

    public virtual DbSet<TbSetting> TbSettings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TbAd>(entity =>
        {
            entity.Property(e => e.Id)
              .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.EndDate)
                .HasColumnType("datetime")
                .HasColumnName("End_Date");
            entity.Property(e => e.ImgName)
                .HasMaxLength(50)
                .HasColumnName("IMG_Name");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.StartDate)
                .HasColumnType("datetime")
                .HasColumnName("Start_Date");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.Url)
                .HasMaxLength(50)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<TbBrand>(entity =>
        {
            entity.HasKey(e => e.BrandId);

            entity.Property(e => e.BrandId)
                .ValueGeneratedNever()
                .HasColumnName("Brand_ID");
            entity.Property(e => e.BrandName)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Brand_name");
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("create_date");
            entity.Property(e => e.ImgName)
                .HasMaxLength(50)
                .HasColumnName("IMG_name");
            entity.Property(e => e.UpdateDate)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("update_date");
        });

        modelBuilder.Entity<TbCar>(entity =>
        {
            entity.ToTable("TbCar");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Brand).HasMaxLength(20);
            entity.Property(e => e.City)
                .HasMaxLength(15)
                .IsFixedLength();
            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Country)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasColumnName("Create_date");
            entity.Property(e => e.Description).HasMaxLength(300);
            entity.Property(e => e.Engine)
                .HasMaxLength(12)
                .IsFixedLength();
            entity.Property(e => e.Fuel)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Img1)
                .HasMaxLength(50)
                .HasColumnName("IMG1");
            entity.Property(e => e.Img2)
                .HasMaxLength(50)
                .HasColumnName("IMG2");
            entity.Property(e => e.Img3)
                .HasMaxLength(50)
                .HasColumnName("IMG3");
            entity.Property(e => e.Model).HasMaxLength(25);
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Status).HasMaxLength(15);
            entity.Property(e => e.Transmission).HasMaxLength(20);
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UserId)
                .HasMaxLength(450)
                .HasColumnName("User_ID");
            entity.Property(e => e.Year)
                .HasMaxLength(12)
                .IsFixedLength();
        });

        modelBuilder.Entity<TbReview>(entity =>
        {
            entity.Property(e => e.Id)
                .UseIdentityColumn()
                .HasColumnName("ID");

            entity.Property(e => e.ContentMsg)
                .HasMaxLength(150)
                .HasColumnName("Content_MSG");

            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("Created_date");

            entity.Property(e => e.IsPublic)
                .HasMaxLength(10)
                .HasColumnName("Is_Public");

            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsFixedLength();

            entity.Property(e => e.UpdatedDate)
                .HasColumnType("datetime")
                .HasColumnName("updated_date");

            entity.Property(e => e.CarId)
                .HasColumnName("CarId"); // ✅ سميه اسمه الحقيقي وخلي النوع int
        });


        modelBuilder.Entity<TbSetting>(entity =>
        {
            entity.Property(e => e.id)
              .UseIdentityColumn()
              .HasColumnName("id");

            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Facebook).HasMaxLength(50);
            entity.Property(e => e.Favicon).HasMaxLength(50);
            entity.Property(e => e.HomeImg1)
                .HasMaxLength(50)
                .HasColumnName("Home_IMG1");
            entity.Property(e => e.HomeImg2)
                .HasMaxLength(50)
                .HasColumnName("Home_IMG2");
            entity.Property(e => e.HomeImg3)
                .HasMaxLength(50)
                .HasColumnName("Home_IMG3");
            entity.Property(e => e.HomeTxt1)
                .HasMaxLength(500)
                .HasColumnName("Home_txt1");
            entity.Property(e => e.HomeTxt2)
                .HasMaxLength(500)
                .HasColumnName("Home_txt2");
            entity.Property(e => e.HomeTxt3)
                .HasMaxLength(500)
                .HasColumnName("Home_txt3");
            entity.Property(e => e.Instagram).HasMaxLength(50);
            entity.Property(e => e.Logo).HasMaxLength(50);
            entity.Property(e => e.SiteName)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("Site_Name");
            entity.Property(e => e.Whatsapp).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
