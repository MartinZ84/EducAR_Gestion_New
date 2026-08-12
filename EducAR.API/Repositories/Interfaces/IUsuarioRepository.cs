using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ObtenerTodos(int idEscuela);
    Task<Usuario?> ObtenerPorId(int idUsuario, int idEscuela);
    Task<bool> ExisteNombreUsuario(string nombreUsuario, int idEscuela);
    Task<Usuario> Crear(Usuario usuario);
    Task<bool> Actualizar(Usuario usuario);
    Task<bool> Eliminar(int idUsuario, int idEscuela);
    Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdUsuario = null);
    Task<bool> ExisteEmail(string email, int idEscuela, int? excluirIdUsuario = null);
    Task<Usuario?> ObtenerPorIdSinEscuela(int idUsuario);
    Task<Usuario?> ObtenerPerfilCompleto(int idUsuario);
    Task<Usuario?> ObtenerUsuarioInactivoPorNombre(string nombreUsuario, int idEscuela);
    Task<IQueryable<Usuario>> ObtenerQueryable(int idEscuela);
}