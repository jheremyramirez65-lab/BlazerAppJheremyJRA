using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorAppJheremyJRA.API.Models;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;

        public CategoriaController(BdbibliotecaContext contexto)
        {
            bd = contexto;
        }

        // GET: api/Categoria
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var categorias = await bd.Categorias
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // POST: api/Categoria
        [HttpPost]
        public async Task<IActionResult> Registrar(
            [FromBody] Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                return BadRequest("El nombre de la categoría es obligatorio.");
            }

            categoria.Nombre = categoria.Nombre.Trim();

            bd.Categorias.Add(categoria);
            await bd.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Categoría registrada correctamente.",
                categoria.IdCategoria,
                categoria.Nombre
            });
        }

        // GET: api/Categoria/1/libros
        [HttpGet("{idCategoria:int}/libros")]
        public async Task<IActionResult> ListarLibros(int idCategoria)
        {
            var categoria = await bd.Categorias
                .Where(c => c.IdCategoria == idCategoria)
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    Libros = c.Libros.Select(l => new
                    {
                        l.IdLibro,
                        l.Titulo
                    })
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound("No se encontró la categoría.");
            }

            return Ok(categoria);
        }



        // POST: api/Categoria/1/libros/1
        [HttpPost("{idCategoria:int}/libros/{idLibro:int}")]
        public async Task<IActionResult> AsignarLibro(
            int idCategoria,
            int idLibro)
        {
            var categoria = await bd.Categorias
                .FindAsync(idCategoria);

            if (categoria == null)
            {
                return NotFound("No se encontró la categoría.");
            }

            var libro = await bd.Libros
                .FindAsync(idLibro);

            if (libro == null)
            {
                return NotFound("No se encontró el libro.");
            }

            bool yaExiste = await bd.Categorias
                .Where(c => c.IdCategoria == idCategoria)
                .AnyAsync(c => c.Libros.Any(l => l.IdLibro == idLibro));

            if (yaExiste)
            {
                return BadRequest(
                    "Este libro ya está asignado a esa categoría.");
            }

            categoria.Libros.Add(libro);
            await bd.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "Libro asignado a la categoría correctamente.",
                idCategoria,
                idLibro
            });
        }


    }
}
