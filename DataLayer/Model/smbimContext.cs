using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataLayer.Model
{
    public partial class smbimContext : DbContext
    {
        public smbimContext()
        {
        }

        public smbimContext(DbContextOptions<smbimContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<ClaimedObjects> ClaimedObjects { get; set; }
        public virtual DbSet<IgnoredObjects> IgnoredObjects { get; set; }
        public virtual DbSet<ProjectLabels> ProjectLabels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:smbim.database.windows.net,1433;Initial Catalog=smbim;Persist Security Info=False;User ID=rudywilkjr;Password=Th1sisatest;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => new { e.Id, e.PostTime })
                    .HasName("PK_DDLAudits");

                entity.ToTable("AuditLog", "DDL_AUDIT");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.PostTime).HasColumnType("datetime");

                entity.Property(e => e.DatabaseName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Event).HasMaxLength(100);

                entity.Property(e => e.HostName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ObjectSchema).HasMaxLength(128);

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.ParentTable).HasMaxLength(128);

                entity.Property(e => e.Tsql).HasColumnName("TSQL");
            });

            modelBuilder.Entity<ClaimedObjects>(entity =>
            {
                entity.ToTable("ClaimedObjects", "DDL_AUDIT");

                entity.Property(e => e.Notes)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectDatabase)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectSchema)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectType)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.ReleaseDate).HasColumnType("datetime");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.ProjectLabel)
                    .WithMany(p => p.ClaimedObjects)
                    .HasForeignKey(d => d.ProjectLabelId)
                    .HasConstraintName("FK_DDLAuditClaimedObjects_ProjectLabel_ProjectLabelId");
            });

            modelBuilder.Entity<IgnoredObjects>(entity =>
            {
                entity.ToTable("IgnoredObjects", "DDL_AUDIT");

                entity.Property(e => e.IgnoredByTime).HasColumnType("datetime");

                entity.Property(e => e.IgnoredByUser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectDatabase)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ObjectSchema)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProjectLabels>(entity =>
            {
                entity.ToTable("ProjectLabels", "DDL_AUDIT");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
            });
        }
    }
}
