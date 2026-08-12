using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ObtenerPorUsuarioYEscuela(string nombreUsuario, int idEscuela)
    {
        return await _context.Usuarios
            .Include(u => u.Rol)
            .Include(u => u.Escuela)
            .FirstOrDefaultAsync(u =>
                u.NombreUsuario == nombreUsuario &&
                u.IdEscuela == idEscuela &&
                u.Activo);
    }

    public async Task ActualizarUltimoAcceso(int idUsuario)
    {
        var usuario = await _context.Usuarios.FindAsync(idUsuario);
        if (usuario is null) return;

        usuario.UltimoAcceso = DateTime.Now;
        usuario.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
    }
}