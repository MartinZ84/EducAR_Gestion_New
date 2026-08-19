using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class DocenteRepository : IDocenteRepository
{
    private readonly AppDbContext _context;

    public DocenteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Docente>> ObtenerTodos(int idEscuela)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .Where(d => d.Usuario.IdEscuela == idEscuela)
            .OrderBy(d => d.Usuario.Apellido)
            .ToListAsync();
    }

    public async Task<Docente?> ObtenerPorId(int idDocente, int idEscuela)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.IdDocente == idDocente && d.Usuario.IdEscuela == idEscuela);
    }

    public async Task<Docente> Crear(Docente docente)
    {
        _context.Docentes.Add(docente);
        await _context.SaveChangesAsync();
        return docente;
    }

    public async Task<bool> Actualizar(Docente docente)
    {
        docente.FechaAct = DateTime.Now;
        _context.Docentes.Update(docente);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idDocente, int idEscuela)
    {
        var docente = await ObtenerPorId(idDocente, idEscuela);
        if (docente is null) return false;

        docente.Activo = false;
        docente.Usuario.Activo = false;
        docente.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Docente?> ObtenerPorUsuarioInactivo(int idUsuario)
    {
        return await _context.Docentes
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.IdUsuario == idUsuario && !d.Activo);
    }

    public async Task<Docente> Reactivar(Docente docente)
    {
        docente.Activo = true;
        docente.FechaAct = DateTime.Now;
        _context.Docentes.Update(docente);
        await _context.SaveChangesAsync();
        return docente;
    }

    public Task<IQueryable<Docente>> ObtenerQueryable(int idEscuela, string? nombre = null, string? apellido = null, int? dni = null)
    {
        var query = _context.Docentes
            .Include(d => d.Usuario)
            .Where(d => d.Usuario.IdEscuela == idEscuela && d.Activo)
            .AsQueryable();

       var texto = nombre;

        if (!string.IsNullOrWhiteSpace(texto))
        {
            query = query.Where(a =>
                a.Usuario.Nombre.Contains(texto) ||
                a.Usuario.Apellido.Contains(texto));
        }

        if (dni.HasValue)
            query = query.Where(d => d.Usuario.Dni == dni.Value);

        return Task.FromResult(query.OrderBy(d => d.Usuario.Apellido).AsQueryable());
    }
}
