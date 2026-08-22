using EducAR.API.DTOs.Telefonos;
using EducAR.API.Models;

namespace EducAR.API.Services.Interfaces;

public interface ITelefonoService
{
    Task<IEnumerable<TelefonoDto>> ObtenerPorAlumnoAsync(int idAlumno, int idEscuela);
    Task<(bool exito, string mensaje, TelefonoDto? telefono)> CrearAsync(TelefonoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje, TelefonoDto? telefono)> ActualizarAsync(int idTelefono, TelefonoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> EliminarAsync(int idTelefono, int idEscuela);
}