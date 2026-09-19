using EducAR.API.DTOs.Mensajes;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.Data;
using Microsoft.EntityFrameworkCore;
using EducAR.API.DTOs.Paginacion;

using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class MensajeService : IMensajeService
{
    private readonly IMensajeRepository _mensajeRepository;
    private readonly AppDbContext _context;

    public MensajeService(IMensajeRepository mensajeRepository, AppDbContext context)
    {
        _mensajeRepository = mensajeRepository;
        _context = context;
    }

    public async Task<List<MensajeResumenDto>> ObtenerRecibidos(int idUsuario)
    {
        var mensajes = await _mensajeRepository.ObtenerRecibidos(idUsuario);
        return mensajes.Select(MapearAResumenDto).ToList();
    }

    public async Task<List<MensajeResumenDto>> ObtenerEnviados(int idUsuario)
    {
        var mensajes = await _mensajeRepository.ObtenerEnviados(idUsuario);
        return mensajes.Select(MapearAResumenDto).ToList();
    }

    public async Task<MensajeResponseDto?> ObtenerPorId(int idMensaje, int idUsuario)
    {
        var mensaje = await _mensajeRepository.ObtenerPorId(idMensaje, idUsuario);
        if (mensaje is null) return null;

        // Marcar como leído automáticamente si es el destinatario
        if (mensaje.IdUsuarioDestinat == idUsuario && !mensaje.Leido)
            await _mensajeRepository.MarcarLeido(idMensaje, idUsuario);

        return MapearAResponseDto(mensaje);
    }

    public async Task<(bool exito, string mensaje)> Enviar(MensajeCreateDto dto, int idUsuarioRemitente, int idEscuela)
    {
        // Mantiene compatibilidad con el destinatario individual anterior y agrega
        // la lista nueva. Distinct impide generar mensajes duplicados para un usuario.
        var idsDestinatarios = dto.IdsUsuariosDestinatarios.Append(dto.IdUsuarioDestinat)
            .Where(id => id > 0).Distinct().ToList();
        if (idsDestinatarios.Count == 0)
            return (false, "Debe seleccionar al menos un destinatario.");
        if (idsDestinatarios.Contains(idUsuarioRemitente))
            return (false, "No puede enviarse un mensaje a sí mismo.");

        var cantidadActivos = await _context.Usuarios.CountAsync(u => idsDestinatarios.Contains(u.IdUsuario) && u.IdEscuela == idEscuela && u.Activo);
        if (cantidadActivos != idsDestinatarios.Count)
            return (false, "Uno o más destinatarios no existen o no pertenecen a esta escuela.");

        // ObtenerDestinatarios aplica la relación docente-curso-alumno-tutor y exige
        // que el ciclo lectivo sea el actual. Ningún ID enviado por el navegador puede
        // saltear esa validación, incluso usando la opción "Seleccionar todos".
        var permitidos = (await ObtenerDestinatarios(idUsuarioRemitente, idEscuela))
            .Select(u => u.IdUsuario).ToHashSet();
        if (idsDestinatarios.Any(id => !permitidos.Contains(id)))
            return (false, "Solo puede enviar mensajes a docentes o tutores vinculados en el ciclo lectivo actual.");

        // Se crea un registro independiente por destinatario. Así cada tutor puede
        // leer el mensaje por separado y conservar su propio estado Leido.
        foreach (var idDestinatario in idsDestinatarios)
            await _mensajeRepository.Crear(new Mensaje
            {
                IdUsuarioRemitente = idUsuarioRemitente, IdUsuarioDestinat = idDestinatario,
                Asunto = dto.Asunto, MensajeTexto = dto.MensajeTexto,
                FechaEnvio = DateTime.Now, Leido = false, Activo = true
            });

        return (true, idsDestinatarios.Count == 1
            ? "Mensaje enviado correctamente."
            : $"Mensaje enviado a {idsDestinatarios.Count} destinatarios.");
    }

    public async Task<List<DestinatarioMensajeDto>> ObtenerDestinatarios(int idUsuario, int idEscuela)
    {
        // La lista se limita al año actual y al ciclo activo. Para un docente se
        // obtienen tutores de alumnos de sus cursos; para un tutor, los docentes
        // de los cursos de sus alumnos vinculados.
        var anioActual = DateTime.Now.Year;
        var esDocente = await _context.Docentes.AnyAsync(d =>
            d.IdUsuario == idUsuario && d.Usuario.IdEscuela == idEscuela && d.Usuario.Activo);
        var esTutor = await _context.Tutores.AnyAsync(t =>
            t.IdUsuario == idUsuario && t.Usuario.IdEscuela == idEscuela && t.Usuario.Activo);

        if (!esDocente && !esTutor) return new();

        if (esDocente)
        {
            return await (from asignacion in _context.DocenteMateriaCursos
                  join docente in _context.Docentes on asignacion.IdDocente equals docente.IdDocente
                  join curso in _context.Cursos on asignacion.IdCurso equals curso.IdCurso
                  join ciclo in _context.CiclosLectivos on curso.IdCicloLectivo equals ciclo.IdCicloLectivo
                  join matricula in _context.Matriculas on curso.IdCurso equals matricula.IdCurso
                  join alumno in _context.Alumnos on matricula.IdAlumno equals alumno.IdAlumno
                  join relacion in _context.AlumnoTutores on matricula.IdAlumno equals relacion.IdAlumno
                  join tutor in _context.Tutores on relacion.IdTutor equals tutor.IdTutor
                  join usuarioTutor in _context.Usuarios on tutor.IdUsuario equals usuarioTutor.IdUsuario
                  where docente.IdUsuario == idUsuario && asignacion.Activo && curso.Activo &&
                        curso.IdEscuela == idEscuela && matricula.IdEscuela == idEscuela &&
                        alumno.IdEscuela == idEscuela && alumno.Activo && usuarioTutor.Activo &&
                        ciclo.IdEscuela == idEscuela && ciclo.Activo && ciclo.Anio == anioActual &&
                        matricula.Estado == EstadoMatricula.Activa && relacion.Activo
                  select new DestinatarioMensajeDto
                  {
                      IdUsuario = usuarioTutor.IdUsuario,
                      IdAlumno = alumno.IdAlumno,
                      NombreCompleto = usuarioTutor.Nombre + " " + usuarioTutor.Apellido,
                      NombreAlumno = alumno.Nombre + " " + alumno.Apellido,
                      Rol = "Tutor"
                  }).Distinct().OrderBy(x => x.NombreAlumno).ThenBy(x => x.NombreCompleto).ToListAsync();
        }

        return await (from tutor in _context.Tutores
                  join relacion in _context.AlumnoTutores on tutor.IdTutor equals relacion.IdTutor
                  join matricula in _context.Matriculas on relacion.IdAlumno equals matricula.IdAlumno
                  join alumno in _context.Alumnos on relacion.IdAlumno equals alumno.IdAlumno
                  join curso in _context.Cursos on matricula.IdCurso equals curso.IdCurso
                  join ciclo in _context.CiclosLectivos on curso.IdCicloLectivo equals ciclo.IdCicloLectivo
                  join asignacion in _context.DocenteMateriaCursos on curso.IdCurso equals asignacion.IdCurso
                  join docente in _context.Docentes on asignacion.IdDocente equals docente.IdDocente
                  join usuarioDocente in _context.Usuarios on docente.IdUsuario equals usuarioDocente.IdUsuario
                  where tutor.IdUsuario == idUsuario && relacion.Activo && asignacion.Activo && curso.Activo &&
                        curso.IdEscuela == idEscuela && matricula.IdEscuela == idEscuela &&
                        alumno.IdEscuela == idEscuela && alumno.Activo && usuarioDocente.Activo &&
                        ciclo.IdEscuela == idEscuela && ciclo.Activo && ciclo.Anio == anioActual &&
                        matricula.Estado == EstadoMatricula.Activa
                  select new DestinatarioMensajeDto
                  {
                      IdUsuario = usuarioDocente.IdUsuario,
                      IdAlumno = alumno.IdAlumno,
                      NombreCompleto = usuarioDocente.Nombre + " " + usuarioDocente.Apellido,
                      NombreAlumno = alumno.Nombre + " " + alumno.Apellido,
                      Rol = "Docente"
                  }).Distinct().OrderBy(x => x.NombreAlumno).ThenBy(x => x.NombreCompleto).ToListAsync();
    }

    public async Task<bool> MarcarLeido(int idMensaje, int idUsuario)
    {
        return await _mensajeRepository.MarcarLeido(idMensaje, idUsuario);
    }

    public async Task<int> ContarNoLeidos(int idUsuario)
    {
        return await _mensajeRepository.ContarNoLeidos(idUsuario);
    }

    private static MensajeResumenDto MapearAResumenDto(Mensaje m) => new()
    {
        IdMensaje = m.IdMensaje,
        NombreRemitente = $"{m.Remitente.Nombre} {m.Remitente.Apellido}",
        NombreDestinatario = $"{m.Destinatario.Nombre} {m.Destinatario.Apellido}",
        Asunto = m.Asunto,
        FechaEnvio = m.FechaEnvio,
        Leido = m.Leido
    };

    private static MensajeResponseDto MapearAResponseDto(Mensaje m) => new()
    {
        IdMensaje = m.IdMensaje,
        IdUsuarioRemitente = m.IdUsuarioRemitente,
        NombreRemitente = $"{m.Remitente.Nombre} {m.Remitente.Apellido}",
        IdUsuarioDestinat = m.IdUsuarioDestinat,
        NombreDestinatario = $"{m.Destinatario.Nombre} {m.Destinatario.Apellido}",
        Asunto = m.Asunto,
        MensajeTexto = m.MensajeTexto,
        FechaEnvio = m.FechaEnvio,
        Leido = m.Leido
    };
    public async Task<ResultadoPaginadoDto<MensajeResumenDto>> ObtenerRecibidosPaginado(
    int idUsuario, int pagina, int cantidad)
    {
        var query = await _mensajeRepository.ObtenerQueryableRecibidos(idUsuario);
        var queryDto = query.Select(m => new MensajeResumenDto
        {
            IdMensaje = m.IdMensaje,
            NombreRemitente = m.Remitente.Nombre + " " + m.Remitente.Apellido,
            NombreDestinatario = m.Destinatario.Nombre + " " + m.Destinatario.Apellido,
            Asunto = m.Asunto,
            FechaEnvio = m.FechaEnvio,
            Leido = m.Leido
        });

        return await PaginacionHelper.PaginarAsync(queryDto, pagina, cantidad);
    }

    public async Task<ResultadoPaginadoDto<MensajeResumenDto>> ObtenerEnviadosPaginado(
        int idUsuario, int pagina, int cantidad)
    {
        var query = await _mensajeRepository.ObtenerQueryableEnviados(idUsuario);
        var queryDto = query.Select(m => new MensajeResumenDto
        {
            IdMensaje = m.IdMensaje,
            NombreRemitente = m.Remitente.Nombre + " " + m.Remitente.Apellido,
            NombreDestinatario = m.Destinatario.Nombre + " " + m.Destinatario.Apellido,
            Asunto = m.Asunto,
            FechaEnvio = m.FechaEnvio,
            Leido = m.Leido
        });

        return await PaginacionHelper.PaginarAsync(queryDto, pagina, cantidad);
    }
}
