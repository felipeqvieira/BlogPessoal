# Blog Pessoal
---
Uma API RESTful robusta desenvolvida em .NET 9 para gerenciamento de um blog pessoal, contando com autenticação segura, persistência relacional e integração com Inteligência Artificial para enriquecimento automático de conteúdo.
## Arquitetura e Tecnologias
---
O projeto adota uma arquitetura em camadas (Controllers, Services, Repositories, DTOs e Models) focada em separação de responsabilidades e injeção de dependências.

- **Framework Base:** .NET 9.0 (C# 13)
    
- **Acesso a Dados:** Entity Framework Core com provedor MySQL (Pomelo).
    
- **Segurança e Identidade:** ASP.NET Core Identity integrado com autenticação baseada em JWT (JSON Web Tokens).
    
- **Integração de IA:** Integração assíncrona com a API do Groq (`llama-3.1-8b-instant`) para sumarização e categorização de postagens.
    
- **Documentação da API:** Swagger/OpenAPI configurado para suportar cabeçalhos Bearer.
    
- **Testes e Qualidade:** Cobertura de testes unitários com xUnit, NSubstitute e FluentAssertions. Análise de qualidade de código contínua configurada via SonarQube e Coverlet.
## Estrutura de Diretórios
---
- `Controllers/`: Exposição dos endpoints HTTP e documentação de rotas.
    
- `Services/`: Camada de regras de negócio, orquestração e contratos (`Interfaces`).
    
- `Services/IA/`: Isolamento da infraestrutura de comunicação externa com provedores de LLM.
    
- `Repositories/`: Encapsulamento de consultas LINQ e operações no Entity Framework.
    
- `DTOs/`: Objetos de Transferência de Dados para validação de entrada e saída segura.
    
- `Models/`: Entidades de domínio mapeadas para o banco de dados relacional.
    
- `Config/`: Configurações de infraestrutura cruzada (ex: Criptografia JWT).
    
- `Middlewares/`: Interceptadores globais de requisição (ex: Tratamento de Exceções padronizado).

## Pré-requisitos
---
Para clonar e executar este projeto localmente, o ambiente de desenvolvimento requer:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
    
- Servidor MySQL (versão 8.0+).
    
- Chave de API válida do [Groq Console](https://console.groq.com/).
    
## Configuração do Ambiente
---
1. Clone o repositório para a sua máquina local:

```bash
git clone https://github.com/seu-usuario/blog-pessoal-dotnet.git
cd blog-pessoal-dotnet
```

2. Configure as credenciais. O arquivo `appsettings.json` utiliza placeholders de segurança. É recomendado o uso da ferramenta `dotnet user-secrets` ou variáveis de ambiente para preencher as seguintes chaves antes da execução:
    
    - `ConnectionStrings:DefaultConnection` -> String de conexão do MySQL.
        
    - `Jwt:SecretKey` -> Chave criptográfica (mínimo de 256 bits).
        
    - `Groq:ApiKey` -> Sua chave gerada no painel do Groq.
        
3. Aplique as migrações para gerar o esquema relacional:

```bash
dotnet ef database update
```
## Executando a Aplicação
---
Para iniciar o servidor web Kestrel acoplado ao .NET:

```bash
dotnet run
```
## Executando os Testes e Análise Estática
---
O projeto possui cobertura de testes unitários sob o padrão AAA (Arrange, Act, Assert).

Para rodar os testes e gerar o relatório de cobertura de código (Coverlet):

```bash
dotnet test BlogPessoal.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="TestResults/"
```

Caso possua um servidor local do SonarQube rodando na porta 9000, o comportamento esperado para submeter os relatórios é a execução sequencial do scanner:

```
dotnet sonarscanner begin /k:"blog-pessoal-dotnet" /d:sonar.token="SEU_TOKEN" /d:sonar.host.url="http://localhost:9000" /d:sonar.cs.opencover.reportsPaths="BlogPessoal.Tests/TestResults/coverage.opencover.xml"

dotnet test BlogPessoal.Tests /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="TestResults/"

dotnet sonarscanner end /d:sonar.token="SEU_TOKEN"
```
