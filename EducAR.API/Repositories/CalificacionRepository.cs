using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class CalificacionRepository : ICalificacionRepository
{
    private readonly AppDbContext _context;

    public CalificacionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Calificacion>> ObtenerPorCursoMateriaYPeriodo(int idCurso, int idMateria, int idPeriodo)
    {
        // Obtenemos los alumnos del curso y cruzamos con calificaciones
        return await _context.Calificaciones
            .Include(c => c.Alumno)
            .Include(c => c.Materia)
            .Include(c => c.PeriodoEvaluacion)
            .Where(c => c.IdMateria == idMateria &&
                        c.IdPeriodoEvaluacion == idPeriodo &&
                        _context.Matriculas.Any(m => m.IdAlumno == c.IdAlumno &&
                                                       m.IdCurso == idCurso &&
                                                       m.Estado == EstadoMatricula.Activa))
            .OrderBy(c => c.Alumno.Apellido)
            .ToListAsync();
    }

    public async Task<List<Calificacion>> ObtenerPorAlumno(int idAlumno, int idPeriodo)
    {
        return await _context.Calificaciones
            .Include(c => c.Materia)
            .Include(c => c.PeriodoEvaluacion)
            .Where(c => c.IdAlumno == idAlumno && c.IdPeriodoEvaluacion == idPeriodo)
            .OrderBy(c => c.Materia.Nombre)
            .ToListAsync();
    }

    public async Task<Calificacion?> ObtenerPorAlumnoMateriaYPeriodo(int idAlumno, int idMateria, int idPeriodo)
    {
        return await _context.Calificaciones
            .FirstOrDefaultAsync(c => c.IdAlumno == idAlumno &&
                                      c.IdMateria == idMateria &&
                                      c.IdPeriodoEvaluacion == idPeriodo);
    }

    public async Task RegistrarLote(List<Calificacion> calificaciones)
    {
        _context.Calificaciones.AddRange(calificaciones);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarLote(List<Calificacion> calificaciones)
    {
        _context.Calificaciones.UpdateRange(calificaciones);
        await _context.SaveChangesAsync();
    }
}