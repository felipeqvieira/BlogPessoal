using BlogPessoal.DTOs.Usuario;
using BlogPessoal.Models;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogPessoal.Controllers;

/// <summary>
/// Controlador -> gerencia identidade, autenticação e perfil dos usuários.
/// </summary>
[ApiController]
[Route("api/usuarios")]
public class UsuarioController(IUsuarioService service) : ControllerBase
{
    /// <summary>
    /// Registra um novo usuário no sistema. Rota pública.
    /// </summary>
    /// <param name="dto">Objeto contendo os dados de registro.</param>
    /// <returns>Dados públicos do usuário recém-criado.</returns>
    /// <response code="201">Sucesso no cadastro.</response>
    /// <response code="400">Dados inválidos ou o e-mail já está em uso.</response>
    [HttpPost("cadastrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Cadastrar([FromBody] CreateUsuarioDto dto)
    {
        var result = await service.CadastrarAsync(dto);
        return StatusCode(201, result);
    }

    /// <summary>
    /// Autentica um usuário existente e gera um Token de acesso (JWT). Rota pública.
    /// </summary>
    /// <param name="dto">As credenciais de acesso.</param>
    /// <returns>Objeto contendo o Token JWT válido.</returns>
    /// <response code="200">Autenticação realizada com sucesso. Token retornado.</response>
    /// <response code="401">E-mail ou senha incorretos.</response>
    [HttpPost("logar")]
    [AllowAnonymous]
    public async Task<IActionResult> Logar([FromBody] UsuarioLogin dto)
    {
        var token = await service.LoginAsync(dto);
        return Ok(new { token });
    }

    /// <summary>
    /// Atualiza os dados de perfil do usuário logado. Requer autenticação válida.
    /// </summary>
    /// <remarks>
    /// ID do usuário a ser atualizado não é passado pela URL, mas extraído diretamente das 
    /// "Claims" do Token JWT enviado no cabeçalho da requisição.
    /// </remarks>
    /// <param name="dto">Objeto contendo os dados a serem atualizados.</param>
    /// <returns>Novos dados do perfil do usuário.</returns>
    /// <response code="200">Perfil atualizado com sucesso.</response>
    /// <response code="401">Acesso negado. Token ausente, inválido ou expirado.</response>
    [HttpPut("atualizar")]
    [Authorize]
    public async Task<IActionResult> Atualizar([FromBody] UpdateUsuarioDto dto)
    {
        // Extrai o ID do token criptografado
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userIdString, out var userId))
            return Unauthorized(new { error = "Token inválido ou expirado." });

        var result = await service.AtualizarAsync(userId, dto);
        return Ok(result);
    }

    /// <summary>
    /// Exclui permanentemente a conta do usuário logado. Requer autenticação válida.
    /// </summary>
    /// <returns>Resposta indicando o sucesso da operação.</returns>
    /// <response code="204">Conta excluída com sucesso.</response>
    /// <response code="401">Acesso negado. Token inválido.</response>
    [HttpDelete("excluir")]
    [Authorize]
    public async Task<IActionResult> Excluir()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userIdString, out var userId))
            return Unauthorized();

        await service.ExcluirAsync(userId);
        return NoContent();
    }
}