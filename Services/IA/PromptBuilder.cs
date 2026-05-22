/// <summary>
/// Classe utilitária estática responsável por construir, formatar e padronizar 
/// o prompt enviado para a IA
/// </summary>
public static class PromptBuilder
{
    /// <summary>
    /// Constrói o prompt formatado para solicitar a um modelo de IA (LLM) que gere 
    /// um resumo, extraia tags e defina uma categoria com base num texto fornecido.
    /// </summary>
    /// <param name="conteudo">Conteúdo da postagem.</param>
    /// <returns>Uma string formatada contendo as diretrizes de sistema, o esquema JSON exigido e o texto do usuário.</returns>
    public static string BuildResumoPrompt(string conteudo) => $$"""
        Analise o texto de uma postagem de blog e retorne SOMENTE um JSON
        com este formato exato (sem markdown, sem explicação adicional):
        {
          "resumo": "resumo em até 2 frases",
          "tags": "tag1, tag2, tag3",
          "categoria": "categoria principal"
        }

        Texto da postagem:
        {{conteudo}}
        """;
}