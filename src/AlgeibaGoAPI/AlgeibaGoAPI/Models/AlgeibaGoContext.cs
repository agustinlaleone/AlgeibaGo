using Microsoft.EntityFrameworkCore;

namespace AlgeibaGoAPI.Models
{
    public partial class AlgeibaGoContext : DbContext
    {
        public AlgeibaGoContext()
        {
        }

        public AlgeibaGoContext(DbContextOptions<AlgeibaGoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Routes> Routes { get; set; }
        public virtual DbSet<RouteVisit> RouteVisit { get; set; }
        public virtual DbSet<RelRouteUser> RelRouteUser { get; set; }
        public virtual DbSet<Favorites> Favorites { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Routes>(entity =>
            {
                entity.Property(e => e.RedirectUrl)
                    .IsRequired()
                    .HasColumnName("RedirectURL")
                    .IsUnicode(false);

                entity.Property(e => e.PageTitle)
                    .IsRequired()
                    .HasColumnName("PageTitle")
                    .IsUnicode(false);

                entity.Property(e => e.Route)
                    .IsRequired()
                    .HasMaxLength(2000)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RouteVisit>(entity =>
            {
                entity.Property(e => e.VisitDate).HasColumnType("datetime");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.RouteVisit)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_RouteVisit_Routes");
            });

            modelBuilder.Entity<RelRouteUser>(entity =>
            {
                entity.HasKey(e => e.IdRelation);

                entity.HasOne(d => d.IdRouteNavigation)
                    .WithMany(p => p.RelRouteUser)
                    .HasForeignKey(d => d.IdRoute)
                    .HasConstraintName("FK_RelRouteUser_Routes");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.RelRouteUser)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_RelRouteUser_Users");
            });

            modelBuilder.Entity<Favorites>(entity =>
            {
                entity.HasKey(e => e.IdFavorites);

                entity.HasOne(d => d.IdRouteNavigation)
                    .WithMany(p => p.FavoriteRoute)
                    .HasForeignKey(d => d.IdRoute)
                    .HasConstraintName("FK_Favorites_Routes");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.FavoriteUser)
                    .HasForeignKey(d => d.IdUser)
                    .HasConstraintName("FK_Favorites_Users");

                entity.Property(e => e.isFavorite);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);

                entity.Property(e => e.PersonName).IsUnicode(false);

                entity.Property(e => e.PersonSurname).IsUnicode(false);
            });
        }

        public DbSet<AlgeibaGoAPI.Models.User> User { get; set; }
    }
}
