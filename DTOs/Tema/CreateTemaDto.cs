using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.DTOs.Tema;

/// <summary>
/// DTO p/ receber informações do cliente ao criar ou atualizar 
/// uma categoria (Tema).
/// </summary>
public record CreateTemaDto(
    /// <summary>
    /// Descrição do tema. 
    /// Restrito ao limite estrutural para adequação ao bd.
    /// </summary>
    [Required][StringLength(255)] string Descricao
);