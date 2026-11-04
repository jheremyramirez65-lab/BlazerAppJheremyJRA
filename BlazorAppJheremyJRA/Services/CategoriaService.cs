using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorAppJheremyJRA.Services
{
    public class CategoriaService
    {
        private readonly HttpClient http;

        private const string UrlApi = "http://localhost:5273/api/Categoria";

        public CategoriaService(HttpClient http)
        {
            this.http = http;
        }

        // GET: Listar categorías
        public async Task<List<CategoriaCLS>> Listar()
        {
            return await http.GetFromJsonAsync<List<CategoriaCLS>>(
                UrlApi
            ) ?? new List<CategoriaCLS>();
        }

        // POST: Registrar una categoría
        public async Task<HttpResponseMessage> Registrar(
            CategoriaCLS categoria)
        {
            return await http.PostAsJsonAsync(
                UrlApi,
                categoria
            );
        }

        // POST: Asignar un libro a una categoría
        public async Task<HttpResponseMessage> AsignarLibro(
            int idCategoria,
            int idLibro)
        {
            return await http.PostAsync(
                $"{UrlApi}/{idCategoria}/libros/{idLibro}",
                null
            );
        }

        // GET: Consultar los libros de una categoría
        public async Task<CategoriaLibrosCLS?> ListarLibros(
            int idCategoria)
        {
            return await http.GetFromJsonAsync<CategoriaLibrosCLS>(
                $"{UrlApi}/{idCategoria}/libros"
            );
        }
    }

    // Modelo de categoría
    public class CategoriaCLS
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = "";
    }

    // Modelo de categoría con sus libros
    public class CategoriaLibrosCLS
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = "";

        public List<LibroCategoriaCLS> Libros { get; set; } = new();
    }

    // Modelo de libro
    public class LibroCategoriaCLS
    {
        public int IdLibro { get; set; }

        public string Titulo { get; set; } = "";
    }
}
