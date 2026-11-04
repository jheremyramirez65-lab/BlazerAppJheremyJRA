using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class PaginaTipoUsuario
{
    public int Iidpaginatipousuario { get; set; }

    public int? Iidpagina { get; set; }

    public int? Iidtipousuario { get; set; }

    public virtual Pagina? IidpaginaNavigation { get; set; }

    public virtual TipoUsuario? IidtipousuarioNavigation { get; set; }

    public virtual ICollection<PaginaTipoUsuButton> PaginaTipoUsuButtons { get; set; } = new List<PaginaTipoUsuButton>();
}
