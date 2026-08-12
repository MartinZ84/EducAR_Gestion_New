using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class MensajeRepository : IMensajeRepository
{
    private readonly AppDbContext _context;

    public MensajeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Mensaje>> ObtenerRecibidos(int idUsuario)
    {
        return await _context.Mensajes
            .Include(m => m.Remitente)
            .Include(m => m.Destinatario)
            .Where(m => m.IdUsuarioDestinat == idUsuario && m.Activo)
            .OrderByDescending(m => m.FechaEnvio)
            .ToListAsync();
    }

    public async Task<List<Mensaje>> ObtenerEnviados(int idUsuario)
    {
        return await _context.Mensajes
            .Include(m => m.Remitente)
            .Include(m => m.Destinatario)
            .Where(m => m.IdUsuarioRemitente == idUsuario && m.Activo)
            .OrderByDescending(m => m.FechaEnvio)
            .ToListAsync();
    }

    public async Task<Mensaje?> ObtenerPorId(int idMensaje, int idUsuario)
    {
        return await _context.Mensajes
            .Include(m => m.Remitente)
            .Include(m => m.Destinatario)
            .FirstOrDefaultAsync(m => m.IdMensaje == idMensaje &&
                                      (m.IdUsuarioRemitente == idUsuario ||
                                       m.IdUsuarioDestinat == idUsuario) &&
                                      m.Activo);
    }

    public async Task<Mensaje> Crear(Mensaje mensaje)
    {
        _context.Mensajes.Add(mensaje);
        await _context.SaveChangesAsync();
        return mensaje;
    }

    public async Task<bool> MarcarLeido(int idMensaje, int idUsuario)
    {
        var mensaje = await _context.Mensajes
            .FirstOrDefaultAsync(m => m.IdMensaje == idMensaje &&
                                      m.IdUsuarioDestinat == idUsuario);
        if (mensaje is null) return false;

        mensaje.Leido = true;
        mensaje.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Eliminar(int idMensaje, int idUsuario)
    {
        var mensaje = await ObtenerPorId(idMensaje, idUsuario);
        if (mensaje is null) return false;

        mensaje.Activo = false;
        mensaje.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> ContarNoLeidos(int idUsuario)
    {
        return await _context.Mensajes
            .CountAsync(m => m.IdUsuarioDestinat == idUsuario &&
                             !m.Leido &&
                             m.Activo);
    }

    public Task<IQueryable<Mensaje>> ObtenerQueryableRecibidos(int idUsuario)
    {
        var query = _context.Mensajes
            .Include(m => m.Remitente)
            .Include(m => m.Destinatario)
            .Where(m => m.IdUsuarioDestinat == idUsuario && m.Activo)
            .OrderByDescending(m => m.FechaEnvio)
            .AsQueryable();

        return Task.FromResult(query);
    }

    public Task<IQueryable<Mensaje>> ObtenerQueryableEnviados(int idUsuario)
    {
        var query = _context.Mensajes
            .Include(m => m.Remitente)
            .Include(m => m.Destinatario)
            .Where(m => m.IdUsuarioRemitente == idUsuario && m.Activo)
            .OrderByDescending(m => m.FechaEnvio)
            .AsQueryable();

        return Task.FromResult(query);
    }
}