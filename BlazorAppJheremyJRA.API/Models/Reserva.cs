using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Reserva
{
    public int Iidreserva { get; set; }

    public int? Iidusuario { get; set; }

    public int? Iidreservaestado { get; set; }

    public DateTime? Fecha { get; set; }

    public virtual ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();

    public virtual ReservaEstado? IidreservaestadoNavigation { get; set; }

    public virtual Usuario? IidusuarioNavigation { get; set; }

    public virtual ICollection<ReservaHistorial> ReservaHistorials { get; set; } = new List<ReservaHistorial>();
}
