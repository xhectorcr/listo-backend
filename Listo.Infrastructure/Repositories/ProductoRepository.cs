using System.Net.NetworkInformation;
using Listo.Domain.Entities;
using Listo.Application.Interfaces;
using Listo.Infrastructure.Data;
using Listo.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Listo.Infrastructure.Repositories
{
    public class ProductoRepository : IProductoRepository
    {

        protected readonly ConfigContext _context;
        private readonly IConfiguration _configuration;

        public ProductoRepository(ConfigContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public Task<ResponseCommonDTO> saveItem(ProductoDTO pItem)
        {
            try
            {
                var producto = new Producto
                {
                    IdProducto = pItem.IDProducto,
                    Nombre = pItem.Nombre,
                    Activo = pItem.Activo,
                    Descripcion = pItem.Descripcion,
                    Imagen = pItem.Imagen,
                    YoloLabel = pItem.YoloLabel,
                    Stock = pItem.Stock,
                    Precio = pItem.Precio,
                    IdCategoria = pItem.IDCategoria
                };

                _context.PRODUCTO.Add(producto);
                _context.SaveChanges();



                return Task.FromResult(new ResponseCommonDTO { success = true, message = "Producto guardado correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(new ResponseCommonDTO { success = false, message = "Error al crear el producto: " + ex.Message });
            }

        }

        public Task<ResponseCommonDTO> updateItem(ProductoDTO pItem)
        {
            try
            {
                var queryCheck = _context.PRODUCTO.FirstOrDefault(p => p.IdProducto == pItem.IDProducto);

                if (queryCheck == null)
                {
                    return Task.FromResult(new ResponseCommonDTO { success = false, message = "El producto no existe" });
                }


                queryCheck.Nombre = string.IsNullOrEmpty(pItem.Nombre) ? queryCheck.Nombre : pItem.Nombre;
                queryCheck.Descripcion = string.IsNullOrEmpty(pItem.Descripcion) ? queryCheck.Descripcion : pItem.Descripcion;
                queryCheck.Activo = pItem.Activo;
                queryCheck.Precio = pItem.Precio == 0 ? queryCheck.Precio : pItem.Precio;
                queryCheck.Stock = pItem.Stock == -1 ? queryCheck.Stock : pItem.Stock;
                queryCheck.Imagen = string.IsNullOrEmpty(pItem.Imagen) ? queryCheck.Imagen : pItem.Imagen;
                queryCheck.YoloLabel = string.IsNullOrEmpty(pItem.YoloLabel) ? queryCheck.YoloLabel : pItem.YoloLabel;
                queryCheck.IdCategoria = pItem.IDCategoria == 0 ? queryCheck.IdCategoria : pItem.IDCategoria;

                _context.SaveChanges();

                return Task.FromResult(new ResponseCommonDTO
                {
                    success = true,
                    message = "Producto actualizado correctamente"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(new ResponseCommonDTO { success = false, message = "Error al actualizar el producto: " + ex.Message });
            }
        }



        public async Task<(List<ProductoDTO> platos, int TotalCount)> GetProductoList(
  int pageNumber = 1,
  int pageSize = 10,
  string pSearch = "",
  int idCategoria = 0
)
        {
            try
            {
                pSearch = pSearch?.Trim().ToLower();

                var query = from c in _context.PRODUCTO
                            join d in _context.CATEGORIA on c.IdCategoria equals d.IdCategoria
                            where c.Activo == true 
                            select new ProductoDTO
                            {
                                IDProducto = c.IdProducto,
                                Activo = c.Activo,
                                Nombre = c.Nombre,
                                Descripcion = c.Descripcion,
                                YoloLabel = c.YoloLabel,
                                Imagen = c.Imagen,
                                Stock = c.Stock,
                                IDCategoria = c.IdCategoria,
                                Precio = c.Precio,
                                Categoria = d.Nombre
                            };

                if (idCategoria > 0)
                {
                    query = query.Where(x => x.IDCategoria == idCategoria);
                }

                if (!string.IsNullOrEmpty(pSearch))
                {
                    query = query.Where(x => x.Nombre.ToLower().Contains(pSearch) ||
                                             x.Descripcion.ToLower().Contains(" " + pSearch + " "));
                }

                var totalCount = await query.CountAsync();

                var platos = await query
                    .OrderBy(p => p.Nombre)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

            

                return (platos, totalCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetPlatosList: {ex.Message}");
                return (new List<ProductoDTO>(), 0);
            }
        }


        
        public Task<ProductoDTO?> getById(int pId)
        {
            try
            {
                var query = (from p in _context.PRODUCTO
                             join c in _context.CATEGORIA on p.IdCategoria equals c.IdCategoria
                             where p.IdProducto == pId && p.Activo == true
                             select new ProductoDTO
                             {
                                 IDProducto = p.IdProducto,
                                Activo = p.Activo,
                                Nombre = p.Nombre,
                                Descripcion = p.Descripcion,
                                YoloLabel = p.YoloLabel,
                                Imagen = p.Imagen,
                                Stock = p.Stock,
                                IDCategoria = p.IdCategoria,
                                Precio = p.Precio,
                                Categoria = c.Nombre
                             }).FirstOrDefaultAsync();

                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }



        public Task<ResponseCommonDTO> deleteItem(int pId)
        {
            try
            {
                var producto = _context.PRODUCTO.FirstOrDefault(c => c.IdProducto == pId);
                if (producto == null)
                {
                    return Task.FromResult(
                        new ResponseCommonDTO
                        {
                            success = false,
                            message = "El Producto no existe"
                        }
                    );
                }


                producto.Activo = false;
                _context.PRODUCTO.Update(producto);
                _context.SaveChanges();

                return Task.FromResult(
                    new ResponseCommonDTO
                    {
                        success = true,
                        message = "Producto desactivado satisfactoriamente"
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(
                    new ResponseCommonDTO
                    {
                        success = false,
                        message = ex.Message
                    }
                );
            }
        }
        


        public Task<ResponseCommonDTO> verificarStock(int idPlato, int cantidadRequerida)
        {
            throw new NotImplementedException();
        }
    }

}
