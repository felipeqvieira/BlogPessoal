using System.ComponentModel.DataAnnotations;

/// <summary>
/// DTO p/ atualização de perfil de usuário.
/// Não trata e-mail e senha aqui.
/// </summary>
public record UpdateUsuarioDto(
    /// <summary>Novo nome. Se nulo, mantém o nome atual.</summary>
    [StringLength(100)] string? Nome,

    /// <summary>A nova URL. Se nulo, mantém a foto atual.</summary>
    [Url]               string? Foto
);