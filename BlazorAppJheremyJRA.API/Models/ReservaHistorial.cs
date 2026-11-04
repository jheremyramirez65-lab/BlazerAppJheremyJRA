using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class ReservaHistorial
{
    public int Iidhistorial { get; set; }

    public int? Iidreserva { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual Reserva? IidreservaNavigation { get; set; }
}
