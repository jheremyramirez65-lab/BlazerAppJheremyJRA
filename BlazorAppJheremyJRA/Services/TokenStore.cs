using Microsoft.JSInterop;

namespace BlazorAppJheremyJRA.Services
{
    public class TokenStore
    {
        private const string Clave = "auth_token";
        private readonly IJSRuntime js;

        public TokenStore(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task GuardarAsync(string token, bool recordar)
        {
            await LimpiarAsync();
            var almacen = recordar ? "localStorage" : "sessionStorage";
            await js.InvokeVoidAsync($"{almacen}.setItem", Clave, token);
        }

        public async Task<string?> ObtenerAsync()
        {
            var token = await js.InvokeAsync<string?>("sessionStorage.getItem", Clave);

            if (string.IsNullOrEmpty(token))
            {
                token = await js.InvokeAsync<string?>("localStorage.getItem", Clave);
            }

            return string.IsNullOrEmpty(token) ? null : token;
        }

        public async Task LimpiarAsync()
        {
            await js.InvokeVoidAsync("sessionStorage.removeItem", Clave);
            await js.InvokeVoidAsync("localStorage.removeItem", Clave);
        }
    }
}