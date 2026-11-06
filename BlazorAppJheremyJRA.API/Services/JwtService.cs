using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BlazorAppJheremyJRA.API.Services
{
    public class JwtService
    {
        private readonly IConfiguration config;

        public JwtService(IConfiguration config)
        {
            this.config = config;
        }

        public (string Token, DateTime Expira) Generar(Usuario usuario, string rol)
        {
            var clave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credenciales = new SigningCredentials(clave, SecurityAlgorithms.HmacSha256);

            var minutos = int.TryParse(config["Jwt:Minutos"], out var m) ? m : 120;
            var expira = DateTime.UtcNow.AddMinutes(minutos);

            var claims = new List<Claim>
            {
                new("sub", usuario.Iidusuario.ToString()),
                new("email", usuario.Correo ?? ""),
                new("name", $"{usuario.Nombre} {usuario.Apellido}".Trim()),
                new("username", usuario.NombreUsuario ?? ""),
                new("role", rol)
            };

            foreach (var permiso in Permisos.De(rol))
            {
                claims.Add(new Claim("permiso", permiso));
            }

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expira,
                signingCredentials: credenciales);

            return (new JwtSecurityTokenHandler().WriteToken(token), expira);
        }
    }
}