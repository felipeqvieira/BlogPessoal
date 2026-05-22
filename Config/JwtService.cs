using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// Serviço de infraestrutura encarregado da criação, configuração e assinatura criptográfica de JWT.
/// </summary>
/// <param name="config">Interface de configuração para acesso seguro ao appsettings.json.</param>
/// <param name="userManager">Gestor do Identity utilizado para extrair roles do usuário na emissão do token.</param>
public class JwtService(IConfiguration config, UserManager<Usuario> userManager)
{
    // Leitura das variáveis de ambiente / appsettings
    private readonly string _secret =
        config["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada.");
    private readonly string _issuer   = config["Jwt:Issuer"]!;
    private readonly string _audience = config["Jwt:Audience"]!;
    private readonly int _expiryHours =
        int.Parse(config["Jwt:ExpiryHours"] ?? "8");

    /// <summary>
    /// Gera um JWT assinado digitalmente contendo as claims de identidade e acesso do utilizador.
    /// </summary>
    /// <param name="usuario">Entidade do usuário previamente autenticada no sistema.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona, contendo a string base64 do Token JWT.</returns>
    public async Task<string> GenerateTokenAsync(Usuario usuario)
    {
        // Cria a chave simétrica e define o algoritmo de assinatura para proteger o token contra falsificação
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Busca os papéis do utilizador de forma assíncrona
        var roles = await userManager.GetRolesAsync(usuario);

        // Monta o "Payload" do token
        var claims = new List<Claim>
        {
            // O NameIdentifier é p/ extrair ID numérico posteriormente no UsuarioController
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email,          usuario.Email!),
            new Claim(ClaimTypes.Name,           usuario.Nome),
            // O JTI (JWT ID) fornece um identificador único para o próprio token
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Injeta cada papel de acesso como um Claim separado, permitindo verificações nos controladores
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        // Estrutura as propriedades do token
        var token = new JwtSecurityToken(
            issuer:             _issuer,
            audience:           _audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(_expiryHours),
            signingCredentials: creds
        );

        // Serializa o objeto JwtSecurityToken para o formato de string compacta utilizada em cabeçalhos HTTP
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}