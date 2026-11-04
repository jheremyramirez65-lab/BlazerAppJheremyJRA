using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Pagina
{
    public int Iidpagina { get; set; }

    public string? Nombre { get; set; }

    public string? Url { get; set; }

    public virtual ICollection<PaginaTipoUsuario> PaginaTipoUsuarios { get; set; } = new List<PaginaTipoUsuario>();
}
