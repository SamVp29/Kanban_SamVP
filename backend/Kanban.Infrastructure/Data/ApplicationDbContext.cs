using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Kanban.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Proyecto> Proyectos { get; set; } = null!;
    public DbSet<Columna> Columnas { get; set; } = null!;
    public DbSet<Tarea> Tareas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relación Proyecto -> Columnas (Cascade delete)
        modelBuilder.Entity<Proyecto>()
            .HasMany(p => p.Columnas)
            .WithOne(c => c.Proyecto)
            .HasForeignKey(c => c.ProyectoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación Columna -> Tareas (Restrict delete: no se permite eliminar columna con tareas)
        modelBuilder.Entity<Columna>()
            .HasMany(c => c.Tareas)
            .WithOne(t => t.Columna)
            .HasForeignKey(t => t.ColumnaId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación Tarea -> Responsable (SetNull si se elimina el usuario)
        modelBuilder.Entity<Tarea>()
            .HasOne(t => t.Responsable)
            .WithMany()
            .HasForeignKey(t => t.ResponsableId)
            .OnDelete(DeleteBehavior.SetNull);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seeding de Usuarios
        var salt1 = Guid.NewGuid().ToString();
        var salt2 = Guid.NewGuid().ToString();

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario 
            { 
                Id = 1, 
                Nombre = "Admin Kanban", 
                Correo = "admin@kanban.com", 
                PasswordSalt = salt1, 
                PasswordHash = HashPassword("Password123!", salt1) 
            },
            new Usuario 
            { 
                Id = 2, 
                Nombre = "Tester Kanban", 
                Correo = "tester@kanban.com", 
                PasswordSalt = salt2, 
                PasswordHash = HashPassword("Password123!", salt2) 
            }
        );
    }

    private string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        // Agregamos un pepper fijo de ejemplo (Recomendado en requerimientos)
        string pepper = "K@nb4n_P3pp3r_2024!";
        var bytes = Encoding.UTF8.GetBytes(password + salt + pepper);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
