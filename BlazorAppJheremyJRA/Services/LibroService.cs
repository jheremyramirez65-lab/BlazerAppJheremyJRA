using BlazorAppJheremyJRA.Entities;
using System.Net.Http.Json;

namespace BlazorAppJheremyJRA.Services
{
    public class LibroService
    {
        private readonly HttpClient http;

        public LibroService(HttpClient http)
        {
            this.http = http;
        }

        // LISTAR LIBROS
        public async Task<List<LibroListCLS>> listarLibros()
        {
            try
            {
                var response =
                    await http.GetFromJsonAsync<List<LibroListCLS>>(
                        "api/Libro"
                    );

                if (response == null)
                {
                    return new List<LibroListCLS>();
                }

                return response;
            }
            catch
            {
                return new List<LibroListCLS>();
            }
        }

        // FILTRAR LIBROS
        public async Task<List<LibroListCLS>> filtrarLibros(string nombretitulo)
        {
            List<LibroListCLS> lista =
                await listarLibros();

            if (string.IsNullOrWhiteSpace(nombretitulo))
            {
                return lista;
            }

            List<LibroListCLS> listaFiltrada =
                lista
                .Where(p =>
                    p.titulo.ToUpper().Contains(
                        nombretitulo.ToUpper()
                    )
                )
                .ToList();

            return listaFiltrada;
        }

        // BUSCAR LIBRO POR ID
        public async Task<LibroFormCLS?> recuperarLibroPorId(int id)
        {
            try
            {
                return await http.GetFromJsonAsync<LibroFormCLS>(
                    $"api/Libro/{id}"
                );
            }
            catch
            {
                return null;
            }
        }

        // ELIMINAR LIBRO
        public async Task<string> eliminarLibro(int idlibro)
        {
            var response = await http.DeleteAsync(
                "api/Libro/" + idlibro
            );

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Error: " +
                       await response.Content.ReadAsStringAsync();
            }
        }

        // GUARDAR LIBRO
        public async Task<string> guardarLibro(LibroFormCLS libro)
        {
            try
            {
                var response =
                    await http.PostAsJsonAsync(
                        "api/Libro",
                        libro
                    );

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return "Error: " +
                    await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // RECUPERAR ARCHIVO PDF
        public async Task<string> recuperarArchivoPorId(int id)
        {
            try
            {
                var archivo = await http.GetByteArrayAsync(
                    $"api/Libro/recuperarArchivo/{id}"
                );

                return Convert.ToBase64String(archivo);
            }
            catch
            {
                return "";
            }
        }

        // ACTUALIZAR LIBRO
        public async Task<string> actualizarLibro(LibroFormCLS libro)
        {
            try
            {
                var response = await http.PutAsJsonAsync(
                    "api/Libro/" + libro.IdLibro,
                    libro
                );


                if (response.IsSuccessStatusCode)
                {
                    return "Actualizado correctamente";
                }
                else
                {
                    return "Error: " + await response.Content.ReadAsStringAsync();
                }

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}