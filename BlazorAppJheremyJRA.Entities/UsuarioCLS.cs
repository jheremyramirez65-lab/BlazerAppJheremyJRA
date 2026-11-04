using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorAppJheremyJRA.Entities
{
    public class UsuarioCLS
    {
        public int id { get; set; }

        public string nombre { get; set; } = "";

        public string apellido { get; set; } = "";

        public string correo { get; set; } = "";

        public string contrasena { get; set; } = "";

        public string rol { get; set; } = "";

        public bool estado { get; set; }

        public DateTime fechaCreacion { get; set; }
    }
}
