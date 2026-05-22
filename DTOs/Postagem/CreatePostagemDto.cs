using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs.Postagem;

/// <summary>
/// DTO p/ criação de uma nova postagem.
/// Valida as regras de negócio de entrada antes mesmo de a requisição chegar à camada de Serviço.
/// </summary>
public record CreatePostagemDto(
    /// <summary>
    /// Título da postagem. Not null e limitado a 100 caracteres.
    /// </summary>
    [Required][StringLength(100)]  string Titulo,

    /// <summary>
    /// Conteúdo da postagem. Not null e limitado a 1000 caracteres.
    /// </summary>
    [Required][StringLength(1000)] string Texto,

    /// <summary>
    /// O ID do Tema escolhido p/ esta postagem.
    /// </summary>
    [Required] long? TemaId
);