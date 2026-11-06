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
    public class AutorController : ControllerBase
    {
        private readonly BdbibliotecaContext bd;

        public AutorController(BdbibliotecaContext _bd)
        {
            bd = _bd;
        }

        // =========================================================
        // LISTADO COMPLETO DE AUTORES
        // =========================================================
        [HttpGet]
        public IActionResult listarAutores()
        {
            try
            {
                var lista = (
                    from autor in bd.Autors
                    join sexo in bd.Sexos
                        on autor.Iidsexo equals (int?)sexo.Iidsexo
                    join pais in bd.Pais
                        on autor.Iidpais equals (int?)pais.Iidpais
                    where autor.Bhabilitado == 1
                    select new AutorListCLS
                    {
                        idAutor = autor.Iidautor,
                        nombre = autor.Nombre ?? "",
                        apPaterno = autor.Appaterno ?? "",
                        apMaterno = autor.Apmaterno ?? "",

                        iidSexo = autor.Iidsexo ?? 0,
                        nombreSexo = sexo.Nombre ?? "",

                        iidPais = autor.Iidpais ?? 0,
                        nombrePais = pais.Nombre ?? "",

                        descripcion = autor.Descripcion ?? "",

                        imagen =
                            autor.Imagen != null &&
                            autor.Imagen.Length > 0
                                ? "data:image/jpeg;base64," +
                                  Convert.ToBase64String(autor.Imagen)
                                : "img/usuario.png"
                    }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // LISTADO DE AUTORES PARA COMBO
        // =========================================================
        [HttpGet("combo")]
        public IActionResult listarAutorCombo()
        {
            try
            {
                var lista = (
                    from autor in bd.Autors
                    where autor.Bhabilitado == 1
                    select new AutorCLS
                    {
                        idautor = autor.Iidautor,

                        nombreautor =
                            (autor.Nombre ?? "") + " " +
                            (autor.Appaterno ?? "") + " " +
                            (autor.Apmaterno ?? "")
                    }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // RECUPERAR AUTOR POR ID
        // =========================================================
        [HttpGet("{idautor:int}")]
        public IActionResult recuperarAutorPorId(int idautor)
        {
            try
            {
                var autor = bd.Autors
                    .Where(x => x.Iidautor == idautor)
                    .Select(x => new AutorFormCLS
                    {
                        IdAutor = x.Iidautor,
                        Nombre = x.Nombre ?? "",
                        ApPaterno = x.Appaterno ?? "",
                        ApMaterno = x.Apmaterno ?? "",
                        IidSexo = x.Iidsexo ?? 0,
                        IidPais = x.Iidpais ?? 0,
                        Descripcion = x.Descripcion ?? "",
                        Imagen = x.Imagen
                    })
                    .FirstOrDefault();

                if (autor == null)
                {
                    return NotFound();
                }

                return Ok(autor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // GUARDAR AUTOR
        // =========================================================
        [HttpPost]
        [Authorize(Policy = Politicas.Crear)]
        public IActionResult guardarAutor([FromBody] AutorFormCLS oAutor)
        {
            try
            {
                var autor = new Autor
                {
                    Nombre = oAutor.Nombre,
                    Appaterno = oAutor.ApPaterno,
                    Apmaterno = oAutor.ApMaterno,
                    Iidsexo = oAutor.IidSexo,
                    Iidpais = oAutor.IidPais,
                    Descripcion = oAutor.Descripcion,
                    Imagen = oAutor.Imagen,
                    Bhabilitado = 1
                };

                bd.Autors.Add(autor);
                bd.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // ACTUALIZAR AUTOR
        // =========================================================
        [HttpPut("{idautor:int}")]
        [Authorize(Policy = Politicas.Editar)]
        public IActionResult actualizarAutor(
            int idautor,
            [FromBody] AutorFormCLS oAutor)
        {
            try
            {
                if (idautor != oAutor.IdAutor)
                {
                    return BadRequest();
                }

                var existe = bd.Autors
                    .FirstOrDefault(x => x.Iidautor == idautor);

                if (existe == null)
                {
                    return NotFound();
                }

                existe.Nombre = oAutor.Nombre;
                existe.Appaterno = oAutor.ApPaterno;
                existe.Apmaterno = oAutor.ApMaterno;
                existe.Iidsexo = oAutor.IidSexo;
                existe.Iidpais = oAutor.IidPais;
                existe.Descripcion = oAutor.Descripcion;

                if (oAutor.Imagen != null &&
                    oAutor.Imagen.Length > 0)
                {
                    existe.Imagen = oAutor.Imagen;
                }

                bd.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // ELIMINAR AUTOR DE FORMA LÓGICA
        // =========================================================
        [HttpDelete("{idautor:int}")]
        [Authorize(Policy = Politicas.Eliminar)]
        public IActionResult eliminarAutor(int idautor)
        {
            try
            {
                var autor = bd.Autors
                    .FirstOrDefault(x => x.Iidautor == idautor);

                if (autor == null)
                {
                    return NotFound();
                }

                autor.Bhabilitado = 0;

                bd.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // LISTAR SEXO
        // =========================================================
        [HttpGet("listarSexo")]
        public IActionResult listarSexo()
        {
            try
            {
                var lista = (
                    from sexo in bd.Sexos
                    where sexo.Bhabilitado == 1
                    select new ComboCLS
                    {
                        id = sexo.Iidsexo,
                        nombre = sexo.Nombre ?? ""
                    }).ToList();

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // =========================================================
        // LISTAR PAIS
        // =========================================================
        [HttpGet("listarPais")]
        public IActionResult listarPais()
        {
            try
            {
                var lista = (
                    from pais in bd.Pais
                    where pais.Bhabilitado == 1
                    select new ComboCLS
                    {
                        id = pais.Iidpais,
                        nombre = pais.Nombre ?? ""
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