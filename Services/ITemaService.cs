using BlogPessoal.DTOs.Tema;
using BlogPessoal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPessoal.Services;

/// <summary>
/// Define o contrato abstrato para as regras de negócio e gerenciamento da entidade <see cref="Tema"/>.
/// </summary>
public interface ITemaService
{
    /// <summary>
    /// Processa a criação e validação de um tema de forma assíncrona.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados de entrada fornecidos pelo swagger.</param>
    /// <returns>Tarefa que representa a operação assíncrona, contendo a entidade <see cref="Tema"/> criada e com seus dados.</returns>
    Task<Tema> CriarAsync(CreateTemaDto dto);

    /// <summary>
    /// Lista todos os temas.
    /// </summary>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a lista de temas.</returns>
    Task<IEnumerable<Tema>> GetAllAsync();

    /// <summary>
    /// Localiza uma tema utilizando o seu ID.
    /// </summary>
    /// <param name="id">ID do tema pesquisado.</param>
    /// <returns>Tarefa contendo o tema localizado, ou <c>null</c> caso não exista no banco.</returns>
    Task<Tema?> GetByIdAsync(long id);

    /// <summary>
    /// Modifica descrição de um tema existente.
    /// </summary>
    /// <param name="id">ID do tema que sofrerá a alteração.</param>
    /// <param name="dto">Objeto de transferência contendo os novos dados a serem aplicados.</param>
    /// <returns>Uma tarefa contendo a instância modificada do tema.</returns>
    Task<Tema> AtualizarAsync(long id, CreateTemaDto dto);

    /// <summary>
    /// Remove logicamente ou fisicamente uma categoria da aplicação.
    /// </summary>
    /// <param name="id">ID do tema a ser excluído.</param>
    /// <returns>Uma tarefa contendo um valor booleano indicando se a exclusão foi realizada com sucesso.</returns>
    Task<bool> ExcluirAsync(long id);
}