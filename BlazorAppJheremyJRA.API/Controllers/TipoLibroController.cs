using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlazorAppJheremyJRA.API.Models;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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