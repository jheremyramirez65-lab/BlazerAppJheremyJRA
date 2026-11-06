namespace BlazorAppJheremyJRA.API.Models
{
    public partial class RecuperacionPassword
    {
        public int Iidrecuperacion { get; set; }
        public int Iidusuario { get; set; }
        public string TokenHash { get; set; } = null!;
        public DateTime Expira { get; set; }
        public bool Usado { get; set; }
        public DateTime Fecha { get; set; }
    }
}