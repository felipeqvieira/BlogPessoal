using System.ComponentModel.DataAnnotations;

/// <summary>
/// Objeto de Transferência de Dados (DTO) focado em receber requisições independentes 
/// para processamento e sumarização de texto via IA.
/// </summary>
/// <param name="Texto">Texto obrigatório que será analisado pela IA.</param>
public record ResumoRequestDto([Required] string Texto);