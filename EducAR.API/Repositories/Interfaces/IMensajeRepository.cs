using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IMensajeRepository
{
    Task<List<Mensaje>> ObtenerRecibidos(int idUsuario);
    Task<List<Mensaje>> ObtenerEnviados(int idUsuario);
    Task<Mensaje?> ObtenerPorId(int idMensaje, int idUsuario);
    Task<Mensaje> Crear(Mensaje mensaje);
    Task<bool> MarcarLeido(int idMensaje, int idUsuario);
    Task<bool> Eliminar(int idMensaje, int idUsuario);
    Task<int> ContarNoLeidos(int idUsuario);

    Task<IQueryable<Mensaje>> ObtenerQueryableRecibidos(int idUsuario);
    Task<IQueryable<Mensaje>> ObtenerQueryableEnviados(int idUsuario);
}