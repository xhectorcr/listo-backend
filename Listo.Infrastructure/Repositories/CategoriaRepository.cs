using Listo.Domain.Entities;
using Listo.Application.Interfaces;
using Listo.Infrastructure.Data;

using Listo.Application.DTOs;
using Microsoft.Extensions.Configuration;

namespace Listo.Infrastructure.Repositories
{
    public class CategoriaRepository : ICategoriaRepository
    {

        protected readonly ConfigContext _context;
        private readonly IConfiguration _configuration;

        public CategoriaRepository(ConfigContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }
      
        public Task<List<CategoriaDTO>> getList(string pSearch = "")
        {
            var list = new List<CategoriaDTO>();
            try
            {
                var query = (from c in _context.CATEGORIA where c.Activo == true
                             select new CategoriaDTO
                             {
                                 IDCategoria = c.IdCategoria,
                                 Nombre = c.Nombre,
                                 Activo = c.Activo
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

        public Task<ResponseCommonDTO> saveItem(CategoriaDTO pItem)
        {
            try
            {
                var categoria = new Categoria();
                categoria.IdCategoria = pItem.IDCategoria;
                categoria.Nombre = pItem.Nombre;
                categoria.Activo = pItem.Activo;
                _context.CATEGORIA.Add(categoria);
                _context.SaveChanges();

                return Task.FromResult(
                   new ResponseCommonDTO { success = true, message = "Categoria Guardada correctamente" }
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
        
                var categoria = _context.CATEGORIA.FirstOrDefault(c => c.IdCategoria == pId);
                if (categoria == null)
                {
                    return new ResponseCommonDTO 
                    { 
                        success = false, 
                        message = "La categoría no existe o ya fue eliminada." 
                    };
                }

                _context.CATEGORIA.Remove(categoria); // 👈 aquí está la magia

                _context.SaveChanges();

                return new ResponseCommonDTO 
                { 
                    success = true, 
                    message = "Categoría eliminada correctamente" 
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

        public Task<ResponseCommonDTO> updateItem(CategoriaDTO pItem)
        {
            throw new NotImplementedException();
        }
    }

}
