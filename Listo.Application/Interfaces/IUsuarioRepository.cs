using Listo.Application.DTOs;

namespace Listo.Application.Interfaces
{
    public interface IUsuarioRepository
    {

        Task<ResponseCommonDTO> RegisterClientAsync(RegistroClienteDTO pItem);
        Task<List<UsuarioDTO>> getInactivosList(string pSearch = "");
        Task<UsuarioDTO> getById(int pId);
        Task<ResponseCommonDTO> updateItem(UsuarioDTO pItem);
        Task<ResponseCommonDTO> deleteItem(int pId, int idUsuarioInt, string ipOrigen);
        Task<ResponseCommonDTO> saveItem(UsuarioDTO pItem);
        Task<ResponseCommonDTO> RecuperarUsuario(int pId);
        Task<ResponseCommonDTO> ValidarLogin(string correo, string password);

        Task<(List<UsuarioDTO> usuarios, int TotalCount)> GetUsuarioList(int pageNumber, int pageSize, string pSearch, int idRol);
    }
}
