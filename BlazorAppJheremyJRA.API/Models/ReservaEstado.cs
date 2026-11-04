using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class ReservaEstado
{
    public int Iidreservaestado { get; set; }

    public string? Nombre { get; set; }

    public int? Bhabilitado { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
