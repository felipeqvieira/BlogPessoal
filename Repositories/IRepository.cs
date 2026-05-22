using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlogPessoal.Repositories;

/// <summary>
/// Interface - abstrai as operações de CRUD.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Pega todos os registros no bd
    /// </summary>
    /// <returns>Coleção assíncrona com as entidades</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Busca entidade pelo id
    /// </summary>
    /// <param name="id">Chave primária da entidade</param>
    /// <returns>A entidade ou null caso não exista</returns>
    Task<T?>             GetByIdAsync(long id);

    /// <summary>
    /// Insere uma nova entidade no bd
    /// </summary>
    /// <param name="entity">Objeto da entidade</param>
    /// <returns>A entidade criada com novo id</returns>
    Task<T>              CreateAsync(T entity);

    /// <summary>
    /// Atualiza dados de entidade existente no bd
    /// </summary>
    /// <param name="entity">O objeto com os dados atualizados</param>
    /// <returns>A entidade atualizada ou null se falhar</returns>
    Task<T?>             UpdateAsync(T entity);

    /// <summary>
    /// Exclui uma entidade do bd pelo id
    /// </summary>
    /// <param name="id">Chave primária da entidade a ser excluída</param>
    /// <returns>True se deu certo; false caso falhe</returns>
    Task<bool>           DeleteAsync(long id);
}