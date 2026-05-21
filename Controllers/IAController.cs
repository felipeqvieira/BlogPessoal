using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/ia")]
[Authorize]
public class IAController(IIAService iaService) : ControllerBase
{
    /// <summary>
    /// Envia um texto para a IA e retorna resumo, tags e categoria.
    /// </summary>
    [HttpPost("resumir")]
    public async Task<IActionResult> Resumir([FromBody] ResumoRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Texto))
            return BadRequest(new { error = "Texto não pode ser vazio." });

        var resultado = await iaService.GerarResumoAsync(dto.Texto);
        return Ok(resultado);
    }
}