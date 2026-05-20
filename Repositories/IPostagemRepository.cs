using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Interface p/ repo de postagem
/// Estende as operações básicas de <see cref="IRepository{Postagem}"/> c/ consultas customizadas
/// </summary>
public interface IPostagemRepository : IRepository<Postagem>
{
    /// <summary>
    /// Busca uma lista de postagens baseada em filtros opcionais de autor e/ou tema
    /// </summary>
    /// <param name="usuarioId">Opcional. O id do autor</param>
    /// <param name="temaId">Opcional. O id do tema</param>
    /// <returns>Uma coleção assíncrona de postagens que atende aos critérios passados</returns>
    Task<IEnumerable<Postagem>> GetByFiltroAsync(long? usuarioId, long? temaId);
}