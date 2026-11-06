using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.Entities;
using Microsoft.AspNetCore.Authorization;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.Ver)]
    public class TipoLibroController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;

        public TipoLibroController(BdbibliotecaContext _bd)
        {
            bd = _bd;
        }


        [HttpGet]
        public IActionResult listarTipoLibros()
        {
            try
            {
                var lista = (from tipolibro in bd.TipoLibros
                             where tipolibro.Bhabilitado == 1
                             select new
                             {
                                 idtipolibro = tipolibro.IdTipoLibro,
                                 nombretipolibro = tipolibro.NombreTipoLibro
                             }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}