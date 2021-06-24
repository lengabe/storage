using System.Threading.Tasks;
using Storage.API.Models;
using Storage.API.Models.Stores;

namespace Storage.API.Core.Interfaces
{
    public interface IStoreService
    {
        Task<ServiceResult> Add(AddStoreModel addStoreModel);
        Task<ServiceResult> Edit(int id, EditStoreModel editStoreModel);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> AddProduct(int id, int productId, AddStoreProductModel addStoreProductModel);
        Task<ServiceResult<Store>> Get(int id);
        Task<ServiceResult<Paginated<Store>>> GetPaginatedList(string like, int limit = 10, int count = 1);

        Task<ServiceResult<Paginated<StoreProducts>>> GetPaginatedProducts(int storeId, string like, int limit = 10,
            int count = 1);
    }
}