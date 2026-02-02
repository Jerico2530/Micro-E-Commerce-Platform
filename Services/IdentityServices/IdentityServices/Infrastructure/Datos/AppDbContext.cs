using IdentityServices.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace IdentityServices.Infrastructure.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<UserRol> UserRoles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<PermRol> PermRoles { get; set; }

        // Configuración centralizada de relaciones, claves y reglas del modelo
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario - Rol (N:M)
            modelBuilder.Entity<UserRol>()
                 .HasKey(ur => ur.UserRolId);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Usuario)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RolId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rol - Permiso(N: M)
            modelBuilder.Entity<PermRol>()
                .HasKey(ur => ur.PermRolId);

            modelBuilder.Entity<PermRol>()
                .HasOne(ur => ur.Permiso)
                .WithMany(u => u.PermRoles)
                .HasForeignKey(ur => ur.PermisoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PermRol>()
                .HasOne(ur => ur.Rol)
                .WithMany(u => u.PermRoles)
                .HasForeignKey(ur => ur.RolId)
                .OnDelete(DeleteBehavior.Cascade);



        }
    }
}
