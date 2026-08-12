using EducAR.API.DTOs.Usuarios;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface IUsuarioService
{
    Task<List<UsuarioResponseDto>> ObtenerTodos(int idEscuela);
    Task<UsuarioResponseDto?> ObtenerPorId(int idUsuario, int idEscuela);
    Task<(bool exito, string mensaje, UsuarioResponseDto? usuario)> Crear(UsuarioCreateDto dto);
    Task<(bool exito, string mensaje)> Actualizar(int idUsuario, int idEscuela, UsuarioUpdateDto dto);
    Task<bool> Eliminar(int idUsuario, int idEscuela);
    Task<(bool exito, string mensaje)> CambiarContrasena(int idUsuario, CambiarContrasenaDto dto);
    Task<PerfilResponseDto?> ObtenerPerfil(int idUsuario);
    Task<ResultadoPaginadoDto<UsuarioResponseDto>> ObtenerTodosPaginado(int idEscuela, int pagina, int cantidad);
}