using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlogPessoal.Models;

/// <summary>
///  Assunto ao qual uma ou mais postagens pertencem.
/// </summary>
public class Tema
{
    /// <summary>
    /// Id único do tema (Chave Primária).
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Nome ou descrição do tema. Not null.
    /// </summary>
    [Required][StringLength(255)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Contém a lista de todas as postagens que estão 
    /// classificadas sob este tema.
    /// </summary>
    public ICollection<Postagem> Postagens { get; set; } = [];
}