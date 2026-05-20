using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Repositório p/ a manipulação de dados da entidade <see cref="Postagem"/> utilizando Entity Framework Core
/// </summary>
/// <param name="db">O contexto de bd </param>
public class PostagemRepository(AppDbContext db) : IPostagemRepository
{

    /// <inheritdoc />
    /// <remarks>
    /// Retorna as postagens ordenadas de forma decrescente pela data e já inclui as propriedades 
    /// de navegação <see cref="Tema"/> e <see cref="Usuario"/>
    /// </remarks>
    public async Task<IEnumerable<Postagem>> GetAllAsync() =>
        await db.Postagens
                .Include(p => p.Tema)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.Data)
                .ToListAsync();


    /// <inheritdoc />
    /// <remarks>
    /// <c>AsQueryable</c> para compor a query SQL dinamicamente apenas com os filtros 
    /// que possuem valor
    /// </remarks>
    public async Task<IEnumerable<Postagem>> GetByFiltroAsync(long? usuarioId, long? temaId)
    {
        var query = db.Postagens
                      .Include(p => p.Tema)
                      .Include(p => p.Usuario)
                      .AsQueryable();

        if (usuarioId.HasValue)
            query = query.Where(p => p.UsuarioId == usuarioId);
        if (temaId.HasValue)
            query = query.Where(p => p.TemaId == temaId);

        return await query.OrderByDescending(p => p.Data).ToListAsync();
    }

    /// <inheritdoc />
    public async Task<Postagem?> GetByIdAsync(long id) =>
        await db.Postagens
                .Include(p => p.Tema)
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

    /// <inheritdoc />
    /// <remarks>
    /// Nova consulta para recarrega propriedades Tema e Usuario p/ garantir que tenha os nomes 
    /// e descrições para o envio
    /// </remarks>
    public async Task<Postagem> CreateAsync(Postagem entity)
    {
        db.Postagens.Add(entity);
        await db.SaveChangesAsync();

        // Recarrega com navegações para que temaDescricao e usuarioNome
        // sejam preenchidos corretamente na resposta do POST
        return await GetByIdAsync(entity.Id) ?? entity;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Igual ao <see cref="CreateAsync"/> em relação ao recarregamento das 
    /// propriedades de navegação.
    /// </remarks>
    public async Task<Postagem?> UpdateAsync(Postagem entity)
    {
        db.Postagens.Update(entity);
        await db.SaveChangesAsync();

        // Recarrega com navegações após atualização pelo mesmo motivo
        return await GetByIdAsync(entity.Id);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await db.Postagens.FindAsync(id);
        if (entity is null) return false;
        db.Postagens.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}