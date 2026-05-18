
using ListoAPI.Aplication.Core.Entities;
using ListoAPI.Aplication.Core.Interfaces;
using ListoAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ListoAPI.API.Controllers
{

    [Route("api/metodoPago")]
    [ApiController]
    public class MetodoPagoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IMetodoPagoRepository _metodoPagoRepository;
        public MetodoPagoController(IConfiguration configuration, IMetodoPagoRepository metodoPagoRepository) {
            _configuration = configuration;
            _metodoPagoRepository = metodoPagoRepository;

        }

        [Route("lista")]
        [HttpGet]
        public async Task<List<MetodoPagoDTO>> getLista(string pSearch="")
        {
            return await _metodoPagoRepository.getList(pSearch);
            
        }

        [HttpDelete("{idUsuario}")]
        public async Task<IActionResult> deleteItem(int idUsuario)
        {
            ResponseCommonDTO result = await _metodoPagoRepository.deleteItem(idUsuario);
            
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
        public async Task<ActionResult<MetodoPagoDTO>> GeMetodoByUsuario(int pId)
        {
            try
            {
                var metodoPago = await _metodoPagoRepository.getById(pId);

                if (metodoPago == null)
                {
                    return NotFound(new { mensaje = "No hay metodo de pagos añadidos" });
                }

                return Ok(metodoPago);
            }
            catch (Exception)
            {
                return StatusCode(500, new { mensaje = "Error interno al procesar la solicitud" });
            }
        }


        //guardar
         [HttpPost()]
        public async Task<IActionResult> saveItem(MetodoPagoDTO categoria){

            ResponseCommonDTO result = await _metodoPagoRepository.saveItem(categoria);
            if(!result.success){
                return BadRequest(result);
            }
            else{
                return Ok(result);
            }     
        }
        
    
    }
    
}