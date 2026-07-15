using MasterNet.Domain;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Persistence;

class MasterNetDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=LocalDatabase.db")
        .LogTo(
        Console.WriteLine,
        new[] { DbLoggerCategory.Database.Command.Name },
        Microsoft.Extensions.Logging.LogLevel.Information).EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Curso>().ToTable("cursos");
        modelBuilder.Entity<Instructor>().ToTable("instructores");
        modelBuilder.Entity<CursoInstructor>().ToTable("curso_instructores");
        modelBuilder.Entity<Precio>().ToTable("precios");
        modelBuilder.Entity<CursoPrecio>().ToTable("curso_precios");
        modelBuilder.Entity<Calificacion>().ToTable("calificaciones");
        modelBuilder.Entity<Photo>().ToTable("imagenes");
        modelBuilder.Entity<Precio>().Property(b => b.PrecioActual).HasPrecision(10, 2);
        modelBuilder.Entity<Precio>().Property(b => b.PrecioPromocion).HasPrecision(10, 2);
        modelBuilder.Entity<Precio>().Property(b => b.Nombre).HasColumnType("VARCHAR").HasMaxLength(250);

        modelBuilder.Entity<Curso>().HasMany(m => m.Photos).WithOne(m => m.Curso)
        .HasForeignKey(m => m.CursoId).IsRequired().OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Curso>().HasMany(m => m.Calificaciones).WithOne(m => m.Curso)
        .HasForeignKey(m => m.CursoId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Curso>().HasMany(m => m.Precios).WithMany(m => m.Cursos)
        .UsingEntity<CursoPrecio>(
            j => j.HasOne(p => p.Precio).WithMany(p => p.CursoPrecios).HasForeignKey(p => p.PrecioId),
            j => j.HasOne(p => p.Curso).WithMany(p => p.CursoPrecios).HasForeignKey(p => p.CursoId),
            j =>
            {
                j.HasKey(t => new { t.PrecioId, t.CursoId });
            }
        );

        modelBuilder.Entity<Curso>().HasMany(m => m.Instructores).WithMany(m => m.Cursos)
        .UsingEntity<CursoInstructor>(
            j => j.HasOne(p => p.Instructor).WithMany(p => p.CursoInstructores).HasForeignKey(p => p.InstructorId),
            j => j.HasOne(p => p.Curso).WithMany(p => p.CursoInstructores).HasForeignKey(p => p.CursoId),
            j =>
            {
                j.HasKey(t => new { t.InstructorId, t.CursoId });
            }
        );
    }
}