
using Listo.Application.Interfaces;
using Listo.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListoAPI.API.Controllers
{

    [Route("api/roles")]
    [ApiController]
    public class RolController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IRolRepository _rolRepository;
        public RolController(IConfiguration configuration, IRolRepository rolRepository) {
            _configuration = configuration;
            _rolRepository = rolRepository;

        }

        //obtener lista
        [Route("lista")]
        [HttpGet]
        public async Task<List<RolDTO>> getRol(string pSearch="")
        {
            return await _rolRepository.getList(pSearch);
            
        }

        //guardar
         [HttpPost()]
        public async Task<IActionResult> saveRol(RolDTO user){

            ResponseCommonDTO result = await _rolRepository.saveItem(user);
            if(!result.success){
                return BadRequest(result);
            }
            else{
                return Ok(result);
            }     
        }
        
    
    }
    
}
