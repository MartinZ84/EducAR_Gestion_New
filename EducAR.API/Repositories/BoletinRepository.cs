using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class BoletinRepository : IBoletinRepository
{
    private readonly AppDbContext _context;

    public BoletinRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Boletin>> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo)
    {
        return await _context.Boletines
            .Include(b => b.Alumno)
            .Include(b => b.Curso).ThenInclude(c => c.CicloLectivo)
            .Include(b => b.PeriodoEvaluacion)
            .Include(b => b.DetallesBoletines).ThenInclude(d => d.Materia)
            .Where(b => b.IdCurso == idCurso && b.IdPeriodoEvaluacion == idPeriodo)
            .OrderBy(b => b.Alumno.Apellido)
            .ToListAsync();
    }

    public async Task<Boletin?> ObtenerPorAlumnoCursoYPeriodo(int idAlumno, int idCurso, int idPeriodo)
    {
        return await _context.Boletines
            .Include(b => b.Alumno)
            .Include(b => b.Curso).ThenInclude(c => c.CicloLectivo)
            .Include(b => b.PeriodoEvaluacion)
            .Include(b => b.DetallesBoletines).ThenInclude(d => d.Materia)
            .FirstOrDefaultAsync(b => b.IdAlumno == idAlumno &&
                                      b.IdCurso == idCurso &&
                                      b.IdPeriodoEvaluacion == idPeriodo);
    }

    public async Task<Boletin?> ObtenerPorId(int idBoletin)
    {
        return await _context.Boletines
            .Include(b => b.Alumno)
            .Include(b => b.Curso).ThenInclude(c => c.CicloLectivo)
            .Include(b => b.PeriodoEvaluacion)
            .Include(b => b.DetallesBoletines).ThenInclude(d => d.Materia)
            .FirstOrDefaultAsync(b => b.IdBoletin == idBoletin);
    }

    public async Task<Boletin> Crear(Boletin boletin)
    {
        // Separamos los detalles para insertarlos después
        var detalles = boletin.DetallesBoletines.ToList();
        boletin.DetallesBoletines = new List<DetalleBoletin>();

        // Primero insertamos el boletín para obtener el IdBoletin
        _context.Boletines.Add(boletin);
        await _context.SaveChangesAsync();

        // Ahora asignamos el IdBoletin a cada detalle y los insertamos
        foreach (var detalle in detalles)
        {
            detalle.IdBoletin = boletin.IdBoletin;
        }

        _context.DetallesBoletines.AddRange(detalles);
        await _context.SaveChangesAsync();

        // Recargamos el boletín completo con los detalles
        return (await ObtenerPorId(boletin.IdBoletin))!;
    }
    public async Task<bool> Actualizar(Boletin boletin)
    {
        boletin.FechaAct = DateTime.Now;

        // Eliminar detalles viejos y reemplazar por los nuevos
        var detallesViejos = await _context.DetallesBoletines
            .Where(d => d.IdBoletin == boletin.IdBoletin)
            .ToListAsync();

        _context.DetallesBoletines.RemoveRange(detallesViejos);
        await _context.SaveChangesAsync();

        var detallesNuevos = boletin.DetallesBoletines.ToList();
        foreach (var detalle in detallesNuevos)
        {
            detalle.IdBoletin = boletin.IdBoletin;
        }

        _context.DetallesBoletines.AddRange(detallesNuevos);
        _context.Boletines.Update(boletin);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }
}