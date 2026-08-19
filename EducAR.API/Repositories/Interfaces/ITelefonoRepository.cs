using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface ITelefonoRepository
{
    Task<IEnumerable<TelefonoContacto>> ObtenerPorAlumnoAsync(int idAlumno);
    Task<TelefonoContacto?> ObtenerPorIdAsync(int idTelefono);
    Task<TelefonoContacto> CrearAsync(TelefonoContacto telefono);
    Task EliminarAsync(int idTelefono);
    Task<bool> ExisteAsync(int idTelefono);
}