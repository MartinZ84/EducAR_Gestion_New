using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class CicloLectivoRepository : ICicloLectivoRepository
{
    private readonly AppDbContext _context;

    public CicloLectivoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CicloLectivo>> ObtenerTodos(int idEscuela)
    {
        return await _context.CiclosLectivos
            .Where(c => c.IdEscuela == idEscuela)
            .OrderByDescending(c => c.Anio)
            .ToListAsync();
    }

    public async Task<CicloLectivo?> ObtenerPorId(int idCicloLectivo, int idEscuela)
    {
        return await _context.CiclosLectivos
            .FirstOrDefaultAsync(c => c.IdCicloLectivo == idCicloLectivo && c.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteAnio(int anio, int idEscuela, int? excluirId = null)
    {
        return await _context.CiclosLectivos
            .AnyAsync(c => c.Anio == anio &&
                           c.IdEscuela == idEscuela &&
                           (excluirId == null || c.IdCicloLectivo != excluirId));
    }

    public async Task<CicloLectivo> Crear(CicloLectivo cicloLectivo)
    {
        _context.CiclosLectivos.Add(cicloLectivo);
        await _context.SaveChangesAsync();
        return cicloLectivo;
    }

    public async Task<bool> Actualizar(CicloLectivo cicloLectivo)
    {
        cicloLectivo.FechaAct = DateTime.Now;
        _context.CiclosLectivos.Update(cicloLectivo);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idCicloLectivo, int idEscuela)
    {
        var ciclo = await ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null) return false;

        ciclo.Activo = false;
        ciclo.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}