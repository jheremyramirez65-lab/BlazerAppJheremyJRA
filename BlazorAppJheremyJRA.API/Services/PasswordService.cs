using System.Security.Cryptography;
using System.Text;

namespace BlazorAppJheremyJRA.API.Services
{
    public static class PasswordService
    {
        private const string Prefijo = "v1";
        private const int Iteraciones = 600_000;
        private const int TamSalt = 16;
        private const int TamHash = 32;

        public static readonly string HashFalso = Hashear("contrasena-falsa-para-igualar-tiempos");

        public static string Hashear(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(TamSalt);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iteraciones, HashAlgorithmName.SHA256, TamHash);

            return $"{Prefijo}.{Iteraciones}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public static bool EstaHasheada(string? guardada) =>
            guardada != null && guardada.StartsWith(Prefijo + ".", StringComparison.Ordinal);

        public static bool Verificar(string password, string? guardada)
        {
            if (string.IsNullOrEmpty(guardada)) return false;

            // Usuarios antiguos con contraseña en texto plano (se migran al iniciar sesión)
            if (!EstaHasheada(guardada))
            {
                return CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(password),
                    Encoding.UTF8.GetBytes(guardada));
            }

            try
            {
                var partes = guardada.Split('.');
                if (partes.Length != 4) return false;

                var iteraciones = int.Parse(partes[1]);
                var salt = Convert.FromBase64String(partes[2]);
                var esperado = Convert.FromBase64String(partes[3]);

                var calculado = Rfc2898DeriveBytes.Pbkdf2(
                    password, salt, iteraciones, HashAlgorithmName.SHA256, esperado.Length);

                return CryptographicOperations.FixedTimeEquals(calculado, esperado);
            }
            catch (FormatException) { return false; }
            catch (OverflowException) { return false; }
        }
    }
}