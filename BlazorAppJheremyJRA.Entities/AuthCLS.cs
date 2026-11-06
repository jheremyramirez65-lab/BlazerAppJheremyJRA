using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BlazorAppJheremyJRA.Entities
{
    public static class Reglas
    {
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,100}$";
        public const string PasswordOpcional = "^$|" + Password;
        public const string MensajePassword = "Mínimo 8 caracteres, con mayúscula, minúscula y número.";
        public const string NombreUsuario = @"^[a-zA-Z0-9._-]{4,30}$";
        public const string MensajeNombreUsuario = "De 4 a 30 caracteres: letras, números, punto, guion o guion bajo.";
    }

    public class LoginRequestCLS
    {
        // Acepta correo o nombre de usuario
        [Required(ErrorMessage = "Ingresa tu correo o nombre de usuario.")]
        [StringLength(150, ErrorMessage = "El dato ingresado es demasiado largo.")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "La contraseña es demasiado larga.")]
        public string Password { get; set; } = "";
    }

    public class RegistroRequestCLS
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres.")]
        public string Apellido { get; set; } = "";

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [RegularExpression(Reglas.NombreUsuario, ErrorMessage = Reglas.MensajeNombreUsuario)]
        public string NombreUsuario { get; set; } = "";

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [StringLength(150, ErrorMessage = "El correo es demasiado largo.")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(Reglas.Password, ErrorMessage = Reglas.MensajePassword)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = "";
    }

    public class AuthResponseCLS
    {
        public string Token { get; set; } = "";
        public DateTime Expira { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Rol { get; set; } = "";
    }

    public class OlvidePasswordCLS
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string Correo { get; set; } = "";
    }

    public class RestablecerPasswordCLS
    {
        [Required] public string Token { get; set; } = "";

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [RegularExpression(Reglas.Password, ErrorMessage = Reglas.MensajePassword)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirma tu contraseña.")]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = "";
    }
}