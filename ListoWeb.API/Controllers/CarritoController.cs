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

        [HttpPost("remover")]
        public async Task<IActionResult> RemoverDesdeYolo([FromBody] CarritoRequestDTO request)
        {
            if (request == null || string.IsNullOrEmpty(request.YoloLabel))
                return BadRequest(new ResponseCommonDTO { message = "Datos inválidos", success = false });

            var producto = await _context.Set<ListoAPI.Aplication.Core.Entities.Producto>()
                .FirstOrDefaultAsync(p => p.YoloLabel.ToLower() == request.YoloLabel.ToLower() && p.Activo);

            if (producto == null)
                return NotFound(new ResponseCommonDTO { message = "Producto no encontrado", success = false });

            _carritoService.RemoverProducto(request.UsuarioId, producto);

            return Ok(new ResponseCommonDTO { message = "Producto removido", success = true });
        }

        [HttpPost("finalizar")]
        public async Task<IActionResult> FinalizarCompra([FromBody] int usuarioId)
        {
            var usuario = await _context.Set<ListoAPI.Aplication.Core.Entities.Usuario>().FindAsync(usuarioId);
            if (usuario == null) return NotFound(new ResponseCommonDTO { message = "Usuario no encontrado", success = false });

            var items = _carritoService.ObtenerCarrito(usuarioId);
            if (!items.Any()) return BadRequest(new ResponseCommonDTO { message = "El carrito está vacío", success = false });

            var total = items.Sum(i => i.Subtotal);

            // Obtener el método de pago (billetera) del usuario
            var metodoPago = await _context.Set<ListoAPI.Aplication.Core.Entities.MetodoPago>()
                .FirstOrDefaultAsync(m => m.IdUsuario == usuarioId);

            if (metodoPago == null)
            {
                return BadRequest(new ResponseCommonDTO { message = "El usuario no tiene una billetera registrada.", success = false });
            }

            if (metodoPago.Saldo < total)
            {
                return BadRequest(new ResponseCommonDTO { message = "Saldo insuficiente en la billetera.", success = false });
            }

            // Descontar saldo automáticamente
            metodoPago.Saldo -= total;

            // 1. Crear el registro de Compra en el Historial
            var historial = new ListoAPI.Aplication.Core.Entities.HistorialCompra
            {
                IdUsuario = usuarioId,
                Fecha = DateTime.UtcNow,
                Total = total,
                CantidadItems = items.Count
            };
            await _context.Set<ListoAPI.Aplication.Core.Entities.HistorialCompra>().AddAsync(historial);
            
            // 2. Limpiar Carrito
            _carritoService.LimpiarCarrito(usuarioId);

            // 3. Limpiar estado de usuario
            usuario.EstadoSesion = null;
            await _context.SaveChangesAsync();

            return Ok(new ResponseCommonDTO { message = $"Compra finalizada con éxito. Saldo restante: S/ {metodoPago.Saldo}", success = true });
        }
    }
}
