using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Storage.API.Models;
using Storage.API.Models.Products;

namespace Storage.API.Core.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult> Add(AddProductModel addProductModel);
        Task<ServiceResult> Edit(int id, EditProductModel editProductModel);
        Task<ServiceResult> Delete(int id);
        Task<ServiceResult> AddImage(int id, IFormFile file);
        Task<ServiceResult<Product>> Get(int id);
        Task<ServiceResult<Paginated<Product>>> GetPaginatedList(string like, int limit = 10, int count = 1);
    }
}