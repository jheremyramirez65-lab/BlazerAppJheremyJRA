using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Usuario
{
    public int Iidusuario { get; set; }

    public string? Nombre { get; set; }

    public string Apellido { get; set; } = "";
    public string NombreUsuario { get; set; } = null!;
    public DateTime FechaCreacion { get; set; }

    public string? Correo { get; set; }

    public string? Password { get; set; }

    public int? Iidtipousuario { get; set; }

    public int? Bhabilitado { get; set; }

    public virtual TipoUsuario? IidtipousuarioNavigation { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
