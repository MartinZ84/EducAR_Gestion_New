using EducAR.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Escuela> Escuelas => Set<Escuela>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Docente> Docentes => Set<Docente>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Alumno> Alumnos => Set<Alumno>();
    public DbSet<AlumnoTutor> AlumnoTutores => Set<AlumnoTutor>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<CicloLectivo> CiclosLectivos => Set<CicloLectivo>();
    public DbSet<Materia> Materias => Set<Materia>();
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<DocenteMateriaCurso> DocenteMateriaCursos => Set<DocenteMateriaCurso>();
    public DbSet<PeriodoEvaluacion> PeriodosEvaluacion => Set<PeriodoEvaluacion>();
    public DbSet<Asistencia> Asistencias => Set<Asistencia>();
    public DbSet<Calificacion> Calificaciones => Set<Calificacion>();
    public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>();
    public DbSet<NotaEvaluacion> NotasEvaluacion => Set<NotaEvaluacion>();
    public DbSet<Boletin> Boletines => Set<Boletin>();
    public DbSet<DetalleBoletin> DetallesBoletines => Set<DetalleBoletin>();
    public DbSet<Mensaje> Mensajes => Set<Mensaje>();
    public DbSet<TelefonoContacto> TelefonosContacto { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Evaluacion>()
            .HasOne(e => e.Curso).WithMany().HasForeignKey(e => e.IdCurso)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Evaluacion>()
            .HasOne(e => e.Materia).WithMany().HasForeignKey(e => e.IdMateria)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<Evaluacion>()
            .HasOne(e => e.PeriodoEvaluacion).WithMany().HasForeignKey(e => e.IdPeriodoEvaluacion)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<NotaEvaluacion>()
            .HasOne(n => n.Evaluacion).WithMany(e => e.Notas).HasForeignKey(n => n.IdEvaluacion)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<NotaEvaluacion>()
            .HasOne(n => n.Alumno).WithMany().HasForeignKey(n => n.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<NotaEvaluacion>()
            .HasIndex(n => new { n.IdEvaluacion, n.IdAlumno }).IsUnique();
        modelBuilder.Entity<NotaEvaluacion>()
            .Property(n => n.Valor).HasPrecision(4, 2);
        modelBuilder.Entity<NotaEvaluacion>()
            .ToTable(t => t.HasCheckConstraint("CK_NotasEvaluacion_Valor", "[Valor] >= 1 AND [Valor] <= 10"));
        modelBuilder.Entity<Evaluacion>()
            .HasIndex(e => new { e.IdCurso, e.IdMateria, e.IdPeriodoEvaluacion, e.Activo });

        // ==========================
        // USUARIO
        // ==========================

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.IdRol)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Escuela)
            .WithMany(e => e.Usuarios)
            .HasForeignKey(u => u.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Docente)
            .WithOne(d => d.Usuario)
            .HasForeignKey<Docente>(d => d.IdUsuario)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Tutor)
            .WithOne(t => t.Usuario)
            .HasForeignKey<Tutor>(t => t.IdUsuario)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // ESCUELA
        // ==========================

        modelBuilder.Entity<Alumno>()
            .HasOne(a => a.Escuela)
            .WithMany(e => e.Alumnos)
            .HasForeignKey(a => a.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Materia>()
            .HasOne(m => m.Escuela)
            .WithMany(e => e.Materias)
            .HasForeignKey(m => m.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Curso>()
            .HasOne(c => c.Escuela)
            .WithMany(e => e.Cursos)
            .HasForeignKey(c => c.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CicloLectivo>()
            .HasOne(c => c.Escuela)
            .WithMany(e => e.CiclosLectivos)
            .HasForeignKey(c => c.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // CICLO LECTIVO
        // ==========================

        modelBuilder.Entity<Curso>()
            .HasOne(c => c.CicloLectivo)
            .WithMany(cl => cl.Cursos)
            .HasForeignKey(c => c.IdCicloLectivo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PeriodoEvaluacion>()
            .HasOne(p => p.CicloLectivo)
            .WithMany(c => c.PeriodosEvaluacion)
            .HasForeignKey(p => p.IdCicloLectivo)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // MATRICULA
        // ==========================

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Escuela)
            .WithMany(e => e.Matriculas)
            .HasForeignKey(m => m.IdEscuela)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Alumno)
            .WithMany(a => a.Matriculas)
            .HasForeignKey(m => m.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.Curso)
            .WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.IdCurso)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Matricula>()
            .HasOne(m => m.CicloLectivo)
            .WithMany(c => c.Matriculas)
            .HasForeignKey(m => m.IdCicloLectivo)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Matricula>()
            .Property(m => m.Estado)
            .HasConversion<string>()
            .HasMaxLength(20);

        // ==========================
        // ALUMNO TUTOR
        // ==========================

        modelBuilder.Entity<AlumnoTutor>()
            .HasOne(at => at.Alumno)
            .WithMany(a => a.AlumnoTutores)
            .HasForeignKey(at => at.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AlumnoTutor>()
            .HasOne(at => at.Tutor)
            .WithMany(t => t.AlumnoTutores)
            .HasForeignKey(at => at.IdTutor)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // DOCENTE MATERIA CURSO
        // ==========================

        modelBuilder.Entity<DocenteMateriaCurso>()
            .HasOne(d => d.Docente)
            .WithMany(x => x.DocenteMateriaCursos)
            .HasForeignKey(d => d.IdDocente)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocenteMateriaCurso>()
            .HasOne(d => d.Materia)
            .WithMany(m => m.DocenteMateriaCursos)
            .HasForeignKey(d => d.IdMateria)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DocenteMateriaCurso>()
            .HasOne(d => d.Curso)
            .WithMany(c => c.DocenteMateriaCursos)
            .HasForeignKey(d => d.IdCurso)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // ASISTENCIA
        // ==========================

        modelBuilder.Entity<Asistencia>()
            .HasOne(a => a.Docente)
            .WithMany(d => d.Asistencias)
            .HasForeignKey(a => a.IdDocente)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Asistencia>()
            .HasOne(a => a.Alumno)
            .WithMany(al => al.Asistencias)
            .HasForeignKey(a => a.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Asistencia>()
            .HasOne(a => a.Curso)
            .WithMany(c => c.Asistencias)
            .HasForeignKey(a => a.IdCurso)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // CALIFICACION
        // ==========================

        modelBuilder.Entity<Calificacion>()
            .HasOne(c => c.Alumno)
            .WithMany(a => a.Calificaciones)
            .HasForeignKey(c => c.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Calificacion>()
            .HasOne(c => c.Materia)
            .WithMany(m => m.Calificaciones)
            .HasForeignKey(c => c.IdMateria)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Calificacion>()
            .HasOne(c => c.PeriodoEvaluacion)
            .WithMany(p => p.Calificaciones)
            .HasForeignKey(c => c.IdPeriodoEvaluacion)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Calificacion>()
            .Property(c => c.ValorCalificacion)
            .HasPrecision(5, 2);

        // ==========================
        // BOLETIN
        // ==========================

        modelBuilder.Entity<Boletin>()
            .HasOne(b => b.Alumno)
            .WithMany(a => a.Boletines)
            .HasForeignKey(b => b.IdAlumno)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Boletin>()
            .HasOne(b => b.Curso)
            .WithMany(c => c.Boletines)
            .HasForeignKey(b => b.IdCurso)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Boletin>()
            .HasOne(b => b.PeriodoEvaluacion)
            .WithMany(p => p.Boletines)
            .HasForeignKey(b => b.IdPeriodoEvaluacion)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // DETALLE BOLETIN
        // ==========================

        modelBuilder.Entity<DetalleBoletin>()
            .HasOne(d => d.Boletin)
            .WithMany(b => b.Detalles)
            .HasForeignKey(d => d.IdBoletin)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DetalleBoletin>()
            .HasOne(d => d.Materia)
            .WithMany(m => m.DetallesBoletines)
            .HasForeignKey(d => d.IdMateria)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<DetalleBoletin>()
            .Property(d => d.CalificacionFinal)
            .HasPrecision(5, 2);

        // ==========================
        // MENSAJES
        // ==========================

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Remitente)
            .WithMany(u => u.MensajesEnviados)
            .HasForeignKey(m => m.IdUsuarioRemitente)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mensaje>()
            .HasOne(m => m.Destinatario)
            .WithMany(u => u.MensajesRecibidos)
            .HasForeignKey(m => m.IdUsuarioDestinat)
            .OnDelete(DeleteBehavior.NoAction);

        // ==========================
        // ÍNDICES
        // ==========================

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => new { u.IdEscuela, u.NombreUsuario })
            .IsUnique();

        modelBuilder.Entity<Alumno>()
            .HasIndex(a => new { a.IdEscuela, a.FechaCrea });

        modelBuilder.Entity<Matricula>()
            .HasIndex(x => new { x.IdAlumno, x.IdCicloLectivo })
            .IsUnique();

        modelBuilder.Entity<Matricula>()
            .HasIndex(x => new { x.IdCurso, x.Estado });

        modelBuilder.Entity<AlumnoTutor>()
            .HasIndex(x => new { x.IdAlumno, x.IdTutor })
            .IsUnique();

        modelBuilder.Entity<DocenteMateriaCurso>()
            .HasIndex(x => new { x.IdDocente, x.IdMateria, x.IdCurso })
            .IsUnique();

        modelBuilder.Entity<Asistencia>()
            .HasIndex(x => new { x.IdAlumno, x.IdCurso, x.Fecha })
            .IsUnique();

        modelBuilder.Entity<Calificacion>()
            .HasIndex(x => new { x.IdAlumno, x.IdMateria, x.IdPeriodoEvaluacion })
            .IsUnique();

        modelBuilder.Entity<Boletin>()
            .HasIndex(x => new { x.IdAlumno, x.IdPeriodoEvaluacion })
            .IsUnique();

        modelBuilder.Entity<DetalleBoletin>()
            .HasIndex(x => new { x.IdBoletin, x.IdMateria })
            .IsUnique();

        modelBuilder.Entity<TelefonoContacto>()
            .HasOne(t => t.Alumno)
            .WithMany(a => a.Telefonos)
            .HasForeignKey(t => t.IdAlumno)
            .OnDelete(DeleteBehavior.Cascade); // O NoAction según tu preferencia
    }
}
