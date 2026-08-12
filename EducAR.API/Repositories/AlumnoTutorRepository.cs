using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AlumnoTutorRepository : IAlumnoTutorRepository
{
    private readonly AppDbContext _context;

    public AlumnoTutorRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AlumnoTutor>> ObtenerPorAlumno(int idAlumno)
    {
        return await _context.AlumnoTutores
            .Include(at => at.Alumno)
            .Include(at => at.Tutor).ThenInclude(t => t.Usuario)
            .Where(at => at.IdAlumno == idAlumno && at.Activo)
            .OrderByDescending(at => at.EsResponsablePrinc)
            .ToListAsync();
    }

    public async Task<List<AlumnoTutor>> ObtenerPorTutor(int idTutor)
    {
        return await _context.AlumnoTutores
            .Include(at => at.Alumno)
            .Include(at => at.Tutor).ThenInclude(t => t.Usuario)
            .Where(at => at.IdTutor == idTutor && at.Activo)
            .OrderBy(at => at.Alumno.Apellido)
            .ToListAsync();
    }

    public async Task<AlumnoTutor?> ObtenerPorId(int idAlumnoTutor)
    {
        return await _context.AlumnoTutores
            .Include(at => at.Alumno)
            .Include(at => at.Tutor).ThenInclude(t => t.Usuario)
            .FirstOrDefaultAsync(at => at.IdAlumnoTutor == idAlumnoTutor);
    }

    public async Task<bool> ExisteRelacion(int idAlumno, int idTutor)
    {
        return await _context.AlumnoTutores
            .AnyAsync(at => at.IdAlumno == idAlumno &&
                            at.IdTutor == idTutor &&
                            at.Activo);
    }

    public async Task<bool> TieneResponsablePrincipal(int idAlumno)
    {
        return await _context.AlumnoTutores
            .AnyAsync(at => at.IdAlumno == idAlumno &&
                            at.EsResponsablePrinc &&
                            at.Activo);
    }

    public async Task<AlumnoTutor> Crear(AlumnoTutor alumnoTutor)
    {
        _context.AlumnoTutores.Add(alumnoTutor);
        await _context.SaveChangesAsync();
        return alumnoTutor;
    }

    public async Task<bool> Eliminar(int idAlumnoTutor, int idEscuela)
    {
        var relacion = await ObtenerPorId(idAlumnoTutor);
        if (relacion is null) return false;

        // Verificar que pertenece a la escuela
        if (relacion.Alumno.IdEscuela != idEscuela) return false;

        relacion.Activo   = false;
        relacion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<AlumnoTutor?> ObtenerRelacionInactiva(int idAlumno, int idTutor)
    {
        return await _context.AlumnoTutores
            .FirstOrDefaultAsync(at => at.IdAlumno == idAlumno &&
                                    at.IdTutor == idTutor &&
                                    !at.Activo);
    }
    
    public async Task<AlumnoTutor> Reactivar(AlumnoTutor alumnoTutor)
    {
        alumnoTutor.Activo            = true;
        alumnoTutor.FechaAct          = DateTime.Now;
        _context.AlumnoTutores.Update(alumnoTutor);
        await _context.SaveChangesAsync();
        return alumnoTutor;
    }
}