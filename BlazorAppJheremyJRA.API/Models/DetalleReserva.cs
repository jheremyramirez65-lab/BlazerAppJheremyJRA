using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class DetalleReserva
{
    public int Iiddetallereserva { get; set; }

    public int? Iidreserva { get; set; }

    public int? IdLibro { get; set; }

    public int? Cantidad { get; set; }

    public virtual Libro? IdLibroNavigation { get; set; }

    public virtual Reserva? IidreservaNavigation { get; set; }
}
