namespace EducAR.API.DTOs.Materias;

public class MateriaDetalleDto
{
    public int IdMateria { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public List<DocenteMateriaDetalleDto> DocentesAsignados { get; set; } = new();
    public List<CursoMateriaDetalleDto> Cursos { get; set; } = new();
}

public class DocenteMateriaDetalleDto { public int IdDocente { get; set; } public string NombreCompleto { get; set; } = null!; }
public class CursoMateriaDetalleDto { public int IdCurso { get; set; } public string Curso { get; set; } = null!; public string CicloLectivo { get; set; } = null!; }
