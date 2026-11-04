using System;
using System.Collections.Generic;

namespace BlazorAppJheremyJRA.API.Models;

public partial class PaginaTipoUsuButton
{
    public int Iidpaginatipousubutton { get; set; }

    public int? Iidpaginatipousuario { get; set; }

    public int? Iidbutton { get; set; }

    public virtual Button? IidbuttonNavigation { get; set; }

    public virtual PaginaTipoUsuario? IidpaginatipousuarioNavigation { get; set; }
}
