using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Libro
{
    public int IdLibro { get; set; }

    public string? Codigo { get; set; }

    public string? Titulo { get; set; }

    public string? Resumen { get; set; }

    public string? Autor { get; set; }

    public string? Editorial { get; set; }

    public int? Anio { get; set; }

    public int? NumeroPaginas { get; set; }

    public int? Stock { get; set; }

    public string? Categoria { get; set; }

    public byte[]? Imagen { get; set; }

    public byte[]? Archivo { get; set; }

    public string? Nombrearchivo { get; set; }

    public int? IdTipoLibro { get; set; }

    public int Bhabilitado { get; set; }

    public int? Iidautor { get; set; }

    public virtual Autor? IidautorNavigation { get; set; }

    public virtual ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();

    public virtual TipoLibro? IdTipoLibroNavigation { get; set; }
    public virtual ICollection<Categoria> Categorias { get; set; }
    = new List<Categoria>();
}
