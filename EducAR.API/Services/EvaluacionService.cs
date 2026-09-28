using EducAR.API.Data;
using EducAR.API.DTOs.Evaluaciones;
using EducAR.API.Models;
using EducAR.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Services;

public class EvaluacionService(AppDbContext context) : IEvaluacionService
{
    private IQueryable<DocenteMateriaCurso> Asignaciones(AccesoEvaluacion acceso) =>
        context.DocenteMateriaCursos.Where(a => a.Activo && a.Curso.Activo && a.Materia.Activo &&
            a.Curso.IdEscuela == acceso.IdEscuela && a.Materia.IdEscuela == acceso.IdEscuela &&
            a.Curso.CicloLectivo.IdEscuela == acceso.IdEscuela &&
            a.Docente.Usuario.Activo && a.Docente.Usuario.IdEscuela == acceso.IdEscuela &&
            (acceso.EsAdmin || a.Docente.IdUsuario == acceso.IdUsuario));

    private Task<bool> PuedeAcceder(int curso, int materia, AccesoEvaluacion acceso) =>
        Asignaciones(acceso).AnyAsync(a => a.IdCurso == curso && a.IdMateria == materia);

    private IQueryable<Evaluacion> Evaluaciones(AccesoEvaluacion acceso) => context.Evaluaciones
        .Where(e => e.Curso.IdEscuela == acceso.IdEscuela && e.Materia.IdEscuela == acceso.IdEscuela)
        .Include(e => e.Curso).ThenInclude(c => c.CicloLectivo)
        .Include(e => e.PeriodoEvaluacion);

    private static ResultadoEvaluacion Error(string mensaje, int estado = 400) => new(estado, new { mensaje });

    public async Task<ResultadoEvaluacion> Opciones(AccesoEvaluacion acceso) => new(200,
        await Asignaciones(acceso).Select(a => new
        {
            a.IdCurso, a.Curso.Grado, a.Curso.Division, a.Curso.Turno,
            a.Curso.IdCicloLectivo, a.Curso.CicloLectivo.Anio,
            a.IdMateria, NombreMateria = a.Materia.Nombre
        }).Distinct().OrderBy(a => a.Anio).ThenBy(a => a.Grado).ThenBy(a => a.Division).ToListAsync());

    public async Task<ResultadoEvaluacion> Obtener(int idCurso, int idMateria, int idPeriodo, AccesoEvaluacion acceso)
    {
        if (!await PuedeAcceder(idCurso, idMateria, acceso)) return new(403);
        return new(200, await Evaluaciones(acceso).Where(e => e.IdCurso == idCurso &&
            e.IdMateria == idMateria && e.IdPeriodoEvaluacion == idPeriodo && e.Activo)
            .OrderBy(e => e.Fecha).ThenBy(e => e.IdEvaluacion).Select(e => new
            {
                e.IdEvaluacion, e.IdCurso, e.IdMateria, e.IdPeriodoEvaluacion,
                e.Curso.IdCicloLectivo, e.Curso.CicloLectivo.Anio,
                e.Titulo, e.Temario, e.Descripcion, e.Fecha,
                Notas = e.Notas.Select(n => new { n.IdAlumno, n.Valor }).ToList()
            }).ToListAsync());
    }

    private static string? ValidarTexto(EvaluacionEditarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo) || dto.Titulo.Length > 200)
            return "Ingresá un título de hasta 200 caracteres.";
        if (string.IsNullOrWhiteSpace(dto.Temario) || dto.Temario.Length > 4000)
            return "Ingresá un temario de hasta 4000 caracteres.";
        if (string.IsNullOrWhiteSpace(dto.Descripcion) || dto.Descripcion.Length > 4000)
            return "Ingresá una descripción de hasta 4000 caracteres.";
        return null;
    }

    private static void Aplicar(Evaluacion e, EvaluacionEditarDto dto)
    {
        e.Titulo = dto.Titulo.Trim(); e.Temario = dto.Temario.Trim();
        e.Descripcion = dto.Descripcion.Trim(); e.Fecha = dto.Fecha.Date;
    }

    public async Task<ResultadoEvaluacion> Crear(EvaluacionCrearDto dto, AccesoEvaluacion acceso)
    {
        if (!await PuedeAcceder(dto.IdCurso, dto.IdMateria, acceso)) return new(403);
        var error = ValidarTexto(dto);
        if (error is not null) return Error(error);
        var periodo = await context.PeriodosEvaluacion.FirstOrDefaultAsync(p =>
            p.IdPeriodoEvaluacion == dto.IdPeriodoEvaluacion && p.Activo &&
            p.CicloLectivo.Activo && p.CicloLectivo.IdEscuela == acceso.IdEscuela &&
            p.CicloLectivo.Cursos.Any(c => c.IdCurso == dto.IdCurso));
        if (periodo is null) return Error("El período debe estar activo y corresponder al ciclo lectivo del curso.");
        if (dto.Fecha.Date < periodo.FechaInicio.Date || dto.Fecha.Date > periodo.FechaFin.Date)
            return Error("La fecha debe estar dentro del período.");
        var evaluacion = new Evaluacion { IdCurso = dto.IdCurso, IdMateria = dto.IdMateria, IdPeriodoEvaluacion = dto.IdPeriodoEvaluacion };
        Aplicar(evaluacion, dto);
        context.Evaluaciones.Add(evaluacion);
        await context.SaveChangesAsync();
        return new(200, new { evaluacion.IdEvaluacion });
    }

    public async Task<ResultadoEvaluacion> Editar(int id, EvaluacionEditarDto dto, AccesoEvaluacion acceso)
    {
        var e = await Evaluaciones(acceso).FirstOrDefaultAsync(e => e.IdEvaluacion == id && e.Activo);
        if (e is null) return Error("Evaluación no encontrada.", 404);
        if (!await PuedeAcceder(e.IdCurso, e.IdMateria, acceso)) return new(403);
        var error = ValidarTexto(dto);
        if (error is not null) return Error(error);
        if (!e.PeriodoEvaluacion.Activo || !e.Curso.CicloLectivo.Activo)
            return Error("El período o ciclo lectivo está inactivo.");
        if (dto.Fecha.Date < e.PeriodoEvaluacion.FechaInicio.Date || dto.Fecha.Date > e.PeriodoEvaluacion.FechaFin.Date)
            return Error("La fecha debe estar dentro del período.");
        Aplicar(e, dto);
        await context.SaveChangesAsync();
        return new(200, new { mensaje = "Evaluación actualizada." });
    }

    public async Task<ResultadoEvaluacion> Archivar(int id, AccesoEvaluacion acceso)
    {
        var e = await Evaluaciones(acceso).FirstOrDefaultAsync(e => e.IdEvaluacion == id && e.Activo);
        if (e is null) return Error("Evaluación no encontrada.", 404);
        if (!await PuedeAcceder(e.IdCurso, e.IdMateria, acceso)) return new(403);
        if (await context.NotasEvaluacion.AnyAsync(n => n.IdEvaluacion == id))
            return Error("No se puede archivar una evaluación con notas registradas.");
        e.Activo = false;
        await context.SaveChangesAsync();
        return new(200, new { mensaje = "Evaluación archivada." });
    }

    private IQueryable<Matricula> Matriculas(Evaluacion e, AccesoEvaluacion acceso) => context.Matriculas.Where(m =>
        m.IdCurso == e.IdCurso && m.IdCicloLectivo == e.Curso.IdCicloLectivo &&
        m.IdEscuela == acceso.IdEscuela && m.Alumno.IdEscuela == acceso.IdEscuela &&
        m.Estado == EstadoMatricula.Activa);

    public async Task<ResultadoEvaluacion> ObtenerAlumnos(int id, AccesoEvaluacion acceso)
    {
        var e = await Evaluaciones(acceso).FirstOrDefaultAsync(e => e.IdEvaluacion == id && e.Activo);
        if (e is null) return Error("Evaluación no encontrada.", 404);
        if (!await PuedeAcceder(e.IdCurso, e.IdMateria, acceso)) return new(403);
        return new(200, await Matriculas(e, acceso).OrderBy(m => m.Alumno.Apellido).ThenBy(m => m.Alumno.Nombre)
            .Select(m => new
            {
                m.IdAlumno, m.Alumno.Nombre, m.Alumno.Apellido,
                Valor = context.NotasEvaluacion.Where(n => n.IdEvaluacion == id && n.IdAlumno == m.IdAlumno)
                    .Select(n => (decimal?)n.Valor).FirstOrDefault()
            }).ToListAsync());
    }

    public async Task<ResultadoEvaluacion> GuardarNotas(int id, NotasEvaluacionGuardarDto dto, AccesoEvaluacion acceso)
    {
        var e = await Evaluaciones(acceso).FirstOrDefaultAsync(e => e.IdEvaluacion == id && e.Activo);
        if (e is null) return Error("Evaluación no encontrada.", 404);
        if (!await PuedeAcceder(e.IdCurso, e.IdMateria, acceso)) return new(403);
        if (!e.PeriodoEvaluacion.Activo || !e.Curso.CicloLectivo.Activo ||
            e.PeriodoEvaluacion.IdCicloLectivo != e.Curso.IdCicloLectivo)
            return Error("El período y el curso deben pertenecer al mismo ciclo lectivo activo.");
        if (dto.Notas is null || dto.Notas.Count == 0) return Error("Ingresá al menos una nota o modificación.");
        var matriculados = await Matriculas(e, acceso).Select(m => m.IdAlumno).ToListAsync();
        var errores = new List<ErrorNota>();
        foreach (var grupo in dto.Notas.GroupBy(n => n.IdAlumno))
        {
            if (grupo.Count() > 1) errores.Add(new(grupo.Key, "El alumno está duplicado en la solicitud."));
            if (!matriculados.Contains(grupo.Key)) errores.Add(new(grupo.Key, "El alumno no tiene matrícula activa en este curso y año lectivo."));
            if (grupo.Any(n => n.Valor is < 1 or > 10 || (n.Valor.HasValue && decimal.Round(n.Valor.Value, 2) != n.Valor)))
                errores.Add(new(grupo.Key, "La nota debe estar entre 1 y 10, con hasta dos decimales."));
        }
        if (errores.Count > 0) return new(400, new { mensaje = "No se guardó ninguna nota. Revisá las filas indicadas.", errores });

        await using var transaccion = context.Database.IsRelational()
            ? await context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable) : null;
        try
        {
            var existentes = await context.NotasEvaluacion.Where(n => n.IdEvaluacion == id).ToListAsync();
            var resultados = new List<NotaGuardada>();
            foreach (var nota in dto.Notas)
            {
                var existente = existentes.FirstOrDefault(n => n.IdAlumno == nota.IdAlumno);
                var accion = "sin cambios";
                if (nota.Valor is null)
                {
                    if (existente is not null) { context.NotasEvaluacion.Remove(existente); accion = "pendiente"; }
                }
                else if (existente is null)
                {
                    context.NotasEvaluacion.Add(new NotaEvaluacion { IdEvaluacion = id, IdAlumno = nota.IdAlumno, Valor = nota.Valor.Value });
                    accion = "registrada";
                }
                else if (existente.Valor != nota.Valor.Value)
                {
                    existente.Valor = nota.Valor.Value; existente.FechaAct = DateTime.Now; accion = "actualizada";
                }
                resultados.Add(new(nota.IdAlumno, nota.Valor, accion));
            }
            await context.SaveChangesAsync();
            var conservadas = await ActualizarPromedios(e, dto.Notas.Select(n => n.IdAlumno).ToList());
            await context.SaveChangesAsync();
            if (transaccion is not null) await transaccion.CommitAsync();
            return new(200, new ResultadoNotas("Calificaciones guardadas.", resultados, conservadas));
        }
        catch (DbUpdateException)
        {
            if (transaccion is not null) await transaccion.RollbackAsync();
            return Error("No se guardó el lote por un conflicto de datos. Volvé a consultar la evaluación antes de reintentar.", 409);
        }
    }

    private async Task<List<int>> ActualizarPromedios(Evaluacion e, List<int> ids)
    {
        var promedios = await context.NotasEvaluacion.Where(n => n.Evaluacion.IdCurso == e.IdCurso &&
            n.Evaluacion.IdMateria == e.IdMateria && n.Evaluacion.IdPeriodoEvaluacion == e.IdPeriodoEvaluacion &&
            n.Evaluacion.Activo && ids.Contains(n.IdAlumno)).GroupBy(n => n.IdAlumno)
            .Select(g => new { IdAlumno = g.Key, Valor = g.Average(n => n.Valor) }).ToListAsync();
        var finales = await context.Calificaciones.Where(c => c.IdMateria == e.IdMateria &&
            c.IdPeriodoEvaluacion == e.IdPeriodoEvaluacion && ids.Contains(c.IdAlumno)).ToListAsync();
        var conservadas = new List<int>();
        foreach (var idAlumno in ids)
        {
            var final = finales.FirstOrDefault(c => c.IdAlumno == idAlumno);
            // Los registros previos no tienen procedencia verificable: nunca sobrescribirlos.
            if (final is not null && !final.GeneradaPorEvaluaciones) { conservadas.Add(idAlumno); continue; }
            var promedio = promedios.FirstOrDefault(p => p.IdAlumno == idAlumno);
            if (promedio is null)
            {
                if (final is not null) { final.Activo = false; final.FechaAct = DateTime.Now; }
                continue;
            }
            if (final is null)
            {
                final = new Calificacion { IdAlumno = idAlumno, IdMateria = e.IdMateria,
                    IdPeriodoEvaluacion = e.IdPeriodoEvaluacion, GeneradaPorEvaluaciones = true };
                context.Calificaciones.Add(final);
            }
            final.ValorCalificacion = Math.Round(promedio.Valor, 2, MidpointRounding.AwayFromZero);
            final.Activo = true; final.FechaAct = DateTime.Now;
        }
        return conservadas;
    }
}
