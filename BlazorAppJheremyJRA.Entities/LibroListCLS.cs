using System.Text.Json.Serialization;

namespace BlazorAppJheremyJRA.Entities
{
    public class LibroListCLS
    {

        public int idLibro { get; set; }

        public string codigo { get; set; } = "";

        public string titulo { get; set; } = "";

        public string resumen { get; set; } = "";

        public string autor { get; set; } = "";

        public string editorial { get; set; } = "";

        public int anio { get; set; }

        public int numeroPaginas { get; set; }

        public int stock { get; set; } = 0;

        public string categoria { get; set; } = "";

        public int idTipoLibro { get; set; }

        public string nombretipolibro { get; set; } = "";


        // Imagen PDF 8
        public string Imagen { get; set; } = "img/usuario.png";

        [JsonIgnore]
        public byte[]? imagen { get; set; }


        // Archivo PDF 9
        public byte[]? archivo { get; set; }

        public string? nombrearchivo { get; set; } = "";

    }
}