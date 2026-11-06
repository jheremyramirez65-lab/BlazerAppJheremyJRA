
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlazorAppJheremyJRA.API.Models;
using BlazorAppJheremyJRA.Entities;
using BlazorAppJheremyJRA.API.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BlazorAppJheremyJRA.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = Politicas.Ver)]
    public class LibroController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;

        public LibroController(BdbibliotecaContext _bd)
        {
            bd = _bd;
        }

        // =========================================================
        // LISTAR LIBROS (COMPATIBLE CON BLAZOR)
        // =========================================================


        [HttpGet]
        public IActionResult listarLibros()
        {
            try
            {
                var lista = (
                    from libro in bd.Libros
                    join tipolibro in bd.TipoLibros
                        on libro.IdTipoLibro equals tipolibro.IdTipoLibro
                    where libro.Bhabilitado == 1
                    select new LibroListCLS
                    {
                        idLibro = libro.IdLibro,
                        codigo = libro.Codigo,
                        titulo = libro.Titulo,
                        resumen = libro.Resumen,

                        // Obtener el nombre del autor desde la tabla Autor
                        autor = libro.Iidautor != null
                            ? bd.Autors
                                .Where(a => a.Iidautor == libro.Iidautor)
                                .Select(a =>
                                    (a.Nombre ?? "") + " " +
                                    (a.Appaterno ?? "") + " " +
                                    (a.Apmaterno ?? ""))
                                .FirstOrDefault()
                            : libro.Autor,

                        editorial = libro.Editorial,
                        anio = libro.Anio ?? 0,
                        numeroPaginas = libro.NumeroPaginas ?? 0,
                        stock = libro.Stock ?? 0,

                        // Mostrar las categorías relacionadas con el libro
                        categoria = string.Join(", ",
                            (from lc in bd.Set<Dictionary<string, object>>(
                                "LibroCategoria")
                             join c in bd.Categorias
                                 on EF.Property<int>(lc, "IdCategoria")
                                 equals c.IdCategoria
                             where EF.Property<int>(lc, "IdLibro")
                                 == libro.IdLibro
                             select c.Nombre).ToList()),

                        idTipoLibro = libro.IdTipoLibro ?? 0,
                        nombretipolibro = tipolibro.NombreTipoLibro,

                        Imagen = libro.Imagen != null &&
                                 libro.Imagen.Length > 0
                            ? "data:image/jpeg;base64," +
                              Convert.ToBase64String(libro.Imagen)
                            : "/img/usuario.png",

                        nombrearchivo = libro.Nombrearchivo
                    }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // =========================================================
        // OPCIÓN 1: BUSCAR, FILTRAR Y PAGINAR LIBROS
        // GET /api/Libro/paginado
        // =========================================================

        [HttpGet("paginado")]
        public IActionResult listarLibrosPaginados(
            string? titulo,
            int? anio,
            int pagina = 1,
            int tamanoPagina = 10)
        {
            try
            {
                if (pagina < 1)
                    pagina = 1;

                if (tamanoPagina < 1)
                    tamanoPagina = 10;

                if (tamanoPagina > 100)
                    tamanoPagina = 100;

                var consulta = bd.Libros
                    .Where(l => l.Bhabilitado == 1);

                // Buscar por título
                if (!string.IsNullOrWhiteSpace(titulo))
                {
                    consulta = consulta.Where(l =>
                        l.Titulo != null &&
                        l.Titulo.Contains(titulo));
                }

                // Filtrar por año
                if (anio.HasValue)
                {
                    consulta = consulta.Where(l =>
                        l.Anio == anio.Value);
                }

                // Total de resultados antes de paginar
                int totalRegistros = consulta.Count();

                // Obtener la página solicitada
                var lista = (
                    from libro in consulta
                    join tipolibro in bd.TipoLibros
                        on libro.IdTipoLibro equals tipolibro.IdTipoLibro
                    orderby libro.IdLibro
                    select new LibroListCLS
                    {
                        idLibro = libro.IdLibro,
                        codigo = libro.Codigo,
                        titulo = libro.Titulo,
                        resumen = libro.Resumen,
                        autor = libro.Autor,
                        editorial = libro.Editorial,
                        anio = libro.Anio ?? 0,
                        numeroPaginas = libro.NumeroPaginas ?? 0,
                        stock = libro.Stock ?? 0,
                        categoria = libro.Categoria,
                        idTipoLibro = libro.IdTipoLibro ?? 0,
                        nombretipolibro = tipolibro.NombreTipoLibro,

                        Imagen = libro.Imagen != null &&
                                 libro.Imagen.Length > 0
                            ? "data:image/jpeg;base64," +
                              Convert.ToBase64String(libro.Imagen)
                            : "/img/usuario.png",

                        nombrearchivo = libro.Nombrearchivo
                    })
                    .Skip((pagina - 1) * tamanoPagina)
                    .Take(tamanoPagina)
                    .ToList();

                return Ok(new
                {
                    pagina = pagina,
                    tamanoPagina = tamanoPagina,
                    totalRegistros = totalRegistros,
                    totalPaginas = (int)Math.Ceiling(
                        totalRegistros / (double)tamanoPagina),
                    libros = lista
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // BUSCAR LIBRO POR ID
        // =========================================================

        [HttpGet("{idlibro}")]
        public IActionResult recuperaLibroPorId(int idlibro)
        {
            try
            {
                var libro = bd.Libros
                    .Where(x =>
                        x.IdLibro == idlibro &&
                        x.Bhabilitado == 1)
                    .Select(x => new LibroFormCLS
                    {
                        IdLibro = x.IdLibro,
                        Codigo = x.Codigo,
                        Titulo = x.Titulo,
                        Resumen = x.Resumen,
                        Autor = x.Autor,
                        Editorial = x.Editorial,
                        Anio = x.Anio ?? 0,
                        NumeroPaginas = x.NumeroPaginas ?? 0,
                        Stock = x.Stock ?? 0,
                        Categoria = x.Categoria,
                        idTipoLibro = x.IdTipoLibro ?? 0,
                        iidautor = x.Iidautor ?? 0,
                        imagen = x.Imagen,
                        archivo = x.Archivo,
                        nombrearchivo = x.Nombrearchivo ?? ""
                    })
                    .FirstOrDefault();

                if (libro == null)
                    return NotFound();

                return Ok(libro);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // ELIMINAR LIBRO LÓGICAMENTE
        // =========================================================

        [HttpDelete("{idlibro}")]
        [Authorize(Policy = Politicas.Eliminar)]
        public IActionResult eliminarLibro(int idlibro)
        {
            try
            {
                var libro = bd.Libros
                    .FirstOrDefault(x => x.IdLibro == idlibro);

                if (libro == null)
                    return NotFound();

                libro.Bhabilitado = 0;
                bd.SaveChanges();

                return Ok("Se elimino correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // GUARDAR LIBRO
        // =========================================================

        [HttpPost]
        [Authorize(Policy = Politicas.Crear)]
        public IActionResult guardarLibro(
            [FromBody] LibroFormCLS oLibroFormCLS)
        {
            using var transaccion = bd.Database.BeginTransaction();

            try
            {
                var libro = new Libro
                {
                    Codigo = oLibroFormCLS.Codigo,
                    Titulo = oLibroFormCLS.Titulo,
                    Resumen = oLibroFormCLS.Resumen,
                    Autor = oLibroFormCLS.Autor,
                    Editorial = oLibroFormCLS.Editorial,
                    Anio = oLibroFormCLS.Anio,
                    NumeroPaginas = oLibroFormCLS.NumeroPaginas,
                    Stock = oLibroFormCLS.Stock,

                    // Se conserva la categoría antigua
                    Categoria = oLibroFormCLS.Categoria,

                    IdTipoLibro = oLibroFormCLS.idTipoLibro,

                    Iidautor = oLibroFormCLS.iidautor == 0
                        ? null
                        : oLibroFormCLS.iidautor,

                    Imagen = oLibroFormCLS.imagen,
                    Archivo = oLibroFormCLS.archivo,
                    Nombrearchivo = oLibroFormCLS.nombrearchivo,
                    Bhabilitado = 1
                };

                bd.Libros.Add(libro);
                bd.SaveChanges();

                // Guardar las relaciones de categorías seleccionadas
                if (oLibroFormCLS.IdCategorias != null &&
                    oLibroFormCLS.IdCategorias.Count > 0)
                {
                    foreach (var idCategoria in
                             oLibroFormCLS.IdCategorias.Distinct())
                    {
                        var categoria = bd.Categorias
                            .Find(idCategoria);

                        if (categoria == null)
                        {
                            transaccion.Rollback();

                            return BadRequest(
                                $"No existe la categoría con ID {idCategoria}.");
                        }

                        libro.Categorias.Add(categoria);
                    }

                    bd.SaveChanges();
                }

                transaccion.Commit();

                return Ok(new
                {
                    mensaje = "Libro guardado correctamente",
                    idLibro = libro.IdLibro
                });
            }
            catch (Exception ex)
            {
                transaccion.Rollback();

                return StatusCode(500, ex.Message);
            }
        }


        // =========================================================
        // RECUPERAR ARCHIVO PDF
        // =========================================================

        [HttpGet("recuperarArchivo/{idlibro}")]
        public IActionResult recuperaArchivoPorId(int idlibro)
        {
            var libro = bd.Libros
                .FirstOrDefault(x =>
                    x.IdLibro == idlibro &&
                    x.Bhabilitado == 1);

            if (libro == null || libro.Archivo == null)
                return NotFound();

            return File(
                libro.Archivo,
                "application/pdf",
                libro.Nombrearchivo
            );
        }

        // =========================================================
        // ACTUALIZAR LIBRO
        // =========================================================

        [HttpPut("{idlibro}")]
        [Authorize(Policy = Politicas.Editar)]
        public IActionResult Actualizar(
            int idlibro,
            LibroFormCLS libro)
        {
            if (idlibro != libro.IdLibro)
                return BadRequest();

            var existe = bd.Libros
                .FirstOrDefault(x =>
                    x.IdLibro == idlibro &&
                    x.Bhabilitado == 1);

            if (existe == null)
                return NotFound();

            existe.Codigo = libro.Codigo;
            existe.Titulo = libro.Titulo;
            existe.Resumen = libro.Resumen;
            existe.Autor = libro.Autor;
            existe.Editorial = libro.Editorial;
            existe.Anio = libro.Anio;
            existe.NumeroPaginas = libro.NumeroPaginas;
            existe.Stock = libro.Stock;
            existe.Categoria = libro.Categoria;
            existe.IdTipoLibro = libro.idTipoLibro;

            existe.Iidautor = libro.iidautor == 0
                ? null
                : libro.iidautor;

            if (libro.imagen != null && libro.imagen.Length > 0)
                existe.Imagen = libro.imagen;

            if (libro.archivo != null && libro.archivo.Length > 0)
            {
                existe.Archivo = libro.archivo;
                existe.Nombrearchivo = libro.nombrearchivo;
            }

            bd.SaveChanges();

            return Ok("Libro actualizado correctamente");
        }


        [HttpGet("dto")]
        public async Task<ActionResult<IEnumerable<LibroRespuestaDto>>> GetLibrosDto()
        {
            var libros = await bd.Libros
                .Where(l => l.Bhabilitado == 1)
                .Select(l => new LibroRespuestaDto
                {
                    Id = l.IdLibro,
                    Titulo = l.Titulo ?? string.Empty,
                    AnioPublicacion = l.Anio ?? 0,
                    NombreAutor = l.IidautorNavigation == null
                        ? (l.Autor ?? string.Empty)
                        : (
                            (l.IidautorNavigation.Nombre ?? string.Empty) + " " +
                            (l.IidautorNavigation.Appaterno ?? string.Empty) + " " +
                            (l.IidautorNavigation.Apmaterno ?? string.Empty)
                          ).Trim()
                })
                .ToListAsync();

            return Ok(libros);
        }

    }
}
