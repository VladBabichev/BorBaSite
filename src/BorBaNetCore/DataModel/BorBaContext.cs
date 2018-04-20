using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BorBaNetCore.DataModel
{
    public partial class BorBaContext : DbContext
    {
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SysSettings> SysSettings { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserTokens> UserTokens { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        private readonly IOptions<ConnectionStringOption> _conStrOptions;

        public BorBaContext()
        {
        }
        public BorBaContext(IOptions<ConnectionStringOption> conStrOptions, DbContextOptions options)
        : base(options)
        {
            _conStrOptions = conStrOptions;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_conStrOptions.Value.ConStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.ImageId)
                    .HasName("PK_Images");

                entity.Property(e => e.FileName).HasMaxLength(150);

                entity.Property(e => e.UpdatedOn).HasDefaultValueSql("sysdatetime()");
            });

            modelBuilder.Entity<Messages>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_Messages");

                entity.Property(e => e.Email).HasColumnType("varchar(150)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.RegDate).HasColumnType("datetime");

                entity.Property(e => e.Subject)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                entity.Property(e => e.Text).HasColumnType("text");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.HasKey(e => e.RoleId)
                    .HasName("PK_Roles");

                entity.Property(e => e.Abbrev)
                    .IsRequired()
                    .HasColumnType("varchar(20)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<SysSettings>(entity =>
            {
                entity.HasKey(e => e.CompanyId)
                    .HasName("PK_SysSettings");

                entity.Property(e => e.CompanyId).ValueGeneratedNever();
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRoles_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserRoles_User");
            });

            modelBuilder.Entity<UserTokens>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK_UserTokens");

                entity.Property(e => e.ExpiryTime).HasColumnType("datetime");

                entity.Property(e => e.Token).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserTokens)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_UserTokens_User");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK_Users");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(50);
            });
        }
    }
}