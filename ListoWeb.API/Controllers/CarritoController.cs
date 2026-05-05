using Microsoft.AspNetCore.Mvc;
using ListoAPI.DTO;
using ListoWeb.API.Services;
using ListoAPI.Aplication.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ListoAPI.Aplication.Infrastructure.Data;

namespace ListoWeb.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly CarritoService _carritoService;
        private readonly ConfigContext _context;

        public CarritoController(CarritoService carritoService, ConfigContext context)
        {
            _carritoService = carritoService;
            _context = context;
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarDesdeYolo([FromBody] CarritoRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.YoloLabel))
            {
                return BadRequest(new ResponseCommonDTO { message = "Datos inválidos", success = false });
            }

            // Buscar producto por YoloLabel
            var producto = await _context.Set<ListoAPI.Aplication.Core.Entities.Producto>()
                .FirstOrDefaultAsync(p => p.YoloLabel.ToLower() == request.YoloLabel.ToLower() && p.Activo);

            if (producto == null)
            {
                return NotFound(new ResponseCommonDTO { message = $"Producto con etiqueta YOLO '{request.YoloLabel}' no encontrado", success = false });
            }

            // Agregar al carrito en memoria
            _carritoService.AgregarProducto(request.UsuarioId, producto);

            return Ok(new ResponseCommonDTO { message = "Producto agregado al carrito exitosamente", success = true });
        }

        [HttpGet("{usuarioId}")]
        public IActionResult ObtenerCarrito(int usuarioId)
        {
            var items = _carritoService.ObtenerCarrito(usuarioId);
            var total = items.Sum(i => i.Subtotal);

            return Ok(new
            {
                UsuarioId = usuarioId,
                Items = items,
                Total = total
            });
        }
    }
}
