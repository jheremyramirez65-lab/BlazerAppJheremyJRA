using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppJheremyJRA.Entities
{
    public class AutorFormCLS
    {
        public int IdAutor { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(3, ErrorMessage = "El nombre debe tener mínimo 3 letras.")]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "El nombre solo debe contener letras.")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [MinLength(3, ErrorMessage = "El apellido paterno debe tener mínimo 3 letras.")]
        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "El apellido paterno solo debe contener letras.")]
        public string ApPaterno { get; set; } = "";

        [RegularExpression(@"^[\p{L} ]+$", ErrorMessage = "El apellido materno solo debe contener letras.")]
        public string ApMaterno { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el sexo.")]
        public int IidSexo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Seleccione el país.")]
        public int IidPais { get; set; }

        [MaxLength(500, ErrorMessage = "La descripción admite máximo 500 caracteres.")]
        public string Descripcion { get; set; } = "";

        public byte[]? Imagen { get; set; }
    }
}
