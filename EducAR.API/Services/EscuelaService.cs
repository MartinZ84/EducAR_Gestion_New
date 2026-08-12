using EducAR.API.DTOs.Escuelas;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class EscuelaService : IEscuelaService
{
    private readonly IEscuelaRepository _escuelaRepository;

    public EscuelaService(IEscuelaRepository escuelaRepository)
    {
        _escuelaRepository = escuelaRepository;
    }

    public async Task<List<EscuelaResponseDto>> ObtenerTodas()
    {
        var escuelas = await _escuelaRepository.ObtenerTodas();
        return escuelas.Select(MapearAResponseDto).ToList();
    }

    public async Task<EscuelaResponseDto?> ObtenerPorId(int idEscuela)
    {
        var escuela = await _escuelaRepository.ObtenerPorId(idEscuela);
        return escuela is null ? null : MapearAResponseDto(escuela);
    }

    public async Task<(bool exito, string mensaje, EscuelaResponseDto? escuela)> Crear(EscuelaCreateDto dto)
    {
        if (await _escuelaRepository.ExisteNombre(dto.Nombre))
            return (false, $"Ya existe una escuela con el nombre '{dto.Nombre}'.", null);

        var escuela = new Escuela
        {
            Nombre    = dto.Nombre,
            Direccion = dto.Direccion,
            Telefono  = dto.Telefono,
            Email     = dto.Email,
            Activo    = true
        };

        var creada = await _escuelaRepository.Crear(escuela);
        return (true, "Escuela creada correctamente.", MapearAResponseDto(creada));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idEscuela, EscuelaUpdateDto dto)
    {
        var escuela = await _escuelaRepository.ObtenerPorId(idEscuela);
        if (escuela is null)
            return (false, "Escuela no encontrada.");

        if (await _escuelaRepository.ExisteNombre(dto.Nombre, idEscuela))
            return (false, $"Ya existe una escuela con el nombre '{dto.Nombre}'.");

        escuela.Nombre    = dto.Nombre;
        escuela.Direccion = dto.Direccion;
        escuela.Telefono  = dto.Telefono;
        escuela.Email     = dto.Email;
        escuela.Activo    = dto.Activo;

        await _escuelaRepository.Actualizar(escuela);
        return (true, "Escuela actualizada correctamente.");
    }

    public async Task<ResultadoPaginadoDto<EscuelaResponseDto>> ObtenerTodasPaginado(FiltroNombreDto filtro)
    {
        var query = await _escuelaRepository.ObtenerQueryable(filtro.Nombre);
        var queryDto = query.Select(e => new EscuelaResponseDto
        {
            IdEscuela = e.IdEscuela,
            Nombre    = e.Nombre,
            Direccion = e.Direccion,
            Telefono  = e.Telefono,
            Email     = e.Email,
            Activo    = e.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static EscuelaResponseDto MapearAResponseDto(Escuela e) => new()
    {
        IdEscuela = e.IdEscuela,
        Nombre    = e.Nombre,
        Direccion = e.Direccion,
        Telefono  = e.Telefono,
        Email     = e.Email,
        Activo    = e.Activo
    };
}
