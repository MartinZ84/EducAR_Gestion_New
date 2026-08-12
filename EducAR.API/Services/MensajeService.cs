using EducAR.API.DTOs.Mensajes;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.Data;
using Microsoft.EntityFrameworkCore;
using EducAR.API.DTOs.Paginacion;

using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class MensajeService : IMensajeService
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly AppDbContext _context;

    public MensajeService(IMensajeRepository mensajeRepository, AppDbContext context)
    {
        _mensajeRepository = mensajeRepository;
        _context = context;
    }

    public async Task<List<MensajeResumenDto>> ObtenerRecibidos(int idUsuario)
    {
        var mensajes = await _mensajeRepository.ObtenerRecibidos(idUsuario);
        return mensajes.Select(MapearAResumenDto).ToList();
    }

    public async Task<List<MensajeResumenDto>> ObtenerEnviados(int idUsuario)
    {
        var mensajes = await _mensajeRepository.ObtenerEnviados(idUsuario);
        return mensajes.Select(MapearAResumenDto).ToList();
    }

    public async Task<MensajeResponseDto?> ObtenerPorId(int idMensaje, int idUsuario)
    {
        var mensaje = await _mensajeRepository.ObtenerPorId(idMensaje, idUsuario);
        if (mensaje is null) return null;

        // Marcar como leído automáticamente si es el destinatario
        if (mensaje.IdUsuarioDestinat == idUsuario && !mensaje.Leido)
            await _mensajeRepository.MarcarLeido(idMensaje, idUsuario);

        return MapearAResponseDto(mensaje);
    }

    public async Task<(bool exito, string mensaje)> Enviar(MensajeCreateDto dto, int idUsuarioRemitente, int idEscuela)
    {
        // Validar que el destinatario existe y pertenece a la misma escuela
        var destinatario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == dto.IdUsuarioDestinat &&
                                      u.IdEscuela == idEscuela &&
                                      u.Activo);

        if (destinatario is null)
            return (false, "El destinatario no existe o no pertenece a esta escuela.");

        if (dto.IdUsuarioDestinat == idUsuarioRemitente)
            return (false, "No puede enviarse un mensaje a sí mismo.");

        var mensaje = new Mensaje
        {
            IdUsuarioRemitente = idUsuarioRemitente,
            IdUsuarioDestinat = dto.IdUsuarioDestinat,
            Asunto = dto.Asunto,
            MensajeTexto = dto.MensajeTexto,
            FechaEnvio = DateTime.Now,
            Leido = false,
            Activo = true
        };

        await _mensajeRepository.Crear(mensaje);
        return (true, "Mensaje enviado correctamente.");
    }

    public async Task<bool> MarcarLeido(int idMensaje, int idUsuario)
    {
        return await _mensajeRepository.MarcarLeido(idMensaje, idUsuario);
    }

    public async Task<bool> Eliminar(int idMensaje, int idUsuario)
    {
        return await _mensajeRepository.Eliminar(idMensaje, idUsuario);
    }

    public async Task<int> ContarNoLeidos(int idUsuario)
    {
        return await _mensajeRepository.ContarNoLeidos(idUsuario);
    }

    private static MensajeResumenDto MapearAResumenDto(Mensaje m) => new()
    {
        IdMensaje = m.IdMensaje,
        NombreRemitente = $"{m.Remitente.Nombre} {m.Remitente.Apellido}",
        NombreDestinatario = $"{m.Destinatario.Nombre} {m.Destinatario.Apellido}",
        Asunto = m.Asunto,
        FechaEnvio = m.FechaEnvio,
        Leido = m.Leido
    };

    private static MensajeResponseDto MapearAResponseDto(Mensaje m) => new()
    {
        IdMensaje = m.IdMensaje,
        IdUsuarioRemitente = m.IdUsuarioRemitente,
        NombreRemitente = $"{m.Remitente.Nombre} {m.Remitente.Apellido}",
        IdUsuarioDestinat = m.IdUsuarioDestinat,
        NombreDestinatario = $"{m.Destinatario.Nombre} {m.Destinatario.Apellido}",
        Asunto = m.Asunto,
        MensajeTexto = m.MensajeTexto,
        FechaEnvio = m.FechaEnvio,
        Leido = m.Leido
    };
    public async Task<ResultadoPaginadoDto<MensajeResumenDto>> ObtenerRecibidosPaginado(
    int idUsuario, int pagina, int cantidad)
    {
        var query = await _mensajeRepository.ObtenerQueryableRecibidos(idUsuario);
        var queryDto = query.Select(m => new MensajeResumenDto
        {
            IdMensaje = m.IdMensaje,
            NombreRemitente = m.Remitente.Nombre + " " + m.Remitente.Apellido,
            NombreDestinatario = m.Destinatario.Nombre + " " + m.Destinatario.Apellido,
            Asunto = m.Asunto,
            FechaEnvio = m.FechaEnvio,
            Leido = m.Leido
        });

        return await PaginacionHelper.PaginarAsync(queryDto, pagina, cantidad);
    }

    public async Task<ResultadoPaginadoDto<MensajeResumenDto>> ObtenerEnviadosPaginado(
        int idUsuario, int pagina, int cantidad)
    {
        var query = await _mensajeRepository.ObtenerQueryableEnviados(idUsuario);
        var queryDto = query.Select(m => new MensajeResumenDto
        {
            IdMensaje = m.IdMensaje,
            NombreRemitente = m.Remitente.Nombre + " " + m.Remitente.Apellido,
            NombreDestinatario = m.Destinatario.Nombre + " " + m.Destinatario.Apellido,
            Asunto = m.Asunto,
            FechaEnvio = m.FechaEnvio,
            Leido = m.Leido
        });

        return await PaginacionHelper.PaginarAsync(queryDto, pagina, cantidad);
    }
}