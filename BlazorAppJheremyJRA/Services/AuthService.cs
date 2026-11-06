using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorAppJheremyJRA.Entities;

namespace BlazorAppJheremyJRA.Services
{
    public class AuthService
    {
        private readonly HttpClient http;
        private readonly TokenStore store;
        private readonly JwtAuthStateProvider proveedor;

        public AuthService(HttpClient http, TokenStore store, JwtAuthStateProvider proveedor)
        {
            this.http = http;
            this.store = store;
            this.proveedor = proveedor;
        }

        public Task<(bool Ok, string? Error)> LoginAsync(LoginRequestCLS datos, bool recordar) =>
            EnviarAsync("api/Auth/login", datos, recordar);

        public Task<(bool Ok, string? Error)> RegistroAsync(RegistroRequestCLS datos) =>
            EnviarAsync("api/Auth/registro", datos, recordar: false);

        public async Task LogoutAsync()
        {
            await store.LimpiarAsync();
            proveedor.NotificarCambio();
        }


        public async Task<(bool Ok, string Mensaje, string? Enlace)> OlvidePasswordAsync(OlvidePasswordCLS datos)
        {
            try
            {
                var r = await http.PostAsJsonAsync("api/Auth/olvide-password", datos);

                if (!r.IsSuccessStatusCode)
                {
                    return (false, await LeerErrorAsync(r), null);
                }

                var json = await r.Content.ReadFromJsonAsync<JsonElement>();
                var mensaje = json.TryGetProperty("mensaje", out var m) ? m.GetString() ?? "" : "";
                string? enlace = json.TryGetProperty("enlaceDesarrollo", out var e)
                                 && e.ValueKind == JsonValueKind.String ? e.GetString() : null;

                return (true, mensaje, enlace);
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.", null);
            }
            catch (JsonException)
            {
                return (false, "El servidor devolvió una respuesta inválida.", null);
            }
        }

        public async Task<(bool Ok, string Mensaje)> RestablecerPasswordAsync(RestablecerPasswordCLS datos)
        {
            try
            {
                var r = await http.PostAsJsonAsync("api/Auth/restablecer-password", datos);

                if (!r.IsSuccessStatusCode)
                {
                    return (false, await LeerErrorAsync(r));
                }

                return (true, "Contraseña actualizada. Ya puedes iniciar sesión.");
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor.");
            }
        }


        private async Task<(bool Ok, string? Error)> EnviarAsync(string url, object datos, bool recordar)
        {
            try
            {
                var respuesta = await http.PostAsJsonAsync(url, datos);

                if (respuesta.IsSuccessStatusCode)
                {
                    var auth = await respuesta.Content.ReadFromJsonAsync<AuthResponseCLS>();

                    if (auth is null || string.IsNullOrEmpty(auth.Token))
                    {
                        return (false, "El servidor devolvió una respuesta inválida.");
                    }

                    await store.GuardarAsync(auth.Token, recordar);
                    proveedor.NotificarCambio();
                    return (true, null);
                }

                return (false, await LeerErrorAsync(respuesta));
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor. Verifica que la API esté en ejecución.");
            }
            catch (TaskCanceledException)
            {
                return (false, "El servidor tardó demasiado en responder. Inténtalo de nuevo.");
            }
            catch (JsonException)
            {
                return (false, "El servidor devolvió una respuesta inválida.");
            }
        }

        private static async Task<string> LeerErrorAsync(HttpResponseMessage respuesta)
        {
            if (respuesta.StatusCode == HttpStatusCode.TooManyRequests)
            {
                return "Demasiados intentos. Espera un minuto e inténtalo de nuevo.";
            }

            try
            {
                var json = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

                if (json.ValueKind == JsonValueKind.Object)
                {
                    // { "mensaje": "..." }
                    if (json.TryGetProperty("mensaje", out var mensaje) &&
                        mensaje.ValueKind == JsonValueKind.String)
                    {
                        return mensaje.GetString()!;
                    }

                    // Errores de validación automáticos de ASP.NET (400)
                    if (json.TryGetProperty("errors", out var errores) &&
                        errores.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var campo in errores.EnumerateObject())
                        {
                            foreach (var texto in campo.Value.EnumerateArray())
                            {
                                return texto.GetString() ?? "Datos inválidos.";
                            }
                        }
                    }
                }
            }
            catch
            {
                // la respuesta no era JSON: se usa el mensaje genérico
            }

            return respuesta.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Correo o contraseña incorrectos.",
                HttpStatusCode.BadRequest => "Revisa los datos ingresados.",
                _ => "Ocurrió un error inesperado. Inténtalo de nuevo."
            };
        }
    }
}