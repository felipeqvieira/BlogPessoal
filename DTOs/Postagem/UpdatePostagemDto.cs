using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs.Postagem;

/// <summary>
/// DTO projetado p/ atualizações parciais (PATCH/PUT) de uma postagem.
/// Todos os campos são opcionais.
/// </summary>
public record UpdatePostagemDto(
    /// <summary>
    /// Novo título da postagem. Se não for enviado, o título original será mantido.
    /// Mantém a restrição de tamanho máximo.
    /// </summary>
    [StringLength(100)]  string? Titulo,

    /// <summary>
    /// Novo corpo de texto da postagem. Se null, o texto original será preservado.
    /// </summary>
    [StringLength(1000)] string? Texto,

    /// <summary>
    /// Novo ID de Tema. Se null, a postagem continuará com o ID de tema anterior.
    /// </summary>
    long? TemaId
);