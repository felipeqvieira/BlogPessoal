using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// Define o contrato de regras de negócio e orquestração para a entidade <see cref="Postagem"/>.
/// </summary>
public interface IPostagemService
{
    /// <summary>
    /// Criação de uma nova postagem, efetuando o mapeamento do DTO e solicitando a geração de resumo por IA.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados de entrada validados.</param>
    /// <param name="usuarioId">Id do autor extraído do token de autenticação.</param>
    /// <returns>Postagem criada.</returns>
    Task<Postagem> CriarAsync(CreatePostagemDto dto, long usuarioId);

    /// <summary>
    /// Recupera todas as postagens ativas no sistema.
    /// </summary>
    /// <returns>Coleção assíncrona de postagens.</returns>
    Task<IEnumerable<Postagem>> GetAllAsync();

    /// <summary>
    /// Recupera postagens com base em critérios dinâmicos.
    /// </summary>
    /// <param name="autor">ID opcional do autor.</param>
    /// <param name="tema">ID opcional do tema.</param>
    /// <returns>Coleção filtrada de postagens.</returns>
    Task<IEnumerable<Postagem>> GetByFiltroAsync(long? autor, long? tema);

    /// <summary>
    /// Localiza uma postagem específica pelo seu id.
    /// </summary>
    /// <param name="id">ID único da postagem.</param>
    /// <returns>A postagem correspondente ou <c>null</c> se não for encontrada.</returns>
    Task<Postagem?> GetByIdAsync(long id);

    /// <summary>
    /// Efetua a atualização parcial dos dados de uma postagem existente.
    /// </summary>
    /// <param name="id">ID da postagem a ser alterada.</param>
    /// <param name="dto">Objeto contendo os campos que devem ser alterados.</param>
    /// <returns>A postagem atualizada.</returns>
    /// <exception cref="KeyNotFoundException">Lançada quando a postagem solicitada não existe no bd.</exception>
    Task<Postagem> AtualizarAsync(long id, UpdatePostagemDto dto);

    /// <summary>
    /// Remove uma postagem do sistema.
    /// </summary>
    /// <param name="id">ID da postagem a ser excluída.</param>
    /// <returns><c>true</c> se a operação for bem-sucedida; <c>false</c> se o registo não for encontrado.</returns>
    Task<bool> ExcluirAsync(long id);
}