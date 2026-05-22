using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace BlogPessoal.Middlewares;

/// <summary>
/// Intercetor global de requisições HTTP encarregado de capturar, catalogar 
/// e formatar exceções não tratadas ao longo do pipeline de execução da aplicação.
/// </summary>
/// <param name="next">Delegate que representa o próximo middleware no pipeline do ASP.NET Core.</param>
/// <param name="logger">Serviço de log utilizado para registar falhas críticas não mapeadas no servidor.</param>
public class ExceptionMiddleware(
    RequestDelegate                  next,
    ILogger<ExceptionMiddleware> logger)
{
    /// <summary>
    /// Invoca a execução da requisição atual, envolvendo-a num bloco de proteção global.
    /// </summary>
    /// <param name="ctx">Contexto encapsulado da requisição e resposta HTTP atual.</param>
    /// <returns>Tarefa representando a operação assíncrona do pipeline.</returns>
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            // Repassa a bola para o próximo componente. Se não houver erros nas camadas inferiores, 
            // a resposta flui normalmente de volta para o cliente.
            await next(ctx);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(ctx, 401, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteError(ctx, 404, ex.Message);
        }
        catch (ValidationException ex)
        {
            await WriteError(ctx, 400, ex.Message);
        }
        catch (Exception ex)
        {
            // Regista o erro completo para depuração
            logger.LogError(ex, "Erro inesperado");
            // Retorna uma mensagem genérica para não expor a arquitetura interna do sistema
            await WriteError(ctx, 500, "Erro interno do servidor.");
        }
    }

    /// <summary>
    /// Método utilitário responsável por montar e padronizar a estrutura do corpo da resposta em formato JSON.
    /// </summary>
    /// <param name="ctx">Contexto atual da requisição.</param>
    /// <param name="code">Código de estado HTTP a ser devolvido.</param>
    /// <param name="msg">Mensagem descritiva amigável do erro.</param>
    /// <returns>Tarefa assíncrona de escrita na stream de resposta.</returns>
    private static async Task WriteError(HttpContext ctx, int code, string msg)
    {
        ctx.Response.StatusCode  = code;
        ctx.Response.ContentType = "application/json";
        
        // Escreve um objeto anônimo formatado em JSON diretamente na saída da resposta HTTP
        await ctx.Response.WriteAsJsonAsync(new { error = msg });
    }
}