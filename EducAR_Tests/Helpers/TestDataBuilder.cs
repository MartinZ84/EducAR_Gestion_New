using EducAR.API.Models;

namespace EducAR.Tests.Helpers;

public static class TestDataBuilder
{
    public static Escuela BuildEscuela(int id = 1) => new()
    {
        IdEscuela = id,
        Nombre    = "Escuela Test",
        Direccion = "Av. Test 100",
        Telefono  = "2664000000",
        Email     = "test@escuela.com",
        Activo    = true,
        FechaCrea = DateTime.Now,
        FechaAct  = DateTime.Now
    };

    public static Rol BuildRol(int id = 1, string nombre = "Administrador") => new()
    {
        IdRol  = id,
        Nombre = nombre,
        Activo = true
    };

    public static Usuario BuildUsuario(
        int id = 1,
        int idRol = 1,
        int idEscuela = 1,
        string nombreUsuario = "admin",
        string rolNombre = "Administrador") => new()
    {
        IdUsuario      = id,
        IdRol          = idRol,
        IdEscuela      = idEscuela,
        Dni            = 12345678,
        Nombre         = "Admin",
        Apellido       = "Test",
        Email          = "admin@test.com",
        NombreUsuario  = nombreUsuario,
        HashContrasena = BCrypt.Net.BCrypt.HashPassword("Admin123"),
        Activo         = true,
        FechaCrea      = DateTime.Now,
        FechaAct       = DateTime.Now,
        Rol            = BuildRol(idRol, rolNombre),
        Escuela        = BuildEscuela(idEscuela)
    };

    public static Docente BuildDocente(int id = 1, int idUsuario = 2) => new()
    {
        IdDocente = id,
        IdUsuario = idUsuario,
        Activo    = true,
        FechaCrea = DateTime.Now,
        FechaAct  = DateTime.Now,
        Usuario   = BuildUsuario(idUsuario, 2, 1, "docente1", "Docente")
    };

    public static Tutor BuildTutor(int id = 1, int idUsuario = 3) => new()
    {
        IdTutor       = id,
        IdUsuario     = idUsuario,
        EsResponsable = true,
        FechaCrea     = DateTime.Now,
        FechaAct      = DateTime.Now,
        Usuario       = BuildUsuario(idUsuario, 3, 1, "tutor1", "Tutor")
    };

    public static Alumno BuildAlumno(int id = 1, int idEscuela = 1) => new()
    {
        IdAlumno  = id,
        IdEscuela = idEscuela,
        Dni       = 40000001 + id,
        FecNac    = new DateOnly(2014, 1, Math.Min(id, 28)),
        Nombre    = $"Alumno{id}",
        Apellido  = $"Apellido{id}",
        Activo    = true,
        FechaCrea = DateTime.Now,
        FechaAct  = DateTime.Now,
        Escuela   = BuildEscuela(idEscuela)
    };

    public static CicloLectivo BuildCicloLectivo(int id = 1, int idEscuela = 1) => new()
    {
        IdCicloLectivo = id,
        IdEscuela      = idEscuela,
        Anio           = 2026,
        FechaInicio    = new DateTime(2026, 3, 1),
        FechaFin       = new DateTime(2026, 12, 15),
        Activo         = true,
        FechaCrea      = DateTime.Now,
        FechaAct       = DateTime.Now,
        Escuela        = BuildEscuela(idEscuela)
    };

    public static Materia BuildMateria(int id = 1, int idEscuela = 1) => new()
    {
        IdMateria   = id,
        IdEscuela   = idEscuela,
        Nombre      = $"Materia{id}",
        Descripcion = $"Descripcion{id}",
        Activo      = true,
        FechaCrea   = DateTime.Now,
        FechaAct    = DateTime.Now,
        Escuela     = BuildEscuela(idEscuela)
    };

    public static Curso BuildCurso(int id = 1, int idEscuela = 1, int idCiclo = 1) => new()
    {
        IdCurso        = id,
        IdEscuela      = idEscuela,
        IdCicloLectivo = idCiclo,
        Grado          = 1,
        Division       = "A",
        Turno          = "Mañana",
        Activo         = true,
        FechaCrea      = DateTime.Now,
        FechaAct       = DateTime.Now,
        CicloLectivo   = BuildCicloLectivo(idCiclo, idEscuela),
        Matriculas     = new List<Matricula>()
    };

    public static PeriodoEvaluacion BuildPeriodo(int id = 1, int idCiclo = 1) => new()
    {
        IdPeriodoEvaluacion = id,
        IdCicloLectivo      = idCiclo,
        Nombre              = "1er Trimestre",
        FechaInicio         = new DateTime(2026, 3, 1),
        FechaFin            = new DateTime(2026, 5, 31),
        Activo              = true,
        FechaCrea           = DateTime.Now,
        FechaAct            = DateTime.Now,
        CicloLectivo        = BuildCicloLectivo(idCiclo)
    };
}
