using BlazorAppJheremyJRA.Entities;
using System.Net.Http.Json;

namespace BlazorAppJheremyJRA.Services
{
    public class TipoLibroService
    {
        private readonly HttpClient http;

        public TipoLibroService(HttpClient _http)
        {
            http = _http;
        }

        public async Task<List<TipoLibroCLS>> listarTipoLibros()
        {
            try
            {
                var response =
                    await http.GetFromJsonAsync<List<TipoLibroCLS>>(
                        "api/TipoLibro"
                    );

                if (response == null)
                {
                    return new List<TipoLibroCLS>();
                }

                return response;
            }
            catch
            {
                return new List<TipoLibroCLS>();
            }
        }
    }
}