using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppJheremyJRA.Entities
{
    public static class Roles
    {
        public const string Administrador = "Administrador";
        public const string Bibliotecario = "Bibliotecario";
        public const string Usuario = "Usuario";
    }

    public static class Permisos
    {
        public const string Ver = "ver";
        public const string Crear = "crear";
        public const string Editar = "editar";
        public const string Eliminar = "eliminar";
        public const string AdministrarUsuarios = "usuarios";

        private static readonly Dictionary<string, string[]> PorRol =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [Roles.Administrador] = new[] { Ver, Crear, Editar, Eliminar, AdministrarUsuarios },
                [Roles.Bibliotecario] = new[] { Ver, Crear, Editar },
                [Roles.Usuario] = new[] { Ver }
            };

        public static string[] De(string rol) =>
            PorRol.TryGetValue(rol, out var permisos) ? permisos : new[] { Ver };
    }

    public static class Politicas
    {
        public const string Ver = "PuedeVer";
        public const string Crear = "PuedeCrear";
        public const string Editar = "PuedeEditar";
        public const string Eliminar = "PuedeEliminar";
        public const string AdministrarUsuarios = "PuedeAdministrarUsuarios";
    }

    public class RolCLS
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
    }

    public class UsuarioAdminCLS
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string NombreUsuario { get; set; } = "";
        public string Correo { get; set; } = "";
        public int IdRol { get; set; }
        public string Rol { get; set; } = "";
        public bool Habilitado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class UsuarioFormCLS
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Entre 2 y 100 caracteres.")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [RegularExpression(Reglas.NombreUsuario, ErrorMessage = Reglas.MensajeNombreUsuario)]
        public string NombreUsuario { get; set; } = "";

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [StringLength(150)]
        public string Correo { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un rol.")]
        public int IdRol { get; set; }

        [RegularExpression(Reglas.PasswordOpcional, ErrorMessage = Reglas.MensajePassword)]
        public string? Password { get; set; }
    }
}