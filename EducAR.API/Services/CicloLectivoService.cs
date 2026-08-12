using EducAR.API.DTOs.CiclosLectivos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class CicloLectivoService : ICicloLectivoService
{
    private readonly ICicloLectivoRepository _cicloLectivoRepository;

    public CicloLectivoService(ICicloLectivoRepository cicloLectivoRepository)
    {
        _cicloLectivoRepository = cicloLectivoRepository;
    }

    public async Task<List<CicloLectivoResponseDto>> ObtenerTodos(int idEscuela)
    {
        var ciclos = await _cicloLectivoRepository.ObtenerTodos(idEscuela);
        return ciclos.Select(MapearAResponseDto).ToList();
    }

    public async Task<CicloLectivoResponseDto?> ObtenerPorId(int idCicloLectivo, int idEscuela)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        return ciclo is null ? null : MapearAResponseDto(ciclo);
    }

    public async Task<(bool exito, string mensaje, CicloLectivoResponseDto? ciclo)> Crear(CicloLectivoCreateDto dto, int idEscuela)
    {
        if (await _cicloLectivoRepository.ExisteAnio(dto.Anio, idEscuela))
            return (false, $"Ya existe un ciclo lectivo para el año {dto.Anio}.", null);

        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de fin debe ser posterior a la fecha de inicio.", null);

        var ciclo = new CicloLectivo
        {
            IdEscuela    = idEscuela,
            Anio         = dto.Anio,
            FechaInicio  = dto.FechaInicio,
            FechaFin     = dto.FechaFin,
            Activo       = true
        };

        var creado = await _cicloLectivoRepository.Crear(ciclo);
        return (true, "Ciclo lectivo creado correctamente.", MapearAResponseDto(creado));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idCicloLectivo, int idEscuela, CicloLectivoUpdateDto dto)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null)
            return (false, "Ciclo lectivo no encontrado.");

        if (await _cicloLectivoRepository.ExisteAnio(dto.Anio, idEscuela, idCicloLectivo))
            return (false, $"Ya existe un ciclo lectivo para el año {dto.Anio}.");

        if (dto.FechaFin <= dto.FechaInicio)
            return (false, "La fecha de fin debe ser posterior a la fecha de inicio.");

        ciclo.Anio        = dto.Anio;
        ciclo.FechaInicio = dto.FechaInicio;
        ciclo.FechaFin    = dto.FechaFin;
        ciclo.Activo      = dto.Activo;

        await _cicloLectivoRepository.Actualizar(ciclo);
        return (true, "Ciclo lectivo actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idCicloLectivo, int idEscuela)
    {
        return await _cicloLectivoRepository.Eliminar(idCicloLectivo, idEscuela);
    }

    private static CicloLectivoResponseDto MapearAResponseDto(CicloLectivo c) => new()
    {
        IdCicloLectivo = c.IdCicloLectivo,
        IdEscuela      = c.IdEscuela,
        Anio           = c.Anio,
        FechaInicio    = c.FechaInicio,
        FechaFin       = c.FechaFin,
        Activo         = c.Activo
    };
}