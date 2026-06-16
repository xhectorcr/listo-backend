using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.Aplication.Infrastructure.Data;

using ListoAPI.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ListoAPI.Aplication.Infrastructure.Repository
{
    public class MetodoPagoRepository : IMetodoPagoRepository
    {

        protected readonly ConfigContext _context;
        private readonly IConfiguration _configuration;

        public MetodoPagoRepository(ConfigContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        public Task<List<MetodoPagoDTO>> getList(string pSearch = "")
        {
            var list = new List<MetodoPagoDTO>();
            try
            {
                var query = (from c in _context.METODOPAGO
                             join d in _context.USUARIO on c.IdUsuario equals d.IdUsuario
                             where c.Estado == true
                             select new MetodoPagoDTO
                             {
                                 IdMetodoPago = c.IdMetodoPago,
                                 IdUsuario = c.IdUsuario,
                                 Usuario = d.Nombre,
                                 Saldo = c.Saldo,
                                 MarcaTarjeta = c.MarcaTarjeta,
                                 TokenSimulado = c.TokenSimulado,
                                 UltimosDigitos = c.UltimosDigitos,
                                 Estado = c.Estado

                             })
                             .Where(x => x.TokenSimulado.Contains(pSearch)
                                         )
                             .ToList();

                return Task.FromResult(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(list);
            }
        }

        public Task<ResponseCommonDTO> saveItem(MetodoPagoDTO pItem)
        {
            try
            {
                var metodo = _context.METODOPAGO.FirstOrDefault(c => c.IdUsuario == pItem.IdUsuario);
                if (metodo != null)
                {
                    return Task.FromResult(
                    new ResponseCommonDTO { success = false, message = "Ya existe un metododepago asignado para este usuario" }
                );

                }
                var metodoPago = new MetodoPago();

                metodoPago.IdMetodoPago = pItem.IdMetodoPago;
                metodoPago.IdUsuario = pItem.IdUsuario;
                metodoPago.TokenSimulado = pItem.TokenSimulado;
                metodoPago.UltimosDigitos = pItem.UltimosDigitos;
                metodoPago.Saldo = pItem.Saldo;
                metodoPago.MarcaTarjeta = pItem.MarcaTarjeta;
                metodoPago.Estado = pItem.Estado;
                _context.METODOPAGO.Add(metodoPago);
                _context.SaveChanges();

                return Task.FromResult(
                   new ResponseCommonDTO { success = true, message = "Metodo Guardado correctamente" }
               );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(new ResponseCommonDTO { success = false, message = "Error al crear " + ex.Message });
            }
        }

        public async Task<ResponseCommonDTO> deleteItem(int pId)
        {
            try
            {

                var metodo = _context.METODOPAGO.FirstOrDefault(c => c.IdUsuario == pId);
                if (metodo == null)
                {
                    return new ResponseCommonDTO
                    {
                        success = false,
                        message = "El metododepago no existe o ya fue eliminada."
                    };
                }

                _context.METODOPAGO.Remove(metodo); // 👈 aquí está la magia

                _context.SaveChanges();

                return new ResponseCommonDTO
                {
                    success = true,
                    message = "Metodo de pago eliminada correctamente"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new ResponseCommonDTO
                {
                    success = false,
                    message = "Error al eliminar la categoría: " + ex.Message
                };
            }
        }


        public Task<ResponseCommonDTO> updateItem(MetodoPagoDTO pItem)
        {
            throw new NotImplementedException();
        }

        public Task<MetodoPagoDTO?> getById(int pId)
        {
            try
            {
                var query = (from c in _context.METODOPAGO
                             join d in _context.USUARIO on c.IdUsuario equals d.IdUsuario
                             where c.Estado == true && c.IdUsuario == pId
                             select new MetodoPagoDTO
                             {
                                 IdMetodoPago = c.IdMetodoPago,
                                 IdUsuario = c.IdUsuario,
                                 Usuario = d.Nombre,
                                 Saldo = c.Saldo,
                                 TokenSimulado = c.TokenSimulado,
                                 MarcaTarjeta = c.MarcaTarjeta,
                                 UltimosDigitos = c.UltimosDigitos,
                                 Estado = c.Estado
                             }).FirstOrDefaultAsync();

                return query;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }

}