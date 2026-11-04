using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class TipoLibro
{
    public int IdTipoLibro { get; set; }

    public string? NombreTipoLibro { get; set; }

    public string? Descripcion { get; set; }

    public int? Bhabilitado { get; set; }

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
