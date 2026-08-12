using EducAR.API.DTOs.PeriodosEvaluacion;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class PeriodoEvaluacionService : IPeriodoEvaluacionService
{
    private readonly IPeriodoEvaluacionRepository _periodoRepository;
    private readonly ICicloLectivoRepository _cicloLectivoRepository;

    public PeriodoEvaluacionService(
        IPeriodoEvaluacionRepository periodoRepository,
        ICicloLectivoRepository cicloLectivoRepository)
    {
        _periodoRepository = periodoRepository;
        _cicloLectivoRepository = cicloLectivoRepository;
    }

    public async Task<List<PeriodoEvaluacionResponseDto>> ObtenerTodos(int idCicloLectivo, int idEscuela)
    {
        // Verificar que el ciclo pertenece a la escuela
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null) return new List<PeriodoEvaluacionResponseDto>();

        var periodos = await _periodoRepository.ObtenerTodos(idCicloLectivo);
        return periodos.Select(MapearAResponseDto).ToList();
    }

    public async Task<PeriodoEvaluacionResponseDto?> ObtenerPorId(int idPeriodo, int idCicloLectivo, int idEscuela)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null) return null;

        var periodo = await _periodoRepository.ObtenerPorId(idPeriodo, idCicloLectivo);
        return periodo is null ? null : MapearAResponseDto(periodo);
    }

    public async Task<(bool exito, string mensaje, PeriodoEvaluacionResponseDto? periodo)> Crear(PeriodoEvaluacionCreateDto dto, int idEscuela)
    {
        // Validar que el ciclo existe y pertenece a la escuela
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(dto.IdCicloLectivo, idEscuela);
        if (ciclo is null)
            return (false, "El ciclo lectivo no existe.", null);

        if (!ciclo.Activo)
            return (false, "No se puede crear un período en un ciclo lectivo inactivo.", null);

        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de fin debe ser posterior a la fecha de inicio.", null);

        if (dto.FechaInicio < ciclo.FechaInicio || dto.FechaFin > ciclo.FechaFin)
            return (false, "Las fechas del período deben estar dentro del ciclo lectivo.", null);

        if (await _periodoRepository.ExisteNombre(dto.Nombre, dto.IdCicloLectivo))
            return (false, $"Ya existe un período llamado '{dto.Nombre}' en este ciclo lectivo.", null);

        var periodo = new PeriodoEvaluacion
        {
            IdCicloLectivo = dto.IdCicloLectivo,
            Nombre         = dto.Nombre,
            FechaInicio    = dto.FechaInicio,
            FechaFin       = dto.FechaFin,
            Activo         = true
        };

        var creado = await _periodoRepository.Crear(periodo);
        var completo = await _periodoRepository.ObtenerPorId(creado.IdPeriodoEvaluacion, dto.IdCicloLectivo);
        return (true, "Período de evaluación creado correctamente.", MapearAResponseDto(completo!));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idPeriodo, int idCicloLectivo, int idEscuela, PeriodoEvaluacionUpdateDto dto)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null)
            return (false, "El ciclo lectivo no existe.");

        var periodo = await _periodoRepository.ObtenerPorId(idPeriodo, idCicloLectivo);
        if (periodo is null)
            return (false, "Período de evaluación no encontrado.");

        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de fin debe ser posterior a la fecha de inicio.");

        if (dto.FechaInicio < ciclo.FechaInicio || dto.FechaFin > ciclo.FechaFin)
            return (false, "Las fechas del período deben estar dentro del ciclo lectivo.");

        if (await _periodoRepository.ExisteNombre(dto.Nombre, idCicloLectivo, idPeriodo))
            return (false, $"Ya existe un período llamado '{dto.Nombre}' en este ciclo lectivo.");

        periodo.Nombre      = dto.Nombre;
        periodo.FechaInicio = dto.FechaInicio;
        periodo.FechaFin    = dto.FechaFin;
        periodo.Activo      = dto.Activo;

        await _periodoRepository.Actualizar(periodo);
        return (true, "Período de evaluación actualizado correctamente.");
    }

    public async Task<(bool exito, string mensaje)> Eliminar(int idPeriodo, int idCicloLectivo, int idEscuela)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null)
            return (false, "El ciclo lectivo no existe.");

        return await _periodoRepository.Eliminar(idPeriodo, idCicloLectivo);
    }

    private static PeriodoEvaluacionResponseDto MapearAResponseDto(PeriodoEvaluacion p) => new()
    {
        IdPeriodoEvaluacion = p.IdPeriodoEvaluacion,
        IdCicloLectivo      = p.IdCicloLectivo,
        Anio                = p.CicloLectivo.Anio,
        Nombre              = p.Nombre,
        FechaInicio         = p.FechaInicio,
        FechaFin            = p.FechaFin,
        Activo              = p.Activo
    };
}