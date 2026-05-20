using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO p/ o registro de um novo usuário.
/// Contém as credenciais mínimas obrigatórias para a criação de um ID no sistema.
/// </summary>
public record CreateUsuarioDto(
    /// <summary>Nome do usuário. Máximo de 100 caracteres.</summary>
    [Required][StringLength(100)] string Nome,

    /// <summary>E-mail p/ login. Validado quanto ao formato.</summary>
    [Required][EmailAddress]      string Email,

    /// <summary>Senha do usuário. Ccriptografada (hash) pela camada de Serviço antes de ir ao banco.</summary>
    [Required]                    string Senha,

    /// <summary>A URL opcional p/ a foto de perfil. Deve ser um link de web válido.</summary>
    [Url]                         string? Foto
);