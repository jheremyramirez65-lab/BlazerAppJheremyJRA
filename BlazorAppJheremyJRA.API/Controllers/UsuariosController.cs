using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.API.Services;
using BlazorAppJheremyJRA.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorAppJheremyJRA.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.AdministrarUsuarios)]
    public class UsuariosController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;

        public UsuariosController(BdbibliotecaContext bd)
        {
            this.bd = bd;
        }

        private int IdActual =>
            int.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : 0;

        [HttpGet("roles")]
        public async Task<ActionResult<List<RolCLS>>> ListarRoles()
        {
            return await bd.TipoUsuarios
                .AsNoTracking()
                .Where(t => t.Bhabilitado == 1)
                .OrderBy(t => t.Iidtipousuario)
                .Select(t => new RolCLS { Id = t.Iidtipousuario, Nombre = t.Nombre })
                .ToListAsync();
        }

        // GET api/Usuarios?buscar=ana&idRol=3&estado=activo
        [HttpGet]
        public async Task<ActionResult<List<UsuarioAdminCLS>>> Listar(
            [FromQuery] string? buscar, [FromQuery] int? idRol, [FromQuery] string? estado)
        {
            var consulta = bd.Usuarios.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var t = buscar.Trim();
                consulta = consulta.Where(u => u.Nombre.Contains(t) || u.Apellido.Contains(t) ||
                                               u.Correo.Contains(t) || u.NombreUsuario.Contains(t));
            }

            if (idRol.HasValue)
            {
                consulta = consulta.Where(u => u.Iidtipousuario == idRol.Value);
            }

            if (estado == "activo") consulta = consulta.Where(u => u.Bhabilitado == 1);
            if (estado == "inactivo") consulta = consulta.Where(u => u.Bhabilitado == 0);

            return await consulta
                .OrderBy(u => u.Nombre).ThenBy(u => u.Apellido)
                .Select(u => new UsuarioAdminCLS
                {
                    Id = u.Iidusuario,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    NombreUsuario = u.NombreUsuario,
                    Correo = u.Correo,
                    IdRol = u.Iidtipousuario ?? 0,
                    Rol = u.IidtipousuarioNavigation!.Nombre,
                    Habilitado = u.Bhabilitado == 1,
                    FechaCreacion = u.FechaCreacion
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] UsuarioFormCLS form)
        {
            if (string.IsNullOrWhiteSpace(form.Password))
            {
                return BadRequest(new { mensaje = "La contraseña es obligatoria para un usuario nuevo." });
            }

            var error = await ValidarAsync(form, 0);
            if (error != null) return Conflict(new { mensaje = error });

            var usuario = new Usuario
            {
                Nombre = form.Nombre.Trim(),
                Apellido = form.Apellido.Trim(),
                NombreUsuario = form.NombreUsuario.Trim().ToLowerInvariant(),
                Correo = form.Correo.Trim().ToLowerInvariant(),
                Password = PasswordService.Hashear(form.Password),
                Iidtipousuario = form.IdRol,
                Bhabilitado = 1,
                FechaCreacion = DateTime.Now
            };

            bd.Usuarios.Add(usuario);
            await bd.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario creado correctamente." });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] UsuarioFormCLS form)
        {
            var usuario = await bd.Usuarios
                .Include(u => u.IidtipousuarioNavigation)
                .FirstOrDefaultAsync(u => u.Iidusuario == id);

            if (usuario == null) return NotFound(new { mensaje = "El usuario no existe." });

            var error = await ValidarAsync(form, id);
            if (error != null) return Conflict(new { mensaje = error });

            bool cambiaRol = usuario.Iidtipousuario != form.IdRol;

            if (cambiaRol && id == IdActual)
            {
                return BadRequest(new { mensaje = "No puedes cambiar tu propio rol." });
            }

            if (cambiaRol && usuario.IidtipousuarioNavigation?.Nombre == Roles.Administrador
                && await QuedariaSinAdminAsync(id))
            {
                return BadRequest(new { mensaje = "No puedes quitar el rol al último administrador." });
            }

            usuario.Nombre = form.Nombre.Trim();
            usuario.Apellido = form.Apellido.Trim();
            usuario.NombreUsuario = form.NombreUsuario.Trim().ToLowerInvariant();
            usuario.Correo = form.Correo.Trim().ToLowerInvariant();
            usuario.Iidtipousuario = form.IdRol;

            if (!string.IsNullOrWhiteSpace(form.Password))
            {
                usuario.Password = PasswordService.Hashear(form.Password);
            }

            await bd.SaveChangesAsync();
            return Ok(new { mensaje = "Usuario actualizado correctamente." });
        }

        // "Eliminar" = desactivar (no se borra el registro)
        [HttpPut("{id:int}/estado")]
        public async Task<IActionResult> CambiarEstado(int id, [FromQuery] bool habilitado)
        {
            var usuario = await bd.Usuarios
                .Include(u => u.IidtipousuarioNavigation)
                .FirstOrDefaultAsync(u => u.Iidusuario == id);

            if (usuario == null) return NotFound(new { mensaje = "El usuario no existe." });

            if (!habilitado && id == IdActual)
            {
                return BadRequest(new { mensaje = "No puedes desactivar tu propia cuenta." });
            }

            if (!habilitado && usuario.IidtipousuarioNavigation?.Nombre == Roles.Administrador
                && await QuedariaSinAdminAsync(id))
            {
                return BadRequest(new { mensaje = "No puedes desactivar al último administrador." });
            }

            usuario.Bhabilitado = habilitado ? 1 : 0;
            await bd.SaveChangesAsync();

            return Ok(new { mensaje = habilitado ? "Usuario activado." : "Usuario desactivado." });
        }

        private async Task<string?> ValidarAsync(UsuarioFormCLS f, int idExcluir)
        {
            var correo = f.Correo.Trim().ToLowerInvariant();
            var nombreUsuario = f.NombreUsuario.Trim().ToLowerInvariant();

            if (await bd.Usuarios.AnyAsync(u => u.Correo == correo && u.Iidusuario != idExcluir))
                return "Ya existe otro usuario con ese correo.";

            if (await bd.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario && u.Iidusuario != idExcluir))
                return "Ya existe otro usuario con ese nombre de usuario.";

            if (!await bd.TipoUsuarios.AnyAsync(t => t.Iidtipousuario == f.IdRol && t.Bhabilitado == 1))
                return "El rol seleccionado no existe.";

            return null;
        }

        // ¿Quedaría el sistema sin ningún administrador activo?
        private async Task<bool> QuedariaSinAdminAsync(int idUsuario)
        {
            return !await bd.Usuarios.AnyAsync(u =>
                u.Iidusuario != idUsuario &&
                u.Bhabilitado == 1 &&
                u.IidtipousuarioNavigation!.Nombre == Roles.Administrador);
        }
    }
}