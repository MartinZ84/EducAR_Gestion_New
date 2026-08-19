using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.Alumnos;

public class AlumnoCreateDto
{
    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [Range(1000000, 99999999, ErrorMessage = "El DNI debe tener entre 7 y 8 dígitos.")]
    public int Dni { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(100)]
    public string Apellido { get; set; } = string.Empty;

    public DateTime? FechaNacimiento { get; set; }

    // Domicilio
    [MaxLength(200)]
    public string? Calle { get; set; }

    [MaxLength(20)]
    public string? Numero { get; set; }

    [MaxLength(10)]
    public string? Piso { get; set; }

    [MaxLength(10)]
    public string? Departamento { get; set; }

    [MaxLength(100)]
    public string? Barrio { get; set; }

    [MaxLength(100)]
    public string? Localidad { get; set; }

    [MaxLength(100)]
    public string? Provincia { get; set; }
}