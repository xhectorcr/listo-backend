
using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListoAPI.API.Controllers
{

    [Route("api/producto")]
    [ApiController]
    public class ProductoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProductoRepository _productoRepository;
        public ProductoController(IConfiguration configuration, IProductoRepository categoriaRepository)
        {
            _configuration = configuration;
            _productoRepository = categoriaRepository;

        }


        [Route("lista/activos")]
        [HttpGet]
        public async Task<IActionResult> GetProductosLista([FromQuery] int pageNumber,
                [FromQuery] int pageSize,
                [FromQuery] string pSearch = "",
                [FromQuery] int idCategoria = 0)
        {

            try
            {

                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1 || pageSize > 100) pageSize = 10;

                var (platos, totalCount) = await _productoRepository.GetProductoList(
                    pageNumber,
                    pageSize,
                    pSearch, idCategoria
                    );


                return Ok(new
                {
                    data = platos,
                    pageNumber,
                    pageSize,
                    totalCount,
                    totalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseCommonDTO
                {
                    success = false,
                    message = $"Error al obtener : {ex.Message}"
                });

            }

        }


        [HttpPost()]
        public async Task<IActionResult> GuardarProducto(ProductoDTO producto)
        {

            ResponseCommonDTO result = await _productoRepository.saveItem(producto);
            if (!result.success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpPut()]
        public async Task<IActionResult> ActualizarUsuario(ProductoDTO plato)
        {

            ResponseCommonDTO result = await _productoRepository.updateItem(plato);

            if (!result.success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("{pId}")]
        public async Task<ActionResult<object>> GetPlatoById(int pId)
        {
            try
            {
                var producto = await _productoRepository.getById(pId);

                if (producto == null)
                {
                    return NotFound(new { mensaje = "No hay producto" });
                }

                return Ok(producto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error interno al procesar la solicitud" });
            }
        }



        [HttpDelete]
        public async Task<IActionResult> EliminarPlato(int id)
        {
            ResponseCommonDTO result = await _productoRepository.deleteItem(id);

            if (!result.success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }





    }

}