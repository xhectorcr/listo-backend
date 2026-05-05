using System.Collections.Concurrent;
using ListoAPI.DTO;
using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;

namespace ListoWeb.API.Services
{
    public class CarritoService
    {
        // Almacena el carrito en memoria usando UsuarioId como key
        private readonly ConcurrentDictionary<int, List<CarritoItemDTO>> _carritos = new();

        public void AgregarProducto(int usuarioId, Producto producto)
        {
            var carrito = _carritos.GetOrAdd(usuarioId, new List<CarritoItemDTO>());

            var itemExistente = carrito.FirstOrDefault(i => i.IdProducto == producto.IdProducto);
            if (itemExistente != null)
            {
                itemExistente.Cantidad++;
            }
            else
            {
                carrito.Add(new CarritoItemDTO
                {
                    IdProducto = producto.IdProducto,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio,
                    Cantidad = 1
                });
            }
        }

        public List<CarritoItemDTO> ObtenerCarrito(int usuarioId)
        {
            if (_carritos.TryGetValue(usuarioId, out var carrito))
            {
                return carrito;
            }
            return new List<CarritoItemDTO>();
        }

        public void LimpiarCarrito(int usuarioId)
        {
            _carritos.TryRemove(usuarioId, out _);
        }
    }
}
