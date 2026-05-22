using System;

namespace BlogPessoal.DTOs.Postagem;

/// <summary>
/// DTO de saída. Responsável por formatar os dados da postagem 
/// para o Swagger, garantindo que as complexas entidades de banco de dados 
/// não sejam expostas diretamente.
/// </summary>
public record PostagemResponseDto(
    /// <summary>O ID único da postagem gerado pelo bd.</summary>
    long     Id,

    /// <summary>Título da postagem.</summary>
    string   Titulo,

    /// <summary>Texto da postagem.</summary>
    string   Texto,

    /// <summary>Data e hora (em padrão UTC) em que a postagem foi inserida no sistema.</summary>
    DateTime Data,

    /// <summary>Resumo do conteúdo feito pela Inteligência Artificial.</summary>
    string?  ResumoIA,

    /// <summary>Tags extraídas pelo processamento de IA.</summary>
    string?  TagsIA,

    /// <summary>Categoria sugerida pela IA.</summary>
    string?  CategoriaIA,

    /// <summary>
    /// Tema ao qual a postagem pertence.
    /// </summary>
    string   TemaDescricao,

    /// <summary>
    /// Autor da postagem.
    /// </summary>
    string   UsuarioNome
);