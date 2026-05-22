using BlogPessoal.DTOs.Tema;
using BlogPessoal.Models;
using BlogPessoal.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPessoal.Services;

/// <summary>
/// Implementação concreta da camada de serviço responsável por gerir as regras de domínio da entidade <see cref="Tema"/>.
/// </summary>
/// <param name="repo">Repo genérico utilizado para isolar o acesso à base de dados.</param>
public class TemaService(IRepository<Tema> repo) : ITemaService
{
    /// <inheritdoc />
    public async Task<Tema> CriarAsync(CreateTemaDto dto)
    {
        var tema = new Tema
        {
            Descricao = dto.Descricao
        };
        
        return await repo.CreateAsync(tema);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Tema>> GetAllAsync() => 
        await repo.GetAllAsync();

    /// <inheritdoc />
    public async Task<Tema?> GetByIdAsync(long id) => 
        await repo.GetByIdAsync(id);

    /// <inheritdoc />
    /// <remarks>
    /// Caso o identificador fornecido não seja localizado, uma exceção
    /// é disparada para interromper o pipeline de execução.
    /// </remarks>
    /// <exception cref="KeyNotFoundException">Lançada quando o ID informado não corresponde a nenhum Tema na base de dados.</exception>
    public async Task<Tema> AtualizarAsync(long id, CreateTemaDto dto)
    {
        var tema = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Tema {id} não encontrado.");

        tema.Descricao = dto.Descricao;
        
        return await repo.UpdateAsync(tema) ?? tema;
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(long id) => 
        await repo.DeleteAsync(id);
}