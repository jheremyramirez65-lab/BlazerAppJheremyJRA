using System.Security.Cryptography;
using System.Text;
using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.API.Services;
using BlazorAppJheremyJRA.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;
        private readonly JwtService jwt;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment entorno;
        private readonly ILogger<AuthController> log;

        public AuthController(BdbibliotecaContext bd, JwtService jwt, IConfiguration config,
            IWebHostEnvironment entorno, ILogger<AuthController> log)
        {
            this.bd = bd;
            this.jwt = jwt;
            this.config = config;
            this.entorno = entorno;
            this.log = log;
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequestCLS req)
        {
            var dato = req.Correo.Trim().ToLowerInvariant();

            var usuario = await bd.Usuarios
                .Include(u => u.IidtipousuarioNavigation)
                .FirstOrDefaultAsync(u => u.Correo == dato || u.NombreUsuario == dato);

            // Siempre se calcula un hash, para no revelar qué cuentas existen por el tiempo de respuesta
            var passwordOk = PasswordService.Verificar(
                req.Password, usuario?.Password ?? PasswordService.HashFalso);

            if (usuario == null || !passwordOk)
            {
                return Unauthorized(new { mensaje = "Correo/usuario o contraseña incorrectos." });
            }

            if (usuario.Bhabilitado == 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { mensaje = "Tu cuenta está desactivada. Contacta al administrador." });
            }

            // Migra contraseñas antiguas en texto plano a hash
            if (!PasswordService.EstaHasheada(usuario.Password))
            {
                usuario.Password = PasswordService.Hashear(req.Password);
                await bd.SaveChangesAsync();
            }

            return Ok(CrearRespuesta(usuario, usuario.IidtipousuarioNavigation?.Nombre ?? Roles.Usuario));
        }

        // ---------------- REGISTRO ----------------
        [HttpPost("registro")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequestCLS req)
        {
            var correo = req.Correo.Trim().ToLowerInvariant();
            var nombreUsuario = req.NombreUsuario.Trim().ToLowerInvariant();

            if (await bd.Usuarios.AnyAsync(u => u.Correo == correo))
            {
                return Conflict(new { mensaje = "Ya existe una cuenta con ese correo." });
            }

            if (await bd.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario))
            {
                return Conflict(new { mensaje = "Ese nombre de usuario ya está en uso." });
            }

            // El rol NUNCA lo elige quien se registra: siempre empieza como "Usuario"
            var tipo = await bd.TipoUsuarios.FirstOrDefaultAsync(t => t.Nombre == Roles.Usuario);

            if (tipo == null)
            {
                return StatusCode(500, new { mensaje = "No existe el rol 'Usuario' en la base de datos." });
            }

            var usuario = new Usuario
            {
                Nombre = req.Nombre.Trim(),
                Apellido = req.Apellido.Trim(),
                NombreUsuario = nombreUsuario,
                Correo = correo,
                Password = PasswordService.Hashear(req.Password),
                Iidtipousuario = tipo.Iidtipousuario,
                Bhabilitado = 1,
                FechaCreacion = DateTime.Now
            };

            try
            {
                bd.Usuarios.Add(usuario);
                await bd.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Dos registros simultáneos con el mismo correo/usuario
                return Conflict(new { mensaje = "El correo o el nombre de usuario ya están registrados." });
            }

            return Ok(CrearRespuesta(usuario, Roles.Usuario));
        }

        // ---------------- RECUPERAR CONTRASEÑA ----------------
        [HttpPost("olvide-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> OlvidePassword([FromBody] OlvidePasswordCLS req)
        {
            const string respuestaGenerica =
                "Si el correo está registrado, se generó un enlace para restablecer la contraseña.";

            var correo = req.Correo.Trim().ToLowerInvariant();
            var usuario = await bd.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo && u.Bhabilitado == 1);

            // Misma respuesta exista o no el correo (no se revela qué cuentas existen)
            if (usuario == null)
            {
                return Ok(new { mensaje = respuestaGenerica, enlaceDesarrollo = (string?)null });
            }

            // Anula los enlaces anteriores sin usar
            var anteriores = await bd.RecuperacionPasswords
                .Where(r => r.Iidusuario == usuario.Iidusuario && !r.Usado)
                .ToListAsync();
            foreach (var anterior in anteriores) anterior.Usado = true;

            var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));

            bd.RecuperacionPasswords.Add(new RecuperacionPassword
            {
                Iidusuario = usuario.Iidusuario,
                TokenHash = HashToken(token),
                Expira = DateTime.Now.AddMinutes(30),
                Usado = false,
                Fecha = DateTime.Now
            });
            await bd.SaveChangesAsync();

            var urlFront = (config["Front:Url"] ?? "http://localhost:5094").TrimEnd('/');
            var enlace = $"{urlFront}/restablecer?token={token}";

            // Aquí se enviaría por correo (SMTP). En este proyecto queda en el log de la API.
            log.LogInformation("Enlace de recuperación para {Correo}: {Enlace}", correo, enlace);

            return Ok(new
            {
                mensaje = respuestaGenerica,
                enlaceDesarrollo = entorno.IsDevelopment() ? enlace : null
            });
        }

        [HttpPost("restablecer-password")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> RestablecerPassword([FromBody] RestablecerPasswordCLS req)
        {
            var hash = HashToken(req.Token);
            var ahora = DateTime.Now;

            var registro = await bd.RecuperacionPasswords
                .FirstOrDefaultAsync(r => r.TokenHash == hash && !r.Usado && r.Expira > ahora);

            if (registro == null)
            {
                return BadRequest(new { mensaje = "El enlace no es válido o ya expiró. Solicita uno nuevo." });
            }

            var usuario = await bd.Usuarios.FirstOrDefaultAsync(u => u.Iidusuario == registro.Iidusuario);

            if (usuario == null || usuario.Bhabilitado == 0)
            {
                return BadRequest(new { mensaje = "No se pudo restablecer la contraseña." });
            }

            usuario.Password = PasswordService.Hashear(req.Password);
            registro.Usado = true;
            await bd.SaveChangesAsync();

            return Ok(new { mensaje = "Contraseña actualizada. Ya puedes iniciar sesión." });
        }

        // ---------------- PERFIL ----------------
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                nombre = User.FindFirst("name")?.Value,
                correo = User.FindFirst("email")?.Value,
                rol = User.FindFirst("role")?.Value
            });
        }

        // ---------------- Auxiliares ----------------
        private AuthResponseCLS CrearRespuesta(Usuario usuario, string rol)
        {
            var (token, expira) = jwt.Generar(usuario, rol);

            return new AuthResponseCLS
            {
                Token = token,
                Expira = expira,
                Nombre = $"{usuario.Nombre} {usuario.Apellido}".Trim(),
                Correo = usuario.Correo ?? "",
                Rol = rol
            };
        }

        private static string HashToken(string token) =>
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token))).ToLowerInvariant();
    }
}