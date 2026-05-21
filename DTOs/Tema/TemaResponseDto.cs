/// <summary>
/// DTO p/ enviar as informações públicas de um Tema de volta para
/// o cliente, ocultando propriedades internas de navegação.
/// </summary>
public record TemaResponseDto(
    /// <summary>
    /// ID único gerado pelo bd.
    /// </summary>
    long Id, 

    /// <summary>
    /// A descrição do tema cadastrado.
    /// </summary>
    string Descricao
);