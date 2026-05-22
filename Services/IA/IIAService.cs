using BlogPessoal.DTOs.IA;
using System.Threading.Tasks;

namespace BlogPessoal.Services.IA;

/// <summary>
/// Define o contrato abstrato para os serviços de integração com IA.
/// </summary>
public interface IIAService
{
    /// <summary>
    /// Processa de forma assíncrona o texto enviado, gerando um resumo, tags e categoria.
    /// </summary>
    /// <param name="conteudo">Conteúdo que será enviado para IA.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo um objeto do tipo <see cref="ResultadoIA"/> com as respostas da IA.</returns>
    Task<ResultadoIA> GerarResumoAsync(string conteudo);
}