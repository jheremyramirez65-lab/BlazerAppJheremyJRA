using System.Net;
using System.Net.Http.Headers;

namespace BlazorAppJheremyJRA.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly TokenStore store;
        private readonly IServiceProvider servicios;

        public AuthHeaderHandler(TokenStore store, IServiceProvider servicios)
        {
            this.store = store;
            this.servicios = servicios;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await store.ObtenerAsync();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var respuesta = await base.SendAsync(request, cancellationToken);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(token))
            {
                await store.LimpiarAsync();
                var proveedor = (JwtAuthStateProvider)servicios.GetService(typeof(JwtAuthStateProvider))!;
                proveedor.NotificarCambio();
            }

            return respuesta;
        }
    }
}