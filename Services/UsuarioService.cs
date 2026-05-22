using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

/// <summary>
/// Implementação concreta da camada de serviço responsável por gerenciar a identidade, 
/// autenticação e o ciclo de vida das contas de Usuário.
/// </summary>
/// <param name="userManager">O gestor nativo do ASP.NET Core Identity responsável pela persistência e validação de segurança dos utilizadores.</param>
/// <param name="jwtService">O serviço de infraestrutura responsável pela geração e assinatura dos tokens criptográficos JWT.</param>
public class UsuarioService(
    UserManager<Usuario> userManager,
    JwtService           jwtService)
    : IUsuarioService
{
    /// <summary>
    /// Efetua o mapeamento interno e isolamento da entidade de domínio <see cref="Usuario"/> 
    /// para o objeto plano de saída <see cref="UsuarioResponseDto"/>.
    /// </summary>
    /// <remarks>
    /// Auxilia a mitigar o risco de exposição de dados confidenciais herdados de <see cref="IdentityUser{TKey}"/>,
    /// como carimbos de segurança e hashes de palavra-passe.
    /// </remarks>
    /// <param name="u">A instância da entidade Usuário carregada da base de dados.</param>
    /// <returns>O DTO contendo apenas as informações públicas e seguras do perfil.</returns>
    private static UsuarioResponseDto ToDto(Usuario u) =>
        new(u.Id, u.Nome, u.Email!, u.Foto);

    /// <inheritdoc />
    /// <exception cref="ValidationException">Lançada caso o Identity rejeite os dados por violação de políticas de senha ou e-mails duplicados.</exception>
    public async Task<UsuarioResponseDto> CadastrarAsync(CreateUsuarioDto dto)
    {
        var usuario = new Usuario
        {
            Nome     = dto.Nome,
            Email    = dto.Email,
            UserName = dto.Email, // Define o e-mail como nome de utilizador padrão p/ o login
            Foto     = dto.Foto
        };

        var result = await userManager.CreateAsync(usuario, dto.Senha);

        if (!result.Succeeded)
            throw new ValidationException(
                string.Join("; ", result.Errors.Select(e => e.Description)));

        // Atribui o papel de acesso padrão para novos utilizadores do sistema
        await userManager.AddToRoleAsync(usuario, "User");
        return ToDto(usuario);
    }

    /// <inheritdoc />
    /// <exception cref="UnauthorizedAccessException">Lançada caso o e-mail não seja localizado ou a psenha informada seja inválida.</exception>
    public async Task<string> LoginAsync(UsuarioLogin login)
    {
        var usuario = await userManager.FindByEmailAsync(login.Email)
            ?? throw new UnauthorizedAccessException("Credenciais inválidas.");

        var valid = await userManager.CheckPasswordAsync(usuario, login.Senha);
        if (!valid)
            throw new UnauthorizedAccessException("Credenciais inválidas.");

        return await jwtService.GenerateTokenAsync(usuario);
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Lançada caso o ID fornecido não corresponda a nenhum registro.</exception>
    public async Task<UsuarioResponseDto> AtualizarAsync(long id, UpdateUsuarioDto dto)
    {
        // O Identity opera nativamente com chaves em formato string, exigindo a conversão do ID
        var usuario = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");

        usuario.Nome = dto.Nome ?? usuario.Nome;
        usuario.Foto = dto.Foto ?? usuario.Foto;

        await userManager.UpdateAsync(usuario);
        return ToDto(usuario);
    }

    /// <inheritdoc />
    /// <exception cref="KeyNotFoundException">Lançada caso o Usuário a ser removido não exista na base de dados.</exception>
    public async Task<bool> ExcluirAsync(long id)
    {
        var usuario = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException($"Usuário {id} não encontrado.");

        var result = await userManager.DeleteAsync(usuario);
        return result.Succeeded;
    }
}