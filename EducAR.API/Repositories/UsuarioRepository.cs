using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ObtenerTodos(int idEscuela)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Where(u => u.IdEscuela == idEscuela)
            .OrderBy(u => u.Apellido)
            .ToListAsync();
    }

    public async Task<Usuario?> ObtenerPorId(int idUsuario, int idEscuela)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.IdEscuela == idEscuela);
    }

    // public async Task<bool> ExisteNombreUsuario(string nombreUsuario, int idEscuela)
    // {
    //     return await _context.Usuarios
    //         .AnyAsync(u => u.NombreUsuario == nombreUsuario && u.IdEscuela == idEscuela);
    // }

    public async Task<bool> ExisteNombreUsuario(string nombreUsuario, int idEscuela)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.NombreUsuario == nombreUsuario &&
                        u.IdEscuela == idEscuela &&
                        u.Activo);  // ← agregar esto
    }

    public async Task<Usuario> Crear(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> Actualizar(Usuario usuario)
    {
        usuario.FechaAct = DateTime.Now;
        _context.Usuarios.Update(usuario);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idUsuario, int idEscuela)
    {
        var usuario = await ObtenerPorId(idUsuario, idEscuela);
        if (usuario is null) return false;

        // Baja lógica
        usuario.Activo = false;
        usuario.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    // public async Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdUsuario = null)
    // {
    //     return await _context.Usuarios
    //         .AnyAsync(u => u.Dni == dni &&
    //                     u.IdEscuela == idEscuela &&
    //                     (excluirIdUsuario == null || u.IdUsuario != excluirIdUsuario));
    // }

    // public async Task<bool> ExisteEmail(string email, int idEscuela, int? excluirIdUsuario = null)
    // {
    //     return await _context.Usuarios
    //         .AnyAsync(u => u.Email == email &&
    //                     u.IdEscuela == idEscuela &&
    //                     (excluirIdUsuario == null || u.IdUsuario != excluirIdUsuario));
    // }
    
    public async Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdUsuario = null)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Dni == dni &&
                        u.IdEscuela == idEscuela &&
                        u.Activo &&  // ← agregar esto
                        (excluirIdUsuario == null || u.IdUsuario != excluirIdUsuario));
    }

    public async Task<bool> ExisteEmail(string email, int idEscuela, int? excluirIdUsuario = null)
    {
        return await _context.Usuarios
            .AnyAsync(u => u.Email == email &&
                        u.IdEscuela == idEscuela &&
                        u.Activo &&  // ← agregar esto
                        (excluirIdUsuario == null || u.IdUsuario != excluirIdUsuario));
    }
    public async Task<Usuario?> ObtenerPorIdSinEscuela(int idUsuario)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.Activo);
    }

    public async Task<Usuario?> ObtenerPerfilCompleto(int idUsuario)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Escuela)
            .FirstOrDefaultAsync(u => u.IdUsuario == idUsuario && u.Activo);
    }

    public async Task<Usuario?> ObtenerUsuarioInactivoPorNombre(string nombreUsuario, int idEscuela)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario &&
                                    u.IdEscuela == idEscuela &&
                                    !u.Activo);
    }

    public Task<IQueryable<Usuario>> ObtenerQueryable(int idEscuela)
    {
        var query = _context.Usuarios
            .Include(u => u.Rol)
            .Where(u => u.IdEscuela == idEscuela)
            .OrderBy(u => u.Apellido)
            .AsQueryable();

        return Task.FromResult(query);
    }
}