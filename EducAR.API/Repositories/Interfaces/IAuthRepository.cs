using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<Usuario?> ObtenerPorUsuarioYEscuela(string nombreUsuario, int idEscuela);
    Task ActualizarUltimoAcceso(int idUsuario);
}