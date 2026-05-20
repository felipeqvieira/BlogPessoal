/// <summary>
/// DTO de saída. Representa o perfil público de um usuário.
/// </summary>
public record UsuarioResponseDto(
    /// <summary>ID único gerado pelo bd p/ o usuário.</summary>
    long    Id,

    /// <summary>Nome registrado no sistema.</summary>
    string  Nome,

    /// <summary>E-mail público de contato ou login do usuário.</summary>
    string  Email,

    /// <summary>URL para a imagem do usuário (opcional).</summary>
    string? Foto
);