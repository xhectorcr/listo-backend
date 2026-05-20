
using ListoAPI.DTO;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface IMetodoPagoRepository
    {
        public Task<List<MetodoPagoDTO>> getList(string pSearch="");
        public Task<MetodoPagoDTO?> getById(int pId);
        public Task<ResponseCommonDTO> saveItem(MetodoPagoDTO pItem);
        public Task<ResponseCommonDTO> updateItem(MetodoPagoDTO pItem);
        public Task<ResponseCommonDTO> deleteItem(int pId);
        
    }
}