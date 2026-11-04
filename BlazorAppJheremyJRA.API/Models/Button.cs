using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Button
{
    public int Iidbutton { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<PaginaTipoUsuButton> PaginaTipoUsuButtons { get; set; } = new List<PaginaTipoUsuButton>();
}
