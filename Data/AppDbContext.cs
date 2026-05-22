using BlogPessoal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogPessoal.Data;

/// <summary>
/// Contexto principal de banco de dados da aplicação.
/// Gerencia as sessões de conexão, transações e o mapeamento objeto-relacional (ORM) via Entity Framework Core.
/// </summary>
/// <remarks>
/// Herda de <see cref="IdentityDbContext{TUser, TRole, TKey}"/> para integrar nativamente as tabelas de 
/// autenticação e gerenciamento de acessos do ASP.NET Core Identity usando chaves primárias do tipo <see cref="long"/>.
/// </remarks>
/// <param name="options">Opções de configuração do contexto, como provedor de banco e string de conexão.</param>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<Usuario, IdentityRole<long>, long>(options)
{
    /// <summary>
    /// Fornece acesso à tabela de <see cref="Tema"/> no bd.
    /// </summary>
    public DbSet<Tema>     Temas     => Set<Tema>();

    /// <summary>
    /// Fornece acesso à tabela de <see cref="Postagem"/> no bd.
    /// </summary>
    public DbSet<Postagem> Postagens => Set<Postagem>();

    /// <summary>
    /// Configura os mapeamentos declarativos das entidades, restrições e relacionamentos usando a Fluent API.
    /// </summary>
    /// <param name="builder">O construtor de modelos utilizado para configurar o esquema do bd.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Garante a execução da configuração base do IdentityDbContext, criando tabelas como AspNetUsers, AspNetRoles, etc.
        base.OnModelCreating(builder); 

        // Configuração detalhada das regras de negócio e chaves estrangeiras da entidade Postagem
        builder.Entity<Postagem>(e => {
            
            // Define a relação de Muitos-para-Um
            e.HasOne(p => p.Tema)
             .WithMany(t => t.Postagens)
             .HasForeignKey(p => p.TemaId)
             // Impede a exclusão de um Tema caso existam postagens vinculadas a ele.
             .OnDelete(DeleteBehavior.Restrict); 

            // Define a relação de Muitos-para-Um
            e.HasOne(p => p.Usuario)
             .WithMany(u => u.Postagens)
             .HasForeignKey(p => p.UsuarioId)
             //Se um usuário for excluído do sistema, todas as suas postagens associadas serão removidas automaticamente.
             .OnDelete(DeleteBehavior.Cascade); 
        });
    }
}