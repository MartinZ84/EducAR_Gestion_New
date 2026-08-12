using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AlumnoRepository : IAlumnoRepository
{
    private readonly AppDbContext _context;

    public AlumnoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Alumno>> ObtenerTodos(int idEscuela)
    {
        return await _context.Alumnos
            .Where(a => a.IdEscuela == idEscuela)
            .OrderBy(a => a.Apellido)
            .ToListAsync();
    }

    public async Task<Alumno?> ObtenerPorId(int idAlumno, int idEscuela)
    {
        return await _context.Alumnos
            .Include(a => a.Matriculas)
                .ThenInclude(m => m.Curso)
                    .ThenInclude(c => c.CicloLectivo)
            .Include(a => a.AlumnoTutores)
                .ThenInclude(at => at.Tutor)
                    .ThenInclude(t => t.Usuario)
            .FirstOrDefaultAsync(a => a.IdAlumno == idAlumno && a.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdAlumno = null)
    {
        return await _context.Alumnos
            .AnyAsync(a => a.Dni == dni
                        && a.IdEscuela == idEscuela
                        && (excluirIdAlumno == null || a.IdAlumno != excluirIdAlumno));
    }

    public async Task<Alumno> Crear(Alumno alumno)
    {
        _context.Alumnos.Add(alumno);
        await _context.SaveChangesAsync();
        return alumno;
    }

    public async Task<bool> Actualizar(Alumno alumno)
    {
        alumno.FechaAct = DateTime.Now;
        _context.Alumnos.Update(alumno);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idAlumno, int idEscuela)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == idAlumno && a.IdEscuela == idEscuela);
        if (alumno is null) return false;

        alumno.Activo = false;
        alumno.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    // ==========================
    // TUTORES
    // ==========================

    public async Task<bool> ExisteTutorEnEscuela(int idTutor, int idEscuela)
    {
        return await _context.Tutores
            .AnyAsync(t => t.IdTutor == idTutor && t.Usuario.IdEscuela == idEscuela && t.Usuario.Activo);
    }

    public async Task<AlumnoTutor?> ObtenerAsociacionTutor(int idAlumno, int idTutor)
    {
        return await _context.AlumnoTutores
            .Include(at => at.Tutor)
                .ThenInclude(t => t.Usuario)
            .FirstOrDefaultAsync(at => at.IdAlumno == idAlumno && at.IdTutor == idTutor);
    }

    public async Task AsociarTutor(AlumnoTutor alumnoTutor)
    {
        _context.AlumnoTutores.Add(alumnoTutor);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> QuitarTutor(int idAlumno, int idTutor)
    {
        var asociacion = await _context.AlumnoTutores
            .FirstOrDefaultAsync(at => at.IdAlumno == idAlumno && at.IdTutor == idTutor && at.Activo);
        if (asociacion is null) return false;

        asociacion.Activo = false;
        asociacion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> GuardarCambios()
    {
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public Task<IQueryable<Alumno>> ObtenerQueryable(int idEscuela, string? nombre = null, string? apellido = null, int? dni = null)
    {
        var query = _context.Alumnos
            .Where(a => a.IdEscuela == idEscuela && a.Activo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(a => a.Nombre.Contains(nombre));

        if (!string.IsNullOrWhiteSpace(apellido))
            query = query.Where(a => a.Apellido.Contains(apellido));

        if (dni.HasValue)
            query = query.Where(a => a.Dni == dni.Value);

        return Task.FromResult(query.OrderBy(a => a.Apellido).AsQueryable());
    }
}
