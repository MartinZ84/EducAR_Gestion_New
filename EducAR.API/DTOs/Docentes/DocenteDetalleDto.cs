namespace EducAR.API.DTOs.Docentes;

public class DocenteDetalleDto
{
    public int IdDocente { get; set; }
    public int Dni { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool Activo { get; set; }
    public List<CursoDocenteDetalleDto> CursosAsignados { get; set; } = new();
}

public class CursoDocenteDetalleDto
{
    public int IdCurso { get; set; }
    public string Curso { get; set; } = null!;
    public string Materia { get; set; } = null!;
    public string CicloLectivo { get; set; } = null!;
}
