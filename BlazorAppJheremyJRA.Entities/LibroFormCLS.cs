using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlazorAppJheremyJRA.Entities
{
    public class LibroFormCLS
    {
        public int IdLibro { get; set; }

        [Required(ErrorMessage = "El código es obligatorio")]
        public string Codigo { get; set; } = "";

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = "";

        [Required(ErrorMessage = "El resumen es obligatorio")]
        public string Resumen { get; set; } = "";

        public string Autor { get; set; } = "";
        public string Editorial { get; set; } = "";

        public int Anio { get; set; }

        [Range(1, 5000,
ErrorMessage = "Debe ingresar un número de páginas mayor a 0 y menor o igual a 5000")]
        public int NumeroPaginas { get; set; }


        [Range(1, int.MaxValue,
        ErrorMessage = "Debe ingresar un stock mayor a 0")]
        public int Stock { get; set; }

        public string Categoria { get; set; } = "";
        public List<int> IdCategorias { get; set; } = new();

        [Range(1, int.MaxValue,
            ErrorMessage = "Seleccione tipo de libro")]
        public int idTipoLibro { get; set; }

        [Range(1, int.MaxValue,
ErrorMessage = "Debe seleccionar un autor")]
        public int iidautor { get; set; }

        // PDF 8-9 Imagen
        [JsonIgnore]
        public string Imagen { get; set; } = "img/usuario.png";

        public byte[]? imagen { get; set; }

        // PDF 9 archivo PDF
        public byte[]? archivo { get; set; }

        public string nombrearchivo { get; set; } = "";
    }
}