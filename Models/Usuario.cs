using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Representa um usuário registrado no sistema. 
/// Herda de <see cref="IdentityUser{TKey}"/> para aproveitar a infraestrutura 
/// pronta de autenticação do ASP.NET Core
/// </summary>
public class Usuario : IdentityUser<long>
{

  /// <summary>
  /// Nome de exibição público do usuário.
  /// </summary>
  [Required]
  [StringLength(100)]
  public string Nome {get; set;} = string.Empty;

  /// <summary>
  /// URL para a imagem de perfil do usuário. A anotação garante um formato de link válido.
  /// </summary>
  [Url]
  public string? Foto {get; set;}

  /// <summary>
  /// Propriedade de navegação. Representa o acervo de postagens publicadas por este autor.
  /// </summary>
  public ICollection<Postagem> Postagens {get; set;} = [];

}