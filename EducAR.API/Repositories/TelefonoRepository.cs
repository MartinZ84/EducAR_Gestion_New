using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using EducAR.API.Data;

namespace EducAR.API.Repositories;

public class TelefonoRepository : ITelefonoRepository
{
    private readonly AppDbContext _context;

    public TelefonoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TelefonoContacto>> ObtenerPorAlumnoAsync(int idAlumno)
    {
        return await _context.TelefonosContacto
            .Where(t => t.IdAlumno == idAlumno)
            .OrderByDescending(t => t.EsPrincipal)
            .ThenBy(t => t.Numero)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TelefonoContacto?> ObtenerPorIdAsync(int idTelefono)
    {
        return await _context.TelefonosContacto
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.IdTelefono == idTelefono);
    }

    public async Task<TelefonoContacto> CrearAsync(TelefonoContacto telefono)
    {
        _context.TelefonosContacto.Add(telefono);
        await _context.SaveChangesAsync();
        return telefono;
    }

    public async Task<TelefonoContacto?> ActualizarAsync(TelefonoContacto telefono)
    {
        var existente = await _context.TelefonosContacto.FindAsync(telefono.IdTelefono);
        if (existente is null) return null;
        existente.Numero = telefono.Numero;
        existente.Des = telefono.Des;
        existente.Tipo = telefono.Tipo;
        existente.EsPrincipal = telefono.EsPrincipal;
        await _context.SaveChangesAsync();
        return existente;
    }

    public async Task EliminarAsync(int idTelefono)
    {
        var telefono = await _context.TelefonosContacto.FindAsync(idTelefono);
        if (telefono != null)
        {
            _context.TelefonosContacto.Remove(telefono);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExisteAsync(int idTelefono)
    {
        return await _context.TelefonosContacto.AnyAsync(t => t.IdTelefono == idTelefono);
    }
}