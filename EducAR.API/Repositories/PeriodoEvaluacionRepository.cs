using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class PeriodoEvaluacionRepository : IPeriodoEvaluacionRepository
{
    private readonly AppDbContext _context;

    public PeriodoEvaluacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PeriodoEvaluacion>> ObtenerTodos(int idCicloLectivo)
    {
        return await _context.PeriodosEvaluacion
            .Include(p => p.CicloLectivo)
            .Where(p => p.IdCicloLectivo == idCicloLectivo)
            .OrderBy(p => p.FechaInicio)
            .ToListAsync();
    }

    public async Task<PeriodoEvaluacion?> ObtenerPorId(int idPeriodo, int idCicloLectivo)
    {
        return await _context.PeriodosEvaluacion
            .Include(p => p.CicloLectivo)
            .FirstOrDefaultAsync(p => p.IdPeriodoEvaluacion == idPeriodo &&
                                      p.IdCicloLectivo == idCicloLectivo);
    }

    public async Task<bool> ExisteNombre(string nombre, int idCicloLectivo, int? excluirId = null)
    {
        return await _context.PeriodosEvaluacion
            .AnyAsync(p => p.Nombre == nombre &&
                           p.IdCicloLectivo == idCicloLectivo &&
                           (excluirId == null || p.IdPeriodoEvaluacion != excluirId));
    }

    public async Task<bool> TieneCalificaciones(int idPeriodo)
    {
        return await _context.Calificaciones
            .AnyAsync(c => c.IdPeriodoEvaluacion == idPeriodo);
    }

    public async Task<PeriodoEvaluacion> Crear(PeriodoEvaluacion periodo)
    {
        _context.PeriodosEvaluacion.Add(periodo);
        await _context.SaveChangesAsync();
        return periodo;
    }

    public async Task<bool> TieneDependencias(int idPeriodo) =>
        await TieneCalificaciones(idPeriodo) ||
        await _context.Evaluaciones.AnyAsync(e => e.IdPeriodoEvaluacion == idPeriodo && e.Activo) ||
        await _context.Boletines.AnyAsync(b => b.IdPeriodoEvaluacion == idPeriodo);

    public Task<bool> TieneEvaluacionesFueraDeFechas(int idPeriodo, DateTime inicio, DateTime fin) =>
        _context.Evaluaciones.AnyAsync(e => e.IdPeriodoEvaluacion == idPeriodo && e.Activo &&
            (e.Fecha < inicio.Date || e.Fecha >= fin.Date.AddDays(1)));

    public async Task<bool> Actualizar(PeriodoEvaluacion periodo)
    {
        periodo.FechaAct = DateTime.Now;
        _context.PeriodosEvaluacion.Update(periodo);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<(bool exito, string mensaje)> Eliminar(int idPeriodo, int idCicloLectivo)
    {
        var periodo = await ObtenerPorId(idPeriodo, idCicloLectivo);
        if (periodo is null)
            return (false, "Período de evaluación no encontrado.");

        if (await TieneDependencias(idPeriodo))
            return (false, "No se puede dar de baja el período porque tiene evaluaciones, calificaciones o boletines asociados.");

        periodo.Activo = false;
        periodo.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return (true, "Período de evaluación dado de baja correctamente.");
    }
}
