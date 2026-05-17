using ListoAPI.DTO;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface IRolRepository
    {
        public Task<List<RolDTO>> getList(string pSearch="");
        public Task<ResponseCommonDTO> saveItem(RolDTO pItem);
        public Task<ResponseCommonDTO> updateItem(RolDTO pItem);
        public Task<ResponseCommonDTO> deleteItem(int pId);
        
    }
}