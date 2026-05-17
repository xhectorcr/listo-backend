using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.Aplication.Infrastructure.Data;

using ListoAPI.DTO;
using Microsoft.Extensions.Configuration;

namespace ListoAPI.Aplication.Infrastructure.Repository
{
    public class RolRepository : IRolRepository
    {

        protected readonly ConfigContext _context;
        private readonly IConfiguration _configuration;

        public RolRepository(ConfigContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
        public Task<ResponseCommonDTO> deleteItem(int pId)
        {
            throw new NotImplementedException();
        }

        public Task<List<RolDTO>> getList(string pSearch = "")
        {
            var list = new List<RolDTO>();
            try
            {
                var query = (from c in _context.ROL
                             select new RolDTO
                             {
                                 IDRol = c.IdRol,
                                 Nombre = c.Nombre,
                             })
                             .Where(x => x.Nombre.Contains(pSearch)
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

        public Task<ResponseCommonDTO> saveItem(RolDTO pItem)
        {
            try
            {
                var rol = new Rol();
                rol.IdRol = pItem.IDRol;
                rol.Nombre = pItem.Nombre;
                _context.ROL.Add(rol);
                _context.SaveChanges();

                return Task.FromResult(
                   new ResponseCommonDTO { success = true, message = "Rol Guardado correctamente" }
               );

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(new ResponseCommonDTO { success = false, message = "Error al crear " + ex.Message });
            }
        }

        public Task<ResponseCommonDTO> updateItem(RolDTO pItem)
        {
            throw new NotImplementedException();
        }
    }

}