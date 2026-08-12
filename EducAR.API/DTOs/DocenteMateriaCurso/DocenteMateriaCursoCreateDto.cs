using System.ComponentModel.DataAnnotations;

namespace EducAR.API.DTOs.DocenteMateriaCurso;

public class DocenteMateriaCursoCreateDto
{
    [Required(ErrorMessage = "El docente es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un docente válido.")]
    public int IdDocente { get; set; }

    [Required(ErrorMessage = "La materia es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una materia válida.")]
    public int IdMateria { get; set; }

    [Required(ErrorMessage = "El curso es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un curso válido.")]
    public int IdCurso { get; set; }
}
