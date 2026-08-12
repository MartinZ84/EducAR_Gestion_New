using EducAR.API.DTOs.Paginacion;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Helpers;

public static class PaginacionHelper
{
    public static async Task<ResultadoPaginadoDto<T>> PaginarAsync<T>(
        IQueryable<T> query, int pagina, int cantidad)
    {
        var totalRegistros = await query.CountAsync();
        var totalPaginas   = (int)Math.Ceiling((double)totalRegistros / cantidad);

        var datos = await query
            .Skip((pagina - 1) * cantidad)
            .Take(cantidad)
            .ToListAsync();

        return new ResultadoPaginadoDto<T>
        {
            PaginaActual       = pagina,
            TotalPaginas       = totalPaginas,
            TotalRegistros     = totalRegistros,
            RegistrosPorPagina = cantidad,
            Datos              = datos
        };
    }
}