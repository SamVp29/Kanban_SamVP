using Kanban.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
        // Hash estático BCrypt (cost 11) para "Password123!" generado externamente para no ralentizar el inicio
        var passwordHash = "$2a$11$0lA2YO/An/7Ybh/43wcH8.XgHkRCPlybm5uE1/LUJA8GssgLiiMZe";

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario 
            { 
                Id = 1, 
                Nombre = "Admin Kanban", 
                Correo = "admin@kanban.com", 
                PasswordSalt = "", // Ya no es necesario con BCrypt, pero la propiedad está en la entidad
                PasswordHash = passwordHash 
            },
            new Usuario 
            { 
                Id = 2, 
                Nombre = "Tester Kanban", 
                Correo = "tester@kanban.com", 
                PasswordSalt = "", 
                PasswordHash = passwordHash 
            }
        );
    }
}
