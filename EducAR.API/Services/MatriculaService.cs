using EducAR.API.Data;
using EducAR.API.DTOs.Matriculas;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Services;

public class MatriculaService : IMatriculaService
{
    private readonly IMatriculaRepository _matriculaRepository;
    private readonly IAlumnoRepository _alumnoRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly ICicloLectivoRepository _cicloLectivoRepository;

    public MatriculaService(
        IMatriculaRepository matriculaRepository,
        IAlumnoRepository alumnoRepository,
        ICursoRepository cursoRepository,
        ICicloLectivoRepository cicloLectivoRepository)
    {
        _matriculaRepository = matriculaRepository;
        _alumnoRepository = alumnoRepository;
        _cursoRepository = cursoRepository;
        _cicloLectivoRepository = cicloLectivoRepository;
    }

    public async Task<List<MatriculaResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<MatriculaResponseDto>();
        var matriculas = await _matriculaRepository.ObtenerPorCurso(idCurso, idEscuela);
        return matriculas.Select(Mapear).ToList();
    }

    public async Task<List<MatriculaResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela)
    {
        var matriculas = await _matriculaRepository.ObtenerPorAlumno(idAlumno, idEscuela);
        return matriculas.Select(Mapear).ToList();
    }

    public async Task<MatriculaResponseDto?> ObtenerPorId(int idMatricula, int idEscuela)
    {
        var matricula = await _matriculaRepository.ObtenerPorId(idMatricula, idEscuela);
        return matricula is null ? null : Mapear(matricula);
    }

    public async Task<(bool exito, string mensaje, MatriculaResponseDto? matricula)> Crear(MatriculaCreateDto dto, int idEscuela)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(dto.IdAlumno, idEscuela);
        if (alumno is null || !alumno.Activo)
            return (false, "El alumno no existe o está inactivo.", null);

        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe o no pertenece a esta escuela.", null);

        if (!curso.Activo)
            return (false, "No se puede matricular un alumno en un curso inactivo.", null);

        var ciclo = await _cicloLectivoRepository.ObtenerPorId(curso.IdCicloLectivo, idEscuela);
        if (ciclo is null || !ciclo.Activo)
            return (false, "El ciclo lectivo del curso no existe o está inactivo.", null);

        var existente = await _matriculaRepository.ObtenerPorAlumnoYCiclo(dto.IdAlumno, curso.IdCicloLectivo, idEscuela);
        if (existente is not null)
        {
            if (existente.Estado == EstadoMatricula.Activa && existente.IdCurso == dto.IdCurso)
                return (false, "El alumno ya está matriculado en este curso.", null);

            if (existente.Estado == EstadoMatricula.Activa)
                return (false, $"El alumno ya está matriculado en otro curso del ciclo lectivo {curso.CicloLectivo.Anio}: {existente.Curso.Grado}° {existente.Curso.Division}.", null);

            existente.IdCurso = dto.IdCurso;
            existente.Estado = EstadoMatricula.Activa;
            existente.FechaBaja = null;
            existente.FechaMatricula = DateTime.Now;
            await _matriculaRepository.Actualizar(existente);
            var reactivada = await _matriculaRepository.ObtenerPorId(existente.IdMatricula, idEscuela);
            return (true, "Matrícula reactivada correctamente.", Mapear(reactivada!));
        }

        var nueva = new Matricula
        {
            IdEscuela = idEscuela,
            IdAlumno = dto.IdAlumno,
            IdCurso = dto.IdCurso,
            IdCicloLectivo = curso.IdCicloLectivo,
            FechaMatricula = DateTime.Now,
            Estado = EstadoMatricula.Activa
        };

        var creada = await _matriculaRepository.Crear(nueva);
        var completa = await _matriculaRepository.ObtenerPorId(creada.IdMatricula, idEscuela);
        return (true, "Alumno matriculado correctamente.", Mapear(completa!));
    }

    public async Task<(bool exito, string mensaje)> DarDeBaja(int idMatricula, int idEscuela)
    {
        var exito = await _matriculaRepository.DarDeBaja(idMatricula, idEscuela);
        return exito
            ? (true, "Matrícula dada de baja correctamente.")
            : (false, "La matrícula no existe o no está activa.");
    }

    public async Task<ResultadoPaginadoDto<MatriculaAlumnoDisponibleDto>> ObtenerAlumnosParaMatricular(
        int idEscuela, int anioRegistro, int idCicloLectivo, PaginacionDto paginacion, string? nombre, string? apellido, int? dni)
    {
        var ciclo = await _cicloLectivoRepository.ObtenerPorId(idCicloLectivo, idEscuela);
        if (ciclo is null)
            return new ResultadoPaginadoDto<MatriculaAlumnoDisponibleDto>();

        var query = await _matriculaRepository.ObtenerAlumnosParaMatricular(idEscuela, anioRegistro, idCicloLectivo);

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(a => a.Nombre.Contains(nombre));
        if (!string.IsNullOrWhiteSpace(apellido))
            query = query.Where(a => a.Apellido.Contains(apellido));
        if (dni.HasValue)
            query = query.Where(a => a.Dni == dni.Value);

        var totalRegistros = await query.CountAsync();
        var totalPaginas = (int)Math.Ceiling((double)totalRegistros / paginacion.Cantidad);

        var alumnos = await query
            .OrderBy(a => a.Apellido)
            .ThenBy(a => a.Nombre)
            .ThenBy(a => a.Dni)
            .ThenBy(a => a.FecNac)
            .Skip((paginacion.Pagina - 1) * paginacion.Cantidad)
            .Take(paginacion.Cantidad)
            .ToListAsync();

        var idsPagina = alumnos.Select(a => a.IdAlumno).ToList();
        var listaMatriculas = await _matriculaRepository.ObtenerMatriculasPorAlumnosYCiclo(idsPagina, idCicloLectivo, idEscuela);
        var mapa = listaMatriculas.ToDictionary(m => m.IdAlumno);

        var datos = alumnos.Select(a =>
        {
            mapa.TryGetValue(a.IdAlumno, out var matricula);
            var activa = matricula?.Estado == EstadoMatricula.Activa;
            return new MatriculaAlumnoDisponibleDto
            {
                IdAlumno = a.IdAlumno,
                Dni = a.Dni,
                Nombre = a.Nombre,
                Apellido = a.Apellido,
                FecNac = a.FecNac,
                Matriculado = activa,
                IdMatricula = matricula?.IdMatricula,
                IdCursoActual = activa ? matricula!.IdCurso : null,
                CursoActual = activa ? $"{matricula!.Curso.Grado}° {matricula.Curso.Division} - {matricula.CicloLectivo.Anio}" : null,
                EstadoMatricula = matricula?.Estado.ToString()
            };
        }).ToList();

        return new ResultadoPaginadoDto<MatriculaAlumnoDisponibleDto>
        {
            PaginaActual = paginacion.Pagina,
            TotalPaginas = totalPaginas,
            TotalRegistros = totalRegistros,
            RegistrosPorPagina = paginacion.Cantidad,
            Datos = datos
        };
    }

    public async Task<(bool exito, string mensaje, MatriculaAsignacionResultadoDto resultado)> AsignarMasivo(MatriculaAsignacionMasivaDto dto, int idEscuela)
    {
        var resultado = new MatriculaAsignacionResultadoDto
        {
            TotalSolicitados = dto.IdsAlumnos.Distinct().Count()
        };

        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe o no pertenece a esta escuela.", resultado);
        if (!curso.Activo)
            return (false, "No se puede matricular alumnos en un curso inactivo.", resultado);

        var ciclo = await _cicloLectivoRepository.ObtenerPorId(curso.IdCicloLectivo, idEscuela);
        if (ciclo is null || !ciclo.Activo)
            return (false, "El ciclo lectivo del curso no existe o está inactivo.", resultado);

        var persistencia = await _matriculaRepository.AsignarMasivo(dto.IdCurso, curso.IdCicloLectivo, idEscuela, dto.IdsAlumnos);

        foreach (var m in persistencia.MatriculasCreadas)
            resultado.Detalle.Add(new() { IdAlumno = m.IdAlumno, Apellido = m.Alumno.Apellido, Nombre = m.Alumno.Nombre, Resultado = "Matriculado", IdMatricula = m.IdMatricula });
        foreach (var m in persistencia.MatriculasReactivadas)
            resultado.Detalle.Add(new() { IdAlumno = m.IdAlumno, Apellido = m.Alumno.Apellido, Nombre = m.Alumno.Nombre, Resultado = "Reactivado", IdMatricula = m.IdMatricula });
        foreach (var m in persistencia.MatriculasYaExistentes)
            resultado.Detalle.Add(new() { IdAlumno = m.IdAlumno, Apellido = m.Alumno.Apellido, Nombre = m.Alumno.Nombre, Resultado = "YaMatriculado", Mensaje = "El alumno ya está matriculado en el curso seleccionado.", IdMatricula = m.IdMatricula });
        foreach (var c in persistencia.Conflictos)
        {
            var alumno = await _alumnoRepository.ObtenerPorId(c.IdAlumno, idEscuela);
            resultado.Detalle.Add(new()
            {
                IdAlumno = c.IdAlumno,
                Apellido = alumno?.Apellido ?? string.Empty,
                Nombre = alumno?.Nombre ?? string.Empty,
                Resultado = "Conflicto",
                Mensaje = $"El alumno ya está matriculado en {c.CursoActual}."
            });
        }
        foreach (var idAlumno in persistencia.AlumnosNoEncontrados)
        {
            resultado.Detalle.Add(new()
            {
                IdAlumno = idAlumno,
                Resultado = "NoEncontrado",
                Mensaje = "El alumno no existe, está inactivo o no pertenece a la escuela."
            });
        }

        resultado.Matriculados = persistencia.MatriculasCreadas.Count + persistencia.MatriculasReactivadas.Count;
        resultado.YaMatriculados = persistencia.MatriculasYaExistentes.Count;
        resultado.Conflictos = persistencia.Conflictos.Count + persistencia.AlumnosNoEncontrados.Count;

        var mensaje = $"Proceso finalizado. {resultado.Matriculados} matriculados, {resultado.YaMatriculados} ya estaban matriculados y {resultado.Conflictos} quedaron con observaciones.";
        return (true, mensaje, resultado);
    }

    private static MatriculaResponseDto Mapear(Matricula m) => new()
    {
        IdMatricula = m.IdMatricula,
        IdEscuela = m.IdEscuela,
        IdAlumno = m.IdAlumno,
        Dni = m.Alumno.Dni,
        NombreAlumno = m.Alumno.Nombre,
        ApellidoAlumno = m.Alumno.Apellido,
        FecNac = m.Alumno.FecNac,
        IdCurso = m.IdCurso,
        Curso = $"{m.Curso.Grado}° {m.Curso.Division}" + (string.IsNullOrWhiteSpace(m.Curso.Turno) ? string.Empty : $" - {m.Curso.Turno}"),
        IdCicloLectivo = m.IdCicloLectivo,
        Anio = m.CicloLectivo.Anio,
        FechaMatricula = m.FechaMatricula,
        FechaBaja = m.FechaBaja,
        Estado = m.Estado.ToString()
    };
}
