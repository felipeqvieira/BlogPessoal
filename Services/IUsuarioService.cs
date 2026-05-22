using System.Threading.Tasks;

/// <summary>
/// Define o contrato abstrato para as regras de negócio, autenticação e gestão de ciclo de vida dos usuários.
/// </summary>
public interface IUsuarioService
{
    /// <summary>
    /// Registra um usuário na aplicação de forma assíncrona.
    /// </summary>
    /// <param name="dto">Objeto de transferência contendo as credenciais e dados obrigatórios de cadastro.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo o <see cref="UsuarioResponseDto"/> com os dados públicos gerados.</returns>
    Task<UsuarioResponseDto> CadastrarAsync(CreateUsuarioDto dto);

    /// <summary>
    /// Valida as credenciais fornecidas pelo usuário e emite um token de acesso assinado.
    /// </summary>
    /// <param name="login">Objeto contendo o e-mail de login e a palavra-passe em texto plano.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a string do Token JWT gerado.</returns>
    Task<string> LoginAsync(UsuarioLogin login);

    /// <summary>
    /// Modifica os dados públicos do perfil de um usuário existente de forma assíncrona.
    /// </summary>
    /// <param name="id">ID do utilizador que sofrerá as alterações.</param>
    /// <param name="dto">O objeto de transferência contendo os novos dados opcionais de perfil (Nome ou Foto).</param>
    /// <returns>Uma tarefa contendo o <see cref="UsuarioResponseDto"/> com as informações atualizadas.</returns>
    Task<UsuarioResponseDto> AtualizarAsync(long id, UpdateUsuarioDto dto);

    /// <summary>
    /// Remove de forma definitiva uma conta de usuário do sistema.
    /// </summary>
    /// <param name="id">ID do utilizador a ser excluído.</param>
    /// <returns>Uma tarefa contendo um valor booleano indicando o sucesso da operação de exclusão.</returns>
    Task<bool> ExcluirAsync(long id);
}