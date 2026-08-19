using EducAR.API.DTOs.Telefonos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class TelefonoService : ITelefonoService
{
    private readonly ITelefonoRepository _telefonoRepo;
    private readonly IAlumnoRepository _alumnoRepo;

    public TelefonoService(
        ITelefonoRepository telefonoRepo,
        IAlumnoRepository alumnoRepo)
    {
        _telefonoRepo = telefonoRepo;
        _alumnoRepo = alumnoRepo;
    }

    public async Task<IEnumerable<TelefonoDto>> ObtenerPorAlumnoAsync(int idAlumno, int idEscuela)
    {
        // Verificamos que el alumno pertenezca a la escuela
        var alumno = await _alumnoRepo.ObtenerPorId(idAlumno, idEscuela);
        if (alumno == null) return new List<TelefonoDto>();

        var telefonos = await _telefonoRepo.ObtenerPorAlumnoAsync(idAlumno);
        return telefonos.Select(t => new TelefonoDto
        {
            IdTelefono = t.IdTelefono,
            IdAlumno = t.IdAlumno,
            Numero = t.Numero,
            Tipo = t.Tipo,
            EsPrincipal = t.EsPrincipal,
        });
    }

    public async Task<(bool exito, string mensaje, TelefonoDto? telefono)> CrearAsync(
        TelefonoCreateDto dto, int idEscuela)
    {
        // Verificamos que el alumno pertenezca a la escuela
        var alumno = await _alumnoRepo.ObtenerPorId(dto.IdAlumno, idEscuela);
        if (alumno == null)
            return (false, "El alumno no existe o no pertenece a la escuela.", null);

        var telefono = new TelefonoContacto
        {
            IdAlumno = dto.IdAlumno,
            Numero = dto.Numero,
            Tipo = dto.Tipo,
            EsPrincipal = dto.EsPrincipal,
        };

        var creado = await _telefonoRepo.CrearAsync(telefono);

        var dtoResult = new TelefonoDto
        {
            IdTelefono = creado.IdTelefono,
            IdAlumno = creado.IdAlumno,
            Numero = creado.Numero,
            Tipo = creado.Tipo,
            EsPrincipal = creado.EsPrincipal,
        };

        return (true, "Teléfono agregado correctamente.", dtoResult);
    }

    public async Task<(bool exito, string mensaje)> EliminarAsync(int idTelefono, int idEscuela)
    {
        var telefono = await _telefonoRepo.ObtenerPorIdAsync(idTelefono);
        if (telefono == null)
            return (false, "Teléfono no encontrado.");

        // Verificamos que el alumno dueño del teléfono pertenezca a la escuela
        var alumno = await _alumnoRepo.ObtenerPorId(telefono.IdAlumno, idEscuela);
        if (alumno == null)
            return (false, "No tenés permiso para eliminar este teléfono.");

        await _telefonoRepo.EliminarAsync(idTelefono);
        return (true, "Teléfono eliminado correctamente.");
    }
}