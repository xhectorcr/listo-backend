using ListoAPI.Aplication.Core.Entities;
using ListoAPI.DTO;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface ICategoriaRepository
    {
        public Task<List<CategoriaDTO>> getList(string pSearch="");
        public Task<ResponseCommonDTO> saveItem(CategoriaDTO pItem);
        public Task<ResponseCommonDTO> updateItem(CategoriaDTO pItem);
        public Task<ResponseCommonDTO> deleteItem(int pId);
        
    }
}