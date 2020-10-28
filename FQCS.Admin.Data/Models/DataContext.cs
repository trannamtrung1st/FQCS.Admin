using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using static FQCS.Admin.Data.Constants;

namespace FQCS.Admin.Data.Models
{
    public partial class DataContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<AppEvent> AppEvent { get; set; }
        public virtual DbSet<DefectType> DefectType { get; set; }
        public virtual DbSet<ProductionBatch> ProductionBatch { get; set; }
        public virtual DbSet<ProductionLine> ProductionLine { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<QCDevice> QCDevice { get; set; }
        public virtual DbSet<QCEvent> QCEvent { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(Constants.Data.CONN_STR)
                    .UseLazyLoadingProxies();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
            });
            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Id)
                    .IsUnicode(false)
                    .HasMaxLength(100);
            });
            modelBuilder.Entity<Resource>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(255);
            });
            modelBuilder.Entity<AppEvent>(entity =>
            {
                var converter = new EnumToNumberConverter<AppEventType, int>();
                entity.Property(e => e.Type)
                    .HasConversion(converter);
                entity.Property(e => e.Description)
                    .HasMaxLength(2000);
                entity.Property(e => e.Data)
                    .HasMaxLength(2000);
                entity.HasOne(e => e.User)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AppUser_AppEvent");
            });
            modelBuilder.Entity<ProductionBatch>(entity =>
            {
                var converter = new EnumToNumberConverter<BatchStatus, int>();
                entity.Property(e => e.Status)
                    .HasConversion(converter);
                entity.Property(e => e.Code)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.Info)
                    .HasMaxLength(2000);
                entity.HasOne(e => e.Line)
                    .WithMany(e => e.Batches)
                    .HasForeignKey(e => e.ProductionLineId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ProductionLine_ProductionBatch");
                entity.HasOne(e => e.Model)
                    .WithMany(e => e.Batches)
                    .HasForeignKey(e => e.ProductModelId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ProductModel_ProductionBatch");
            });
            modelBuilder.Entity<ProductionLine>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.Info)
                    .HasMaxLength(2000);
            });
            modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.Name)
                    .HasMaxLength(255);
                entity.Property(e => e.Info)
                    .HasMaxLength(2000);
                entity.Property(e => e.Image)
                    .IsUnicode(false);
            });
            modelBuilder.Entity<QCDevice>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.Info)
                    .HasMaxLength(2000);
                entity.HasOne(e => e.Line)
                    .WithMany(e => e.Devices)
                    .HasForeignKey(e => e.ProductionLineId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ProductionLine_QCDevice");
            });
            modelBuilder.Entity<QCEvent>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(2000);
                entity.Property(e => e.LeftImage)
                    .IsUnicode(false)
                    .HasMaxLength(2000);
                entity.Property(e => e.RightImage)
                    .IsUnicode(false)
                    .HasMaxLength(2000);
                entity.HasOne(e => e.Batch)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.ProductionBatchId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ProductionBatch_QCEvent");
                entity.HasOne(e => e.Device)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.QCDeviceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_QCDevice_QCEvent");
                entity.HasOne(e => e.DefectType)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.DefectTypeId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_DefectType_QCEvent");
            });
            modelBuilder.Entity<DefectType>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.QCMappingCode)
                    .IsUnicode(false)
                    .HasMaxLength(100);
                entity.Property(e => e.Name)
                    .HasMaxLength(255);
                entity.Property(e => e.Description)
                    .HasMaxLength(2000);
                entity.Property(e => e.SampleImage)
                    .IsUnicode(false);
            });
        }
    }

    public class DbContextFactory : IDesignTimeDbContextFactory<DataContext>
    {

        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Constants.Data.CONN_STR);
            return new DataContext(optionsBuilder.Options);
        }
    }
}
