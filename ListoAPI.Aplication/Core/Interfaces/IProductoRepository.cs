
using ListoAPI.DTO;

namespace ListoAPI.Aplication.Core.Interfaces
{
    public interface IProductoRepository
    {

        public Task<ProductoDTO?> getById(int pId);
        public Task<ResponseCommonDTO> saveItem(ProductoDTO pItem);
        public Task<ResponseCommonDTO> updateItem(ProductoDTO pItem);
        public Task<ResponseCommonDTO> deleteItem(int pId);
        public Task<ResponseCommonDTO> verificarStock(int idPlato, int cantidadRequerida);
        Task<(List<ProductoDTO> platos, int TotalCount)> GetProductoList(
            int pageNumber,
            int pageSize,
            string pSearch,
            int idCategoria
        );

    }
}
