using BlogPessoal.DTOs.Postagem;
using BlogPessoal.Models;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogPessoal.Controllers;

/// <summary>
/// Controlador -> Gerenciar as requisições HTTP relacionadas às Postagens do blog.
/// Autenticação de token por padrão, mas tem exceções p/ leitura
/// </summary>
[ApiController]
[Route("api/postagens")]
[Authorize]
public class PostagemController(IPostagemService service) : ControllerBase
{
    // Mapeia Postagem -> PostagemResponseDto p/ evitar ciclo de referência
    private static PostagemResponseDto ToDto(Postagem p) => new(
        p.Id,
        p.Titulo,
        p.Texto,
        p.Data,
        p.ResumoIA,
        p.TagsIA,
        p.CategoriaIA,
        p.Tema?.Descricao    ?? string.Empty,
        p.Usuario?.Nome      ?? string.Empty
    );

    /// <summary>
    /// Lista c/ todas as postagens cadastradas no sistema.
    /// </summary>
    /// <returns>Uma lista de objetos <see cref="PostagemResponseDto"/>.</returns>
    /// <response code="200">Sucesso</response>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var postagens = await service.GetAllAsync();
        return Ok(postagens.Select(ToDto));
    }

    /// <summary>
    /// Filtra as postagens do blog utilizando parâmetros opcionais.
    /// </summary>
    /// <param name="autor">id único do autor (opcional).</param>
    /// <param name="tema">id único do tema (opcional).</param>
    /// <returns>Lista de postagens que correspondem aos critérios de busca.</returns>
    /// <response code="200">Sucesso.</response>
    [HttpGet("filtro")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByFiltro(
        [FromQuery] long? autor,
        [FromQuery] long? tema)
    {
        var postagens = await service.GetByFiltroAsync(autor, tema);
        return Ok(postagens.Select(ToDto));
    }

    /// <summary>
    /// Recupera uma postagem específica pelo seu id único.
    /// </summary>
    /// <param name="id">Id da postagem desejada.</param>
    /// <returns>Detalhes da postagem selecionada.</returns>
    /// <response code="200">Retorna os dados da postagem encontrada.</response>
    /// <response code="404">Postagem com o ID informado não foi encontrada no banco de dados.</response>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(long id)
    {
        var postagem = await service.GetByIdAsync(id);
        return postagem is null ? NotFound() : Ok(ToDto(postagem));
    }

    /// <summary>
    /// Cria uma nova postagem no blog. Requer um token de autenticação válido.
    /// </summary>
    /// <remarks>
    /// ID do autor é extraído automaticamente do Token JWT fornecido no cabeçalho da requisição.
    /// Envia o texto para processamento de I.A. de forma assíncrona.
    /// </remarks>
    /// <param name="dto">Objeto contendo o título, texto e tema da nova postagem.</param>
    /// <returns>Objeto recém-criado, incluindo IDs gerados e links de acesso.</returns>
    /// <response code="201">Postagem criada com sucesso.</response>
    /// <response code="400">Dados são inválidos ou estão malformados.</response>
    /// <response code="401">Sem autenticação ou o token fornecido é inválido.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePostagemDto dto)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(userIdString, out var userId))
            return Unauthorized(new { error = "Token inválido." });

        var result = await service.CriarAsync(dto, userId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ToDto(result));
    }

    /// <summary>
    /// Atualiza os dados de uma postagem existente. Permite atualizações parciais.
    /// </summary>
    /// <param name="id">ID da postagem a ser alterada.</param>
    /// <param name="dto">Objeto contendo os novos dados.</param>
    /// <returns>Postagem com os dados atualizados.</returns>
    /// <response code="200">Atualizada com sucesso.</response>
    /// <response code="400">Formato dos dados enviados está incorreto.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Postagem não foi encontrada.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] UpdatePostagemDto dto)
    {
        var result = await service.AtualizarAsync(id, dto);
        return Ok(ToDto(result));
    }

    /// <summary>
    /// Remove fisicamente uma postagem do sistema.
    /// </summary>
    /// <param name="id">ID da postagem a ser deletada.</param>
    /// <returns>Uma resposta vazia indicando sucesso.</returns>
    /// <response code="204">Postagem excluída com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.ExcluirAsync(id);
        return NoContent();
    }
}