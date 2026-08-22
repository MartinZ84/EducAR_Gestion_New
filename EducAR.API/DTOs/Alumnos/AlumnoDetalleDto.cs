namespace EducAR.API.DTOs.Alumnos;

public class AlumnoDetalleDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public DateTime? FecNac { get; set; }
    public bool Activo { get; set; }
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Piso { get; set; }
    public string? Departamento { get; set; }
    public string? Barrio { get; set; }
    public string? Localidad { get; set; }
    public string? Provincia { get; set; }
    public List<TelefonoDetalleDto> Telefonos { get; set; } = new();
    public MatriculaActualDto? MatriculaActual { get; set; }
    public List<AlumnoTutorDetalleDto> Tutores { get; set; } = new();
    public AsistenciaResumenDto AsistenciaResumen { get; set; } = new();
    public List<AsistenciaDetalleDto> Asistencias { get; set; } = new();
    public List<CalificacionDetalleDto> Calificaciones { get; set; } = new();
    public List<BoletinDetalleDto> Boletines { get; set; } = new();
}

public class MatriculaActualDto { public int IdCurso { get; set; } public string Curso { get; set; } = null!; public string CicloLectivo { get; set; } = null!; public DateTime FechaMatricula { get; set; } }
public class AlumnoTutorDetalleDto { public int IdTutor { get; set; } public string NombreCompleto { get; set; } = null!; public string? Parentesco { get; set; } }
public class AsistenciaResumenDto { public int Presentes { get; set; } public int Ausentes { get; set; } public int Justificadas { get; set; } }
public class AsistenciaDetalleDto { public int IdAsistencia { get; set; } public DateTime Fecha { get; set; } public bool Presente { get; set; } public string Estado { get; set; } = null!; }
public class CalificacionDetalleDto { public string Materia { get; set; } = null!; public decimal Nota { get; set; } public string Periodo { get; set; } = null!; }
public class BoletinDetalleDto { public string Periodo { get; set; } = null!; public decimal Promedio { get; set; } public string Estado { get; set; } = null!; }
public class TelefonoDetalleDto { public string Numero { get; set; } = null!; public string? Des { get; set; } }
