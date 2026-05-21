using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Implementação concreta da camada de serviços para a entidade <see cref="Postagem"/>.
/// </summary>
/// <param name="repo">Repositório responsável pelas operações de BD.</param>
/// <param name="iaService">Serviço de integração com a IA.</param>
public class PostagemService(
    IPostagemRepository repo,      
    IIAService          iaService)
    : IPostagemService
{
    /// <inheritdoc />
    /// <remarks>
    /// Primeiro, a postagem é guardada no banco para apoiar a integridade do texto original. 
    /// Segundo, a chamada externa para a IA é feita. Em caso de falha da IA, a postagem continua guardada sem o resumo.
    /// </remarks>
    public async Task<Postagem> CriarAsync(CreatePostagemDto dto, long usuarioId)
    {
        var postagem = new Postagem
        {
            Titulo    = dto.Titulo,
            Texto     = dto.Texto,
            TemaId    = dto.TemaId,
            UsuarioId = usuarioId
        };

        // persiste independente da IA
        postagem = await repo.CreateAsync(postagem);

        // Se funcionar, atualiza com resumo. Caso contrário, continua com post sem resumo.
        var resultado = await iaService.GerarResumoAsync(dto.Texto);

        if (!string.IsNullOrEmpty(resultado.Resumo))
        {
            postagem.ResumoIA    = resultado.Resumo;
            postagem.TagsIA      = resultado.Tags;
            postagem.CategoriaIA = resultado.Categoria;
            postagem = await repo.UpdateAsync(postagem) ?? postagem;
        }

        return postagem;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Postagem>> GetAllAsync() =>
        await repo.GetAllAsync();

    /// <inheritdoc />
    public async Task<IEnumerable<Postagem>> GetByFiltroAsync(long? autor, long? tema) =>
        await repo.GetByFiltroAsync(autor, tema);

    /// <inheritdoc />
    public async Task<Postagem?> GetByIdAsync(long id) =>
        await repo.GetByIdAsync(id);

    /// <inheritdoc />
    /// <remarks>
    /// Se os dados não forem alterados, mantém os antigos.
    /// </remarks>
    public async Task<Postagem> AtualizarAsync(long id, UpdatePostagemDto dto)
    {
        // Tenta encontrar id no banco de dados
        var postagem = await repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Postagem {id} não encontrada.");

        // se não foi alterado, mantém
        postagem.Titulo = dto.Titulo ?? postagem.Titulo;
        postagem.Texto  = dto.Texto  ?? postagem.Texto;
        if (dto.TemaId.HasValue) postagem.TemaId = dto.TemaId.Value;

        return await repo.UpdateAsync(postagem) ?? postagem;
    }

    /// <inheritdoc />
    public async Task<bool> ExcluirAsync(long id) =>
        await repo.DeleteAsync(id);
}