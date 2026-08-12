using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class EscuelaRepository : IEscuelaRepository
{
    private readonly AppDbContext _context;

    public EscuelaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Escuela>> ObtenerTodas()
    {
        return await _context.Escuelas
            .OrderBy(e => e.Nombre)
            .ToListAsync();
    }

    public async Task<Escuela?> ObtenerPorId(int idEscuela)
    {
        return await _context.Escuelas
            .FirstOrDefaultAsync(e => e.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteNombre(string nombre, int? excluirId = null)
    {
        return await _context.Escuelas
            .AnyAsync(e => e.Nombre == nombre &&
                           (excluirId == null || e.IdEscuela != excluirId));
    }

    public async Task<Escuela> Crear(Escuela escuela)
    {
        _context.Escuelas.Add(escuela);
        await _context.SaveChangesAsync();
        return escuela;
    }

    public async Task<bool> Actualizar(Escuela escuela)
    {
        escuela.FechaAct = DateTime.Now;
        _context.Escuelas.Update(escuela);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public Task<IQueryable<Escuela>> ObtenerQueryable(string? nombre = null)
    {
        var query = _context.Escuelas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(e => e.Nombre.Contains(nombre));

        return Task.FromResult(query.OrderBy(e => e.Nombre).AsQueryable());
    }
}
