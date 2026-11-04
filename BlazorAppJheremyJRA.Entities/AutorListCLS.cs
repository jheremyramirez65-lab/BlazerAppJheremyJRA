using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace BlazorAppJheremyJRA.Entities
{
    public class AutorListCLS
    {
        public int idAutor { get; set; }

        public string nombre { get; set; } = "";

        public string apPaterno { get; set; } = "";

        public string apMaterno { get; set; } = "";

        public int iidSexo { get; set; }

        public string nombreSexo { get; set; } = "";

        public int iidPais { get; set; }

        public string nombrePais { get; set; } = "";

        public string descripcion { get; set; } = "";

        public string imagen { get; set; } = "img/usuario.png";
    }
}