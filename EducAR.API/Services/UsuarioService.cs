using EducAR.API.DTOs.Usuarios;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Helpers;

namespace EducAR.API.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<List<UsuarioResponseDto>> ObtenerTodos(int idEscuela)
    {
        var usuarios = await _usuarioRepository.ObtenerTodos(idEscuela);
        return usuarios.Select(MapearAResponseDto).ToList();
    }

    public async Task<UsuarioResponseDto?> ObtenerPorId(int idUsuario, int idEscuela)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(idUsuario, idEscuela);
        return usuario is null ? null : MapearAResponseDto(usuario);
    }

    public async Task<(bool exito, string mensaje, UsuarioResponseDto? usuario)> Crear(UsuarioCreateDto dto)
    {
         // Validar duplicados
        if (await _usuarioRepository.ExisteNombreUsuario(dto.NombreUsuario, dto.IdEscuela))
        return (false, "El nombre de usuario ya existe en esta escuela.", null);

        if (await _usuarioRepository.ExisteDni(dto.Dni, dto.IdEscuela))
            return (false, "Ya existe un usuario con ese DNI en esta escuela.", null);

        if (await _usuarioRepository.ExisteEmail(dto.Email, dto.IdEscuela))
            return (false, "Ya existe un usuario con ese email en esta escuela.", null);
       
        var existe = await _usuarioRepository.ExisteNombreUsuario(dto.NombreUsuario, dto.IdEscuela);
        if (existe)
            return (false, "El nombre de usuario ya existe en esta escuela.", null);

        var usuario = new Usuario
        {
            IdRol           = dto.IdRol,
            IdEscuela       = dto.IdEscuela,
            Dni             = dto.Dni,
            Nombre          = dto.Nombre,
            Apellido        = dto.Apellido,
            Email           = dto.Email,
            NombreUsuario   = dto.NombreUsuario,
            HashContrasena  = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena),
            Activo          = true
        };

        var creado = await _usuarioRepository.Crear(usuario);

        // Volver a buscarlo para traer el Rol incluido
        var completo = await _usuarioRepository.ObtenerPorId(creado.IdUsuario, creado.IdEscuela);

        return (true, "Usuario creado correctamente.", MapearAResponseDto(completo!));
    }

    public async Task<(bool exito, string mensaje)> Actualizar(int idUsuario, int idEscuela, UsuarioUpdateDto dto)
    {
        var usuario = await _usuarioRepository.ObtenerPorId(idUsuario, idEscuela);
        if (usuario is null)
            return (false, "Usuario no encontrado.");        

        if (await _usuarioRepository.ExisteDni(dto.Dni, idEscuela, idUsuario))
            return (false, "Ya existe un usuario con ese DNI en esta escuela.");

        if (await _usuarioRepository.ExisteEmail(dto.Email, idEscuela, idUsuario))
            return (false, "Ya existe un usuario con ese email en esta escuela.");

        usuario.IdRol    = dto.IdRol;
        usuario.Dni      = dto.Dni;
        usuario.Nombre   = dto.Nombre;
        usuario.Apellido = dto.Apellido;
        usuario.Email    = dto.Email;
        usuario.Activo   = dto.Activo;

        await _usuarioRepository.Actualizar(usuario);
        return (true, "Usuario actualizado correctamente.");
    }

    public async Task<bool> Eliminar(int idUsuario, int idEscuela)
    {
        return await _usuarioRepository.Eliminar(idUsuario, idEscuela);
    }

    private static UsuarioResponseDto MapearAResponseDto(Usuario u) => new()
    {
        IdUsuario     = u.IdUsuario,
        IdRol         = u.IdRol,
        Rol           = u.Rol.Nombre,
        IdEscuela     = u.IdEscuela,
        Dni           = u.Dni,
        Nombre        = u.Nombre,
        Apellido      = u.Apellido,
        Email         = u.Email,
        NombreUsuario = u.NombreUsuario,
        Activo        = u.Activo,
        UltimoAcceso  = u.UltimoAcceso
    };
    
    public async Task<(bool exito, string mensaje)> CambiarContrasena(int idUsuario, CambiarContrasenaDto dto)
    {
        if (dto.NuevaContrasena != dto.ConfirmarContrasena)
            return (false, "La nueva contraseña y la confirmación no coinciden.");

        if (dto.NuevaContrasena.Length < 6)
            return (false, "La nueva contraseña debe tener al menos 6 caracteres.");

        var usuario = await _usuarioRepository.ObtenerPorIdSinEscuela(idUsuario);
        if (usuario is null)
            return (false, "Usuario no encontrado.");

        if (!BCrypt.Net.BCrypt.Verify(dto.ContrasenaActual, usuario.HashContrasena))
            return (false, "La contraseña actual es incorrecta.");

        usuario.HashContrasena = BCrypt.Net.BCrypt.HashPassword(dto.NuevaContrasena);
        usuario.FechaAct       = DateTime.Now;
        await _usuarioRepository.Actualizar(usuario);

        return (true, "Contraseña actualizada correctamente.");
    }

    public async Task<PerfilResponseDto?> ObtenerPerfil(int idUsuario)
    {
        var usuario = await _usuarioRepository.ObtenerPerfilCompleto(idUsuario);
        if (usuario is null) return null;

        return new PerfilResponseDto
        {
            IdUsuario      = usuario.IdUsuario,
            NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
            NombreUsuario  = usuario.NombreUsuario,
            Email          = usuario.Email,
            Rol            = usuario.Rol.Nombre,
            NombreEscuela  = usuario.Escuela.Nombre,
            UltimoAcceso   = usuario.UltimoAcceso
        };
    }

    public async Task<ResultadoPaginadoDto<UsuarioResponseDto>> ObtenerTodosPaginado(int idEscuela, int pagina, int cantidad)
    {
        var query = await _usuarioRepository.ObtenerQueryable(idEscuela);
        var queryDto = query.Select(u => new UsuarioResponseDto
        {
            IdUsuario     = u.IdUsuario,
            IdRol         = u.IdRol,
            Rol           = u.Rol.Nombre,
            IdEscuela     = u.IdEscuela,
            Dni           = u.Dni,
            Nombre        = u.Nombre,
            Apellido      = u.Apellido,
            Email         = u.Email,
            NombreUsuario = u.NombreUsuario,
            Activo        = u.Activo,
            UltimoAcceso  = u.UltimoAcceso
        });

        return await PaginacionHelper.PaginarAsync(queryDto, pagina, cantidad);
    }
}