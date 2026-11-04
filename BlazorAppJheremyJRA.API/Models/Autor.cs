using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class Autor
{
    public int Iidautor { get; set; }

    public string? Nombre { get; set; }

    public string? Appaterno { get; set; }

    public string? Apmaterno { get; set; }

    public int? Iidsexo { get; set; }

    public int? Iidpais { get; set; }

    public string? Descripcion { get; set; }

    public int? Bhabilitado { get; set; }

    public byte[]? Imagen { get; set; }


    // Relación con Libro
    public virtual ICollection<Libro> Libros { get; set; }
        = new List<Libro>();
}