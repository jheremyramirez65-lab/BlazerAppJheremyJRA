using BlazorAppJheremyJRA.Entities;
using System.Net.Http.Json;

namespace BlazorAppJheremyJRA.Services
{
    public class AutorService
    {
        private readonly HttpClient http;

        public AutorService(HttpClient http)
        {
            this.http = http;
        }

        // =========================================================
        // LISTAR AUTORES COMPLETOS
        // Se usa en AutorPage y AutorFormPage
        // =========================================================
        public async Task<List<AutorListCLS>> listarAutores()
        {
            try
            {
                var lista = await http.GetFromJsonAsync<List<AutorListCLS>>(
                    "api/Autor"
                );

                return lista ?? new List<AutorListCLS>();
            }
            catch
            {
                return new List<AutorListCLS>();
            }
        }

        // =========================================================
        // LISTAR AUTORES PARA COMBO
        // Se usa en FormLibro
        // =========================================================
        public async Task<List<AutorCLS>> listarAutoresCombo()
        {
            try
            {
                var lista = await http.GetFromJsonAsync<List<AutorCLS>>(
                    "api/Autor/combo"
                );

                return lista ?? new List<AutorCLS>();
            }
            catch
            {
                return new List<AutorCLS>();
            }
        }

        // =========================================================
        // RECUPERAR AUTOR POR ID
        // =========================================================
        public async Task<AutorFormCLS?> recuperarAutorPorId(int id)
        {
            try
            {
                return await http.GetFromJsonAsync<AutorFormCLS>(
                    $"api/Autor/{id}"
                );
            }
            catch
            {
                return null;
            }
        }

        // =========================================================
        // GUARDAR AUTOR
        // =========================================================
        public async Task guardarAutor(AutorFormCLS autor)
        {
            await http.PostAsJsonAsync(
                "api/Autor",
                autor
            );
        }

        // =========================================================
        // ACTUALIZAR AUTOR
        // =========================================================
        public async Task actualizarAutor(AutorFormCLS autor)
        {
            await http.PutAsJsonAsync(
                $"api/Autor/{autor.IdAutor}",
                autor
            );
        }

        // =========================================================
        // ELIMINAR AUTOR
        // =========================================================
        public async Task eliminarAutor(int id)
        {
            await http.DeleteAsync(
                $"api/Autor/{id}"
            );
        }

        // =========================================================
        // LISTAR SEXO
        // =========================================================
        public async Task<List<ComboCLS>> listarSexo()
        {
            try
            {
                var lista = await http.GetFromJsonAsync<List<ComboCLS>>(
                    "api/Autor/listarSexo"
                );

                return lista ?? new List<ComboCLS>();
            }
            catch
            {
                return new List<ComboCLS>();
            }
        }

        // =========================================================
        // LISTAR PAIS
        // =========================================================
        public async Task<List<ComboCLS>> listarPais()
        {
            try
            {
                var lista = await http.GetFromJsonAsync<List<ComboCLS>>(
                    "api/Autor/listarPais"
                );

                return lista ?? new List<ComboCLS>();
            }
            catch
            {
                return new List<ComboCLS>();
            }
        }
    }
}