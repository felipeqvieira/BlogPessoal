using BlogPessoal.DTOs.Tema;
using BlogPessoal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlogPessoal.Controllers;

/// <summary>
/// Controlador -> Gerencia as requisições HTTP relacionadas aos Temas do blog.
/// Todas as rotas deste controlador exigem autenticação por padrão.
/// </summary>
[ApiController]
[Route("api/temas")]
[Authorize]
public class TemaController(ITemaService service) : ControllerBase
{
    /// <summary>
    /// Lista todos os temas cadastrados no sistema.
    /// </summary>
    /// <returns>Uma lista de temas disponíveis.</returns>
    /// <response code="200">Sucesso.</response>
    /// <response code="401">Acesso negado. Token de autenticação ausente ou inválido.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await service.GetAllAsync());

    /// <summary>
    /// Recupera um tema específico pelo seu ID único.
    /// </summary>
    /// <param name="id">O ID do tema desejado.</param>
    /// <returns>Os detalhes do tema selecionado.</returns>
    /// <response code="200">Retorna os dados do tema encontrado.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">O tema com o ID informado não foi encontrado.</response>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var tema = await service.GetByIdAsync(id);
        return tema is null ? NotFound() : Ok(tema);
    }

    /// <summary>
    /// Cria um novo tema no sistema.
    /// </summary>
    /// <param name="dto">Objeto c/ a descrição do novo tema.</param>
    /// <returns>Tema c/ novo ID.</returns>
    /// <response code="201">Sucesso na criação.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTemaDto dto)
    {
        var result = await service.CriarAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Atualiza os dados de um tema existente.
    /// </summary>
    /// <param name="id">ID do tema a ser alterado.</param>
    /// <param name="dto">Objeto contendo a nova descrição do tema.</param>
    /// <returns>O tema com os dados atualizados.</returns>
    /// <response code="200">Sucesso na atualização.</response>
    /// <response code="400">Formato dos dados incorreto.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="404">Tema solicitado não encontrado.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] CreateTemaDto dto) =>
        Ok(await service.AtualizarAsync(id, dto));

    /// <summary>
    /// Remove fisicamente um tema do sistema.
    /// </summary>
    /// <param name="id">O ID do tema a ser deletado.</param>
    /// <returns>Uma resposta vazia indicando sucesso.</returns>
    /// <response code="204">Sucesso na remoção.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.ExcluirAsync(id);
        return NoContent();
    }
}