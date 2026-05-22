using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Integração assíncrona com a API externa do Groq p/ análise do texto da postagem.
/// </summary>
/// <param name="httpFactory">A fábrica de clientes HTTP utilizada para gerenciar o pool de conexões do <see cref="HttpClient"/>.</param>
/// <param name="config">A infraestrutura de configuração para leitura segura das chaves e parâmetros do serviço.</param>
/// <param name="logger">O componente de log para registro operacional de erros e avisos.</param>
public class GroqService(
    IHttpClientFactory    httpFactory,
    IConfiguration        config,
    ILogger<GroqService>  logger)
    : IIAService
{
    /// <summary>
    /// Envia o conteúdo do texto de uma postagem de forma assíncrona para a API do Groq e retorna os metadados refinados de resumo, tags e categoria.
    /// </summary>
    /// <param name="conteudo">Conteúdo da postagem a ser analisado pela inteligência artificial.</param>
    /// <returns>Tarefa que representa a operação assíncrona, contendo uma instância preenchida ou vazia de <see cref="ResultadoIA"/>.</returns>
    public async Task<ResultadoIA> GerarResumoAsync(string conteudo)
    {
        var apiKey = config["Groq:ApiKey"]!;
        var model  = config["Groq:Model"]!;

        // URL lida obrigatoriamente da config
        var url = config["Groq:ApiUrl"]!;

        using var client = httpFactory.CreateClient("Groq");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        // Montagem do payload anônimo dividindo a instrução por papéis aceitos pelo padrão OpenAI
        var payload = new
        {
            model,
            messages = new[]
            {
                new { role = "system",
                      content = "Você é um assistente que analisa textos de blog." },
                new { role = "user",
                      content = PromptBuilder.BuildResumoPrompt(conteudo) }
            },
            temperature = 0.3, // Reduz o nível de criatividade do modelo para conter alucinações de dados
            max_tokens  = 300  // Restringe o tamanho máximo de tokens gerados para otimização de banda de rede
        };

        HttpResponseMessage response;
        try
        {
            // Despacha a requisição HTTP POST de forma assíncrona liberando as threads do servidor
            response = await client.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // se o Groq falhar, registra-se o aviso mas a postagem é mantida salva no banco
            logger.LogWarning(ex,
                "Falha ao chamar a API do Groq. Postagem será salva sem enriquecimento.");
            return new ResultadoIA();
        }

        var result = await response.Content.ReadFromJsonAsync<GroqResponse>();
        var json   = result?.Choices?[0]?.Message?.Content ?? "{}";

        // Remove blocos de markdown caso o modelo os inclua no corpo da mensagem
        json = json.Trim();
        if (json.StartsWith("```"))
        {
            var start = json.IndexOf('\n') + 1;
            var end   = json.LastIndexOf("```");
            if (end > start)
                json = json[start..end].Trim();
        }

        try
        {
            // Desserializa a string de texto purificada para o DTO estruturado ResultadoIA usado pelo domínio
            return JsonSerializer.Deserialize<ResultadoIA>(json,
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? new ResultadoIA();
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Resposta do Groq não pôde ser desserializada: {Json}", json);
            return new ResultadoIA();
        }
    }

    /// Records privados marcados como sealed para otimização de performance pelo compilador .NET
    /// <summary> DTO interno para mapeamento da coleção de escolhas devolvida pelo Groq/OpenAI.</summary>
    private sealed record GroqResponse(List<Choice>? Choices);
    /// <summary>DTO interno que encapsula o objeto da mensagem individual gerada pelo modelo.</summary>
    private sealed record Choice(Message? Message);
    /// <summary>DTO interno focado na extração da string de texto contendo os metadados brutos.</summary>
    private sealed record Message(string? Content);
}