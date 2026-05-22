namespace BlogPessoal.Tests;

using FluentAssertions;
using NSubstitute;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

/// <summary>
/// Conjunto de testes unitários dedicado à validação das regras de negócio do <see cref="PostagemService"/>.
/// </summary>
public class PostagemServiceTests
{
    // Instanciação dos atores falsos (Mocks) para isolar o serviço de infraestruturas reais.
    private readonly IPostagemRepository   _repo    = Substitute.For<IPostagemRepository>();
    private readonly IIAService            _ia      = Substitute.For<IIAService>();
    
    // Método de fábrica para instanciar o serviço injetando as dependências falsas.
    private          PostagemService       Service() => new(_repo, _ia);

    /// <summary>
    /// Valida o fluxo de criação de postagem quando a IA responde de forma bem-sucedida,
    /// verificando se as propriedades geradas são corretamente mapeadas na entidade.
    /// </summary>
    [Fact]
    public async Task Criar_DevePreencherCamposIA_QuandoIARetornaResultadoValido()
    {
        // Arrange: Prepara o palco e instrui os atores sobre como devem responder
        var dto      = new CreatePostagemDto("Título", "Texto longo de teste.", 1L);
        var postagem = new Postagem { Id = 1, Titulo = dto.Titulo, Texto = dto.Texto };

        _repo.CreateAsync(Arg.Any<Postagem>()).Returns(postagem);
        _repo.UpdateAsync(Arg.Any<Postagem>()).Returns(postagem);

        _ia.GerarResumoAsync(Arg.Any<string>()).Returns(new ResultadoIA
        {
            Resumo    = "Resumo gerado.",
            Tags      = "tag1, tag2",
            Categoria = "Tecnologia"
        });

        //Executa o método alvo do teste
        var result = await Service().CriarAsync(dto, usuarioId: 1L);

        // Avalia o estado final e verifica se as interações esperadas ocorreram
        result.Should().NotBeNull();
        result.ResumoIA.Should().Be("Resumo gerado.");
        result.TagsIA.Should().Be("tag1, tag2");
        result.CategoriaIA.Should().Be("Tecnologia");

        // Verifica se os atores (Mocks) foram chamados exatamente 1 vez
        await _ia.Received(1).GerarResumoAsync(dto.Texto);
        await _repo.Received(1).CreateAsync(Arg.Any<Postagem>());
        await _repo.Received(1).UpdateAsync(Arg.Any<Postagem>());
    }

    /// <summary>
    /// Avalia o comportamento de degradação graciosa do serviço quando a integração com a IA falha ou retorna dados vazios.
    /// </summary>
    [Fact]
    public async Task Criar_DeveRetornarPostagemSemIA_QuandoIARetornaVazio()
    {
        // Arrange
        var dto      = new CreatePostagemDto("Título", "Texto.", 1L);
        var postagem = new Postagem { Id = 1 };

        _repo.CreateAsync(Arg.Any<Postagem>()).Returns(postagem);
        _ia.GerarResumoAsync(Arg.Any<string>()).Returns(new ResultadoIA());

        // Act
        var result = await Service().CriarAsync(dto, usuarioId: 1L);

        // postagem criada mas campos IA permanecem vazios
        result.Should().NotBeNull();
        result.ResumoIA.Should().BeNullOrEmpty();

        // O método UpdateAsync não deve ser chamado quando a IA retorna vazio, poupando a base de dados
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<Postagem>());
    }

    [Fact]
    public async Task GetAll_DeveRetornarTodasAsPostagens()
    {
        // Arrange
        var lista = new List<Postagem>
        {
            new() { Id = 1, Titulo = "Post A" },
            new() { Id = 2, Titulo = "Post B" }
        };
        _repo.GetAllAsync().Returns(lista);

        // Act
        var result = await Service().GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        await _repo.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetById_DeveRetornarPostagem_QuandoExiste()
    {
        // Arrange
        var postagem = new Postagem { Id = 5, Titulo = "Post Existente" };
        _repo.GetByIdAsync(5L).Returns(postagem);

        // Act
        var result = await Service().GetByIdAsync(5L);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(5L);
    }

    [Fact]
    public async Task GetById_DeveRetornarNull_QuandoNaoExiste()
    {
        // Arrange
        _repo.GetByIdAsync(Arg.Any<long>()).Returns((Postagem?)null);

        // Act
        var result = await Service().GetByIdAsync(999L);

        // Assert
        result.Should().BeNull();
    }

    /// <summary>
    /// Valida se o serviço rejeita corretamente tentativas de modificação em registos inexistentes.
    /// </summary>
    [Fact]
    public async Task Atualizar_DeveLancarExcecao_QuandoPostagemNaoExiste()
    {
        // Arrange
        _repo.GetByIdAsync(Arg.Any<long>()).Returns((Postagem?)null);
        var dto = new UpdatePostagemDto("Novo Título", null, null);

        // Quando testamos exceções assíncronas, encapsulamos a chamada numa função (Func)
        var act = async () => await Service().AtualizarAsync(999L, dto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task Excluir_DeveRetornarTrue_QuandoPostagemExiste()
    {
        // Arrange
        _repo.DeleteAsync(1L).Returns(true);

        // Act
        var result = await Service().ExcluirAsync(1L);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).DeleteAsync(1L);
    }
}

/// <summary>
/// Conjunto de testes unitários dedicado à validação do <see cref="TemaService"/>.
/// </summary>
public class TemaServiceTests
{
    private readonly IRepository<Tema> _repo    = Substitute.For<IRepository<Tema>>();
    private          TemaService       Service() => new(_repo);

    [Fact]
    public async Task Criar_DeveRetornarTemaCriado()
    {
        // Arrange
        var dto  = new CreateTemaDto("Tecnologia");
        var tema = new Tema { Id = 1, Descricao = dto.Descricao };
        _repo.CreateAsync(Arg.Any<Tema>()).Returns(tema);

        // Act
        var result = await Service().CriarAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Descricao.Should().Be("Tecnologia");
        await _repo.Received(1).CreateAsync(Arg.Any<Tema>());
    }

    [Fact]
    public async Task Atualizar_DeveLancarExcecao_QuandoTemaNaoExiste()
    {
        // Arrange
        _repo.GetByIdAsync(Arg.Any<long>()).Returns((Tema?)null);

        // Act
        var act = async () => await Service().AtualizarAsync(999L, new CreateTemaDto("X"));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task Excluir_DeveRetornarTrue_QuandoTemaExiste()
    {
        // Arrange
        _repo.DeleteAsync(2L).Returns(true);

        // Act
        var result = await Service().ExcluirAsync(2L);

        // Assert
        result.Should().BeTrue();
        await _repo.Received(1).DeleteAsync(2L);
    }

    [Fact]
    public async Task GetAll_DeveRetornarTodosOsTemas()
    {
        var lista = new List<Tema>
        {
            new() { Id = 1, Descricao = "Tecnologia" },
            new() { Id = 2, Descricao = "Ciência" }
        };
        _repo.GetAllAsync().Returns(lista);

        var result = await Service().GetAllAsync();

        result.Should().HaveCount(2);
        await _repo.Received(1).GetAllAsync();
    }

    [Fact]
    public async Task GetById_DeveRetornarTema_QuandoExiste()
    {
        var tema = new Tema { Id = 3, Descricao = "Arte" };
        _repo.GetByIdAsync(3L).Returns(tema);

        var result = await Service().GetByIdAsync(3L);

        result.Should().NotBeNull();
        result!.Id.Should().Be(3L);
    }
}