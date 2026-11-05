using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorAppJheremyJRA.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly TokenStore store;

        private static readonly AuthenticationState Anonimo =
            new(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(TokenStore store)
        {
            this.store = store;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await store.ObtenerAsync();

            if (token is null)
            {
                return Anonimo;
            }

            var claims = LeerClaims(token);

            if (claims is null)
            {
                await store.LimpiarAsync();
                return Anonimo;
            }

            var identidad = new ClaimsIdentity(claims, "jwt", "name", "role");
            return new AuthenticationState(new ClaimsPrincipal(identidad));
        }

        public void NotificarCambio()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        // Lee el payload del JWT (la firma la valida siempre la API)
        private static List<Claim>? LeerClaims(string jwt)
        {
            try
            {
                var partes = jwt.Split('.');
                if (partes.Length != 3) return null;

                var payload = partes[1].Replace('-', '+').Replace('_', '/');
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

                using var doc = JsonDocument.Parse(Convert.FromBase64String(payload));

                var claims = new List<Claim>();
                long exp = 0;

                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    if (prop.Name == "exp" && prop.Value.ValueKind == JsonValueKind.Number)
                    {
                        exp = prop.Value.GetInt64();
                    }

                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var valor in prop.Value.EnumerateArray())
                        {
                            claims.Add(new Claim(prop.Name, valor.ToString()));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(prop.Name, prop.Value.ToString()));
                    }
                }

                if (exp == 0 || DateTimeOffset.FromUnixTimeSeconds(exp) <= DateTimeOffset.UtcNow)
                {
                    return null;
                }

                return claims;
            }
            catch
            {
                return null;
            }
        }
    }
}