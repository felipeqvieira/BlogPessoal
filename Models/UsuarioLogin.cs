using System.ComponentModel.DataAnnotations;

/// <summary>
/// Transporta as credenciais de acesso do cliente para o servidor durante 
/// o processo de autenticação.
/// </summary>
public class UsuarioLogin
{
    /// <summary>
    /// Endereço de e-mail do usuário. A anotação <c>[EmailAddress]</c> valida 
    /// o formato padrão
    /// </summary>
    [Required][EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha informada pelo usuário para validação.
    /// </summary>
    [Required]
    public string Senha { get; set; } = string.Empty;
}