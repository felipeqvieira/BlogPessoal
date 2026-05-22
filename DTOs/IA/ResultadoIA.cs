namespace BlogPessoal.DTOs.IA;

/// <summary>
/// DTO interno utilizado para mapear e estruturar a resposta em formato JSON
/// proveniente da integração com a Inteligência Artificial.
/// </summary>
public class ResultadoIA
{
    /// <summary>
    /// Resumo gerado pela IA com base na postagem original.
    /// Inicializado como string vazia para evitar exceções de referência nula.
    /// </summary>
    public string Resumo    { get; set; } = string.Empty;

    /// <summary>
    /// Tags sugeridas pela IA.
    /// </summary>
    public string Tags      { get; set; } = string.Empty;

    /// <summary>
    /// Categoria sugerida pela IA para o conteúdo analisado.
    /// </summary>
    public string Categoria { get; set; } = string.Empty;
}