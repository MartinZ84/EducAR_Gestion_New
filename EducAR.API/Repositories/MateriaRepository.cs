using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class MateriaRepository : IMateriaRepository
{
    private readonly AppDbContext _context;

    public MateriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Materia>> ObtenerTodas(int idEscuela)
    {
        return await _context.Materias
            .Where(m => m.IdEscuela == idEscuela)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Materia?> ObtenerPorId(int idMateria, int idEscuela)
    {
        return await _context.Materias
            .FirstOrDefaultAsync(m => m.IdMateria == idMateria && m.IdEscuela == idEscuela);
    }

    public async Task<Materia?> ObtenerDetalle(int idMateria, int idEscuela)
    {
        return await _context.Materias.AsNoTracking()
            .Include(m => m.DocenteMateriaCursos).ThenInclude(dmc => dmc.Docente).ThenInclude(d => d.Usuario)
            .Include(m => m.DocenteMateriaCursos).ThenInclude(dmc => dmc.Curso).ThenInclude(c => c.CicloLectivo)
            .FirstOrDefaultAsync(m => m.IdMateria == idMateria && m.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteNombre(string nombre, int idEscuela, int? excluirId = null)
    {
        return await _context.Materias
            .AnyAsync(m => m.Nombre == nombre &&
                           m.IdEscuela == idEscuela &&
                           (excluirId == null || m.IdMateria != excluirId));
    }

    public async Task<Materia> Crear(Materia materia)
    {
        _context.Materias.Add(materia);
        await _context.SaveChangesAsync();
        return materia;
    }

    public async Task<bool> Actualizar(Materia materia)
    {
        materia.FechaAct = DateTime.Now;
        _context.Materias.Update(materia);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idMateria, int idEscuela)
    {
        var materia = await ObtenerPorId(idMateria, idEscuela);
        if (materia is null) return false;

        materia.Activo = false;
        materia.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IQueryable<Materia>> ObtenerQueryable(int idEscuela, string? nombre = null)
    {
        var query = _context.Materias
            .Where(m => m.IdEscuela == idEscuela)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(m => m.Nombre.Contains(nombre));

        return Task.FromResult(query.OrderBy(m => m.Nombre).AsQueryable());
    }
}
