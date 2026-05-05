using ListoAPI.DTO;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface IUsuarioRepository
    {

        Task<List<UsuarioDTO>> getList(string pSearch = "");
        Task<List<UsuarioDTO>> getInactivosList(string pSearch = "");
        Task<UsuarioDTO> getById(int pId);
        Task<ResponseCommonDTO> updateItem(UsuarioDTO pItem);
        Task<ResponseCommonDTO> deleteItem(int pId, int idUsuarioInt, string ipOrigen);
        Task<ResponseCommonDTO> saveItem(UsuarioDTO pItem);
        Task<ResponseCommonDTO> RecuperarUsuario(int pId);
        Task<ResponseCommonDTO> ValidarLogin(string correo, string password);
    }
}
