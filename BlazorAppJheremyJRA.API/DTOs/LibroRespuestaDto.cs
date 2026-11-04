namespace BlazorAppJheremyJRA.API.DTOs
{
    public class LibroRespuestaDto
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public int AnioPublicacion { get; set; }

        public string NombreAutor { get; set; } = string.Empty;
    }
}
