using EducAR.API.DTOs.Alumnos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class AlumnoService : IAlumnoService
{
    private readonly IAlumnoRepository _alumnoRepository;

    public AlumnoService(IAlumnoRepository alumnoRepository)
    {
        _alumnoRepository = alumnoRepository;
    }

    public async Task<List<AlumnoResponseDto>> ObtenerTodos(int idEscuela)
    {
        var alumnos = await _alumnoRepository.ObtenerTodos(idEscuela);
        return alumnos.Select(MapearAResponseDto).ToList();
    }

    public async Task<AlumnoResponseDto?> ObtenerPorId(int idAlumno, int idEscuela)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        return alumno is null ? null : MapearAResponseDto(alumno);
    }

    public async Task<AlumnoDetalleDto?> ObtenerDetalle(int idAlumno, int idEscuela)
    {
        var alumno = await _alumnoRepository.ObtenerDetalle(idAlumno, idEscuela);
        if (alumno is null) return null;
        var asistencias = alumno.Asistencias.Where(a => a.Activo).ToList();
        var boletines = alumno.Boletines.Where(b => b.Activo).ToList();
        return new AlumnoDetalleDto
        {
            IdAlumno = alumno.IdAlumno, Dni = alumno.Dni, Nombre = alumno.Nombre,
            Apellido = alumno.Apellido, FecNac = alumno.FechaNacimiento, Activo = alumno.Activo,
            Calle = alumno.Calle, Numero = alumno.Numero, Piso = alumno.Piso, Departamento = alumno.Departamento,
            Barrio = alumno.Barrio, Localidad = alumno.Localidad, Provincia = alumno.Provincia,
            Telefonos = alumno.Telefonos.Select(t => new TelefonoDetalleDto { Numero = t.Numero, Des = t.Des }).ToList(),
            MatriculaActual = alumno.Matriculas.Where(m => m.Estado == EstadoMatricula.Activa && m.CicloLectivo.Activo)
                .OrderByDescending(m => m.FechaMatricula).Select(m => new MatriculaActualDto
                { IdCurso = m.IdCurso, Curso = $"{m.Curso.Grado}° {m.Curso.Division}", CicloLectivo = m.CicloLectivo.Anio.ToString(), FechaMatricula = m.FechaMatricula }).FirstOrDefault(),
            Tutores = alumno.AlumnoTutores.Where(at => at.Activo).Select(at => new AlumnoTutorDetalleDto
            { IdTutor = at.IdTutor, NombreCompleto = $"{at.Tutor.Usuario.Nombre} {at.Tutor.Usuario.Apellido}", Parentesco = at.Parentesco }).ToList(),
            AsistenciaResumen = new AsistenciaResumenDto { Presentes = asistencias.Count(a => a.Presente), Ausentes = asistencias.Count(a => !a.Presente), Justificadas = 0 },
            Asistencias = asistencias.OrderByDescending(a => a.Fecha).Select(a => new AsistenciaDetalleDto { IdAsistencia = a.IdAsistencia, Fecha = a.Fecha, Presente = a.Presente, Estado = a.Presente ? "Presente" : "Ausente" }).ToList(),
            Calificaciones = alumno.Calificaciones.Where(c => c.Activo).Select(c => new CalificacionDetalleDto { Materia = c.Materia.Nombre, Nota = c.ValorCalificacion, Periodo = c.PeriodoEvaluacion.Nombre }).ToList(),
            Boletines = boletines.Select(b =>
            {
                var notas = b.Detalles.Where(d => d.Activo && d.CalificacionFinal.HasValue)
                    .Select(d => d.CalificacionFinal!.Value).ToList();
                var promedio = notas.Count == 0 ? (decimal?)null : notas.Average();
                return new BoletinDetalleDto
                {
                    Periodo = b.PeriodoEvaluacion.Nombre,
                    Promedio = promedio,
                    Estado = promedio >= 6 ? "Aprobado" : "Pendiente"
                };
            }).ToList()
        };
    }

    public async Task<(bool exito, string mensaje, AlumnoResponseDto? alumno)> Crear(AlumnoCreateDto dto, int idEscuela)
    {
        if (dto.FechaNacimiento == default || dto.FechaNacimiento > DateTime.Today)
            return (false, "La fecha de nacimiento no es válida.", null);

        if (await _alumnoRepository.ExisteDni(dto.Dni, idEscuela))
            return (false, "Ya existe un alumno con ese DNI en esta escuela.", null);

        var alumno = new Alumno
        {
            IdEscuela = idEscuela,
            Dni = dto.Dni,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            FechaNacimiento = dto.FechaNacimiento,
            Activo = true,
            Calle = dto.Calle,
            Numero = dto.Numero,
            Piso = dto.Piso,
            Departamento = dto.Departamento,
            Barrio = dto.Barrio,
            Localidad = dto.Localidad,
            Provincia = dto.Provincia,
        };

        await _alumnoRepository.Crear(alumno);
        return (true, "Alumno creado correctamente.", MapearAResponseDto(alumno));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idAlumno, int idEscuela, AlumnoUpdateDto dto)
    {
        if (dto.FechaNacimiento == default || dto.FechaNacimiento > DateTime.Today)
            return (false, "La fecha de nacimiento no es válida.");

        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (await _alumnoRepository.ExisteDni(dto.Dni, idEscuela, idAlumno))
            return (false, "Ya existe un alumno con ese DNI en esta escuela.");

        alumno.Dni = dto.Dni;
        alumno.Nombre = dto.Nombre;
        alumno.Apellido = dto.Apellido;
        alumno.FechaNacimiento = dto.FechaNacimiento;
        alumno.Activo = dto.Activo;
        alumno.FechaNacimiento = dto.FechaNacimiento;
        alumno.Calle = dto.Calle;
        alumno.Numero = dto.Numero;
        alumno.Piso = dto.Piso;
        alumno.Departamento = dto.Departamento;
        alumno.Barrio = dto.Barrio;
        alumno.Localidad = dto.Localidad;
        alumno.Provincia = dto.Provincia;

        await _alumnoRepository.Actualizar(alumno);
        return (true, "Alumno actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idAlumno, int idEscuela)
    {
        return await _alumnoRepository.Eliminar(idAlumno, idEscuela);
    }

    // ==========================
    // TUTORES
    // ==========================

    public async Task<(bool exito, string mensaje)> AsociarTutor(int idAlumno, int idEscuela, AsociarTutorDto dto)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (!await _alumnoRepository.ExisteTutorEnEscuela(dto.IdTutor, idEscuela))
            return (false, "El tutor no existe o no pertenece a esta escuela.");

        var asociacion = await _alumnoRepository.ObtenerAsociacionTutor(idAlumno, dto.IdTutor);
        if (asociacion is not null)
        {
            if (asociacion.Activo)
                return (false, "El tutor ya está asociado a ese alumno.");

            asociacion.Activo = true;
            asociacion.Parentesco = dto.Parentesco;
            asociacion.EsResponsablePrinc = dto.EsResponsablePrinc;
            await _alumnoRepository.GuardarCambios();
            return (true, "Tutor reasociado al alumno correctamente.");
        }

        var nueva = new AlumnoTutor
        {
            IdAlumno = idAlumno,
            IdTutor = dto.IdTutor,
            Parentesco = dto.Parentesco,
            EsResponsablePrinc = dto.EsResponsablePrinc,
            Activo = true
        };

        await _alumnoRepository.AsociarTutor(nueva);
        return (true, "Tutor asociado al alumno correctamente.");
    }

    public async Task<(bool exito, string mensaje)> QuitarTutor(int idAlumno, int idEscuela, int idTutor)
    {
        var alumno = await _alumnoRepository.ObtenerPorId(idAlumno, idEscuela);
        if (alumno is null)
            return (false, "Alumno no encontrado.");

        if (alumno.AlumnoTutores.Count(at => at.Activo) <= 1)
            return (false, "El alumno debe conservar al menos un tutor asignado.");

        var exito = await _alumnoRepository.QuitarTutor(idAlumno, idTutor);
        if (!exito)
            return (false, "El tutor no tiene una asociación activa con ese alumno.");

        return (true, "Tutor desasociado del alumno correctamente.");
    }

    public async Task<ResultadoPaginadoDto<AlumnoResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro)
    {
        var query = await _alumnoRepository.ObtenerQueryable(idEscuela, filtro.Nombre, filtro.Apellido, filtro.Dni);
        var queryDto = query.Select(a => new AlumnoResponseDto
        {
            IdAlumno = a.IdAlumno,
            Nombre = a.Nombre,
            Apellido = a.Apellido,
            Dni = a.Dni,
            FechaNacimiento = a.FechaNacimiento,
            Activo = a.Activo
        });

        return await PaginacionHelper.PaginarAsync(queryDto, filtro.Pagina, filtro.Cantidad);
    }

    private static AlumnoResponseDto MapearAResponseDto(Alumno a) => new()
    {
        IdAlumno = a.IdAlumno,
        Dni = a.Dni,
        Nombre = a.Nombre,
        Apellido = a.Apellido,
        FechaNacimiento = a.FechaNacimiento,
        Activo = a.Activo,
            Calle = a.Calle,
            Numero = a.Numero,
            Piso = a.Piso,
            Departamento = a.Departamento,
            Barrio = a.Barrio,
            Localidad = a.Localidad,
            Provincia = a.Provincia,
        Matriculas = a.Matriculas?.Select(m => new AlumnoMatriculaDto
        {
            IdMatricula = m.IdMatricula,
            IdCurso = m.IdCurso,
            Grado = m.Curso.Grado,
            Division = m.Curso.Division,
            Turno = m.Curso.Turno,
            IdCicloLectivo = m.IdCicloLectivo,
            Anio = m.CicloLectivo.Anio,
            Estado = m.Estado.ToString(),
            FechaMatricula = m.FechaMatricula,
            FechaBaja = m.FechaBaja
        }).ToList() ?? new(),
        Tutores = a.AlumnoTutores?.Select(at => new AlumnoTutorDto
        {
            IdAlumnoTutor = at.IdAlumnoTutor,
            IdTutor = at.IdTutor,
            Nombre = at.Tutor.Usuario.Nombre,
            Apellido = at.Tutor.Usuario.Apellido,
            Parentesco = at.Parentesco,
            EsResponsablePrinc = at.EsResponsablePrinc,
            Activo = at.Activo
        }).ToList() ?? new()
    };
}
