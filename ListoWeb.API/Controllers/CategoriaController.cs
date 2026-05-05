
using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListoAPI.API.Controllers
{

    [Route("api/categoria")]
    [ApiController]
    public class CategoriaController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaController(IConfiguration configuration, ICategoriaRepository categoriaRepository) {
            _configuration = configuration;
            _categoriaRepository = categoriaRepository;

        }

        [Route("lista")]
        [HttpGet]
        public async Task<List<CategoriaDTO>> getLista(string pSearch="")
        {
            return await _categoriaRepository.getList(pSearch);
            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deleteCategoria(int id)
        {
            ResponseCommonDTO result = await _categoriaRepository.deleteItem(id);
            
            if (!result.success)
            {
          
                return BadRequest(result);
            }
            else
            {
           
                return Ok(result);
            }
        }

        //guardar
         [HttpPost()]
        public async Task<IActionResult> saveCategoria(CategoriaDTO categoria){

            ResponseCommonDTO result = await _categoriaRepository.saveItem(categoria);
            if(!result.success){
                return BadRequest(result);
            }
            else{
                return Ok(result);
            }     
        }
        
    
    }
    
}