using EducAR.API.DTOs.Materias;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class MateriaService : IMateriaService
{
    private readonly IMateriaRepository _materiaRepository;

    public MateriaService(IMateriaRepository materiaRepository)
    {
        _materiaRepository = materiaRepository;
    }

    public async Task<List<MateriaResponseDto>> ObtenerTodas(int idEscuela)
    {
        var materias = await _materiaRepository.ObtenerTodas(idEscuela);
        return materias.Select(MapearAResponseDto).ToList();
    }

    public async Task<MateriaResponseDto?> ObtenerPorId(int idMateria, int idEscuela)
    {
        var materia = await _materiaRepository.ObtenerPorId(idMateria, idEscuela);
        return materia is null ? null : MapearAResponseDto(materia);
    }

    public async Task<MateriaDetalleDto?> ObtenerDetalle(int idMateria, int idEscuela)
    {
        var materia = await _materiaRepository.ObtenerDetalle(idMateria, idEscuela);
        if (materia is null) return null;
        return new MateriaDetalleDto { IdMateria = materia.IdMateria, Nombre = materia.Nombre, Descripcion = materia.Descripcion,
            DocentesAsignados = materia.DocenteMateriaCursos.Where(x => x.Activo && x.Docente.Activo).Select(x => new DocenteMateriaDetalleDto { IdDocente = x.IdDocente, NombreCompleto = $"{x.Docente.Usuario.Nombre} {x.Docente.Usuario.Apellido}" }).ToList(),
            Cursos = materia.DocenteMateriaCursos.Where(x => x.Activo && x.Curso.Activo).Select(x => new CursoMateriaDetalleDto { IdCurso = x.IdCurso, Curso = $"{x.Curso.Grado}° {x.Curso.Division}", CicloLectivo = x.Curso.CicloLectivo.Anio.ToString() }).DistinctBy(x => x.IdCurso).ToList() };
    }

    public async Task<(bool exito, string mensaje, MateriaResponseDto? materia)> Crear(MateriaCreateDto dto, int idEscuela)
    {
        if (await _materiaRepository.ExisteNombre(dto.Nombre, idEscuela))
            return (false, $"Ya existe una materia con el nombre '{dto.Nombre}'.", null);

        var materia = new Materia
        {
            IdEscuela   = idEscuela,
            Nombre      = dto.Nombre,
            Descripcion = dto.Descripcion,
            Activo      = true
        };

        var creada = await _materiaRepository.Crear(materia);
        return (true, "Materia creada correctamente.", MapearAResponseDto(creada));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idMateria, int idEscuela, MateriaUpdateDto dto)
    {
        var materia = await _materiaRepository.ObtenerPorId(idMateria, idEscuela);
        if (materia is null)
            return (false, "Materia no encontrada.");

        if (await _materiaRepository.ExisteNombre(dto.Nombre, idEscuela, idMateria))
            return (false, $"Ya existe una materia con el nombre '{dto.Nombre}'.");

        materia.Nombre      = dto.Nombre;
        materia.Descripcion = dto.Descripcion;
        materia.Activo      = dto.Activo;

        await _materiaRepository.Actualizar(materia);
        return (true, "Materia actualizada correctamente.");
    }

    public async Task<bool> Eliminar(int idMateria, int idEscuela)
    {
        return await _materiaRepository.Eliminar(idMateria, idEscuela);
    }

    public async Task<ResultadoPaginadoDto<MateriaResponseDto>> ObtenerTodasPaginado(int idEscuela, FiltroNombreDto filtro)
    {
        var query = await _materiaRepository.ObtenerQueryable(idEscuela, filtro.Nombre);
        var queryDto = query.Select(m => new MateriaResponseDto
        {
            IdMateria   = m.IdMateria,
            IdEscuela   = m.IdEscuela,
            Nombre      = m.Nombre,
            Descripcion = m.Descripcion,
            Activo      = m.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static MateriaResponseDto MapearAResponseDto(Materia m) => new()
    {
        IdMateria   = m.IdMateria,
        IdEscuela   = m.IdEscuela,
        Nombre      = m.Nombre,
        Descripcion = m.Descripcion,
        Activo      = m.Activo
    };
}
