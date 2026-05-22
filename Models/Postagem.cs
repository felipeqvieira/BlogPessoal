using System;
using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.Models;

/// <summary>
/// Representa uma publicação no blog
/// </summary>
public class Postagem
{
    /// <summary>
    /// Id da postagem (Chave Primária).
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Título da postagem. Not null com no máx 100 caracteres.
    /// </summary>
    [Required][StringLength(100)]
    public string Titulo { get; set; } = string.Empty; 

    /// <summary>
    /// Corpo de texto da postagem. Not null com máx 1000 caracteres.
    /// </summary>
    [Required][StringLength(1000)]
    public string Texto { get; set; } = string.Empty; 

    /// <summary>
    /// Data e hora de criação da postagem. Em formato UTC.
    /// </summary>
    public DateTime Data { get; set; } = DateTime.UtcNow; 

    /// <summary>
    /// Resumo do texto original gerado por IA
    /// </summary>
    public string? ResumoIA    { get; set; }

    /// <summary>
    /// Tags associadas ao conteúdo, geradas pela IA.
    /// </summary>
    public string? TagsIA      { get; set; }

    /// <summary>
    /// Categoria sugerida pela IA baseada na análise do texto.
    /// </summary>
    public string? CategoriaIA { get; set; }

    /// <summary>
    /// Chave estrangeira que vincula esta postagem a um <see cref="Tema"/> específico.
    /// </summary>
    public long     TemaId    { get; set; }

    /// <summary>
    /// Propriedade de navegação para acessar os dados completos do Tema vinculado.
    /// </summary>
    public Tema?    Tema      { get; set; }

    /// <summary>
    /// Chave estrangeira que vincula esta postagem ao <see cref="Usuario"/> que a criou.
    /// </summary>
    public long     UsuarioId { get; set; }

    /// <summary>
    /// Propriedade de navegação para acessar os dados completos do Usuário autor.
    /// </summary>
    public Usuario? Usuario   { get; set; }
}