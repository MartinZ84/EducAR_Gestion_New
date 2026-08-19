namespace EducAR.API.DTOs.Alumnos;

public class AlumnoDto
{
    public int IdAlumno { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public bool Activo { get; set; }

    // Datos extendidos
    public DateTime? FechaNacimiento { get; set; }

    // Domicilio
    public string? Calle { get; set; }
    public string? Numero { get; set; }
    public string? Piso { get; set; }
    public string? Departamento { get; set; }
    public string? Barrio { get; set; }
    public string? Localidad { get; set; }
    public string? Provincia { get; set; }
}