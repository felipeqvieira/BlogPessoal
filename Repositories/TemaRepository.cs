using BlogPessoal.Data;
using BlogPessoal.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPessoal.Repositories;
/// <summary>
/// Repositório p/ a manipulação de dados da entidade <see cref="Tema"/> utilizando Entity Framework Core.
/// </summary>
/// <param name="db">O contexto de bd</param>
public class TemaRepository(AppDbContext db) : IRepository<Tema>
{
    /// <inheritdoc />
    public async Task<IEnumerable<Tema>> GetAllAsync() =>
        await db.Temas.ToListAsync();

    /// <inheritdoc />
    /// <remarks>
    /// Realiza carregamento adiantado utilizando <c>.Include()</c>
    /// p/ trazer todas as postagens atreladas a este tema.
    /// </remarks>
    public async Task<Tema?> GetByIdAsync(long id) =>
        await db.Temas
                .Include(t => t.Postagens)
                .FirstOrDefaultAsync(t => t.Id == id);

    /// <inheritdoc />
    public async Task<Tema> CreateAsync(Tema entity)
    {
        db.Temas.Add(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc />
    public async Task<Tema?> UpdateAsync(Tema entity)
    {
        db.Temas.Update(entity);
        await db.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await db.Temas.FindAsync(id);
        if (entity is null) return false;
        db.Temas.Remove(entity);
        await db.SaveChangesAsync();
        return true;
    }
}