using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EducAR.API.Models;
using Microsoft.IdentityModel.Tokens;

namespace EducAR.API.Helpers;

public class JwtHelper
{
    private readonly IConfiguration _config;

    public JwtHelper(IConfiguration config)
    {
        _config = config;
    }

    public string GenerarToken(Usuario usuario)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Name,            usuario.NombreUsuario),
            new Claim(ClaimTypes.Role,            usuario.Rol.Nombre),
            new Claim("IdEscuela",                usuario.IdEscuela.ToString()),
            new Claim("NombreCompleto",           $"{usuario.Nombre} {usuario.Apellido}")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var exp = DateTime.UtcNow.AddHours(int.Parse(_config["Jwt:ExpirationHours"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: exp,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}