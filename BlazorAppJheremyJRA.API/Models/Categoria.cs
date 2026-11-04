namespace BlazorAppJheremyJRA.API.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Libro> Libros { get; set; }
            = new List<Libro>();
    }
}
