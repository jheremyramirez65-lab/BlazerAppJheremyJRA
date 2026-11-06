using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorAppJheremyJRA.Entities;

namespace BlazorAppJheremyJRA.Services
{
    public class UsuarioAdminService
    {
        private readonly HttpClient http;

        public UsuarioAdminService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<UsuarioAdminCLS>> ListarAsync(string? buscar, int? idRol, string? estado)
        {
            var filtros = new List<string>();
            if (!string.IsNullOrWhiteSpace(buscar)) filtros.Add($"buscar={Uri.EscapeDataString(buscar.Trim())}");
            if (idRol.HasValue) filtros.Add($"idRol={idRol.Value}");
            if (!string.IsNullOrEmpty(estado)) filtros.Add($"estado={estado}");

            var url = "api/Usuarios" + (filtros.Count > 0 ? "?" + string.Join("&", filtros) : "");
            return await http.GetFromJsonAsync<List<UsuarioAdminCLS>>(url) ?? new();
        }

        public async Task<List<RolCLS>> ListarRolesAsync()
        {
            return await http.GetFromJsonAsync<List<RolCLS>>("api/Usuarios/roles") ?? new();
        }

        public async Task<(bool Ok, string Mensaje)> GuardarAsync(UsuarioFormCLS form)
        {
            try
            {
                var r = form.Id == 0
                    ? await http.PostAsJsonAsync("api/Usuarios", form)
                    : await http.PutAsJsonAsync($"api/Usuarios/{form.Id}", form);

                return await LeerAsync(r);
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
        }

        public async Task<(bool Ok, string Mensaje)> CambiarEstadoAsync(int id, bool habilitado)
        {
            try
            {
                var r = await http.PutAsync($"api/Usuarios/{id}/estado?habilitado={habilitado.ToString().ToLower()}", null);
                return await LeerAsync(r);
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
        }

        private static async Task<(bool Ok, string Mensaje)> LeerAsync(HttpResponseMessage r)
        {
            string? mensaje = null;

            try
            {
                var json = await r.Content.ReadFromJsonAsync<JsonElement>();
                if (json.ValueKind == JsonValueKind.Object &&
                    json.TryGetProperty("mensaje", out var m) && m.ValueKind == JsonValueKind.String)
                {
                    mensaje = m.GetString();
                }
            }
            catch
            {
                // respuesta sin JSON
            }

            if (r.IsSuccessStatusCode) return (true, mensaje ?? "Operación realizada.");

            return r.StatusCode switch
            {
                HttpStatusCode.Forbidden => (false, "No tienes permiso para realizar esta acción."),
                HttpStatusCode.Unauthorized => (false, "Tu sesión expiró. Inicia sesión de nuevo."),
                _ => (false, mensaje ?? "Ocurrió un error inesperado.")
            };
        }
    }
}