using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Storage.API.Core;
using Storage.API.Core.Interfaces;
using Storage.API.Models;
using Storage.API.Models.Stores;

namespace Storage.API.Services
{
    public class StoreService : IStoreService
    {
        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<StoreProducts> _storeProductsRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;
        
        
        public StoreService(IRepository<Store> storeRepository, IRepository<StoreProducts> storeProductsRepository, IProductService productService, IRepository<Product> productRepository)
        {
            _storeRepository = storeRepository;
            _storeProductsRepository = storeProductsRepository;
            _productService = productService;
            _productRepository = productRepository;
        }
        
        public async Task<ServiceResult> Add(AddStoreModel addStoreModel)
        {
            if (string.IsNullOrEmpty(addStoreModel.Name) || string.IsNullOrEmpty(addStoreModel.Address))
                return new ServiceResult(InvalidData);
            try
            {
                var store = await _storeRepository.GetAsync(x =>
                    x.Name == addStoreModel.Name && x.Address == addStoreModel.Address && x.Type == addStoreModel.Type);
                if (store != null)
                    return new ServiceResult(AlreadyInserted);
                
                await _storeRepository.AddAsync(new Store()
                {
                    Name = addStoreModel.Name,
                    Address = addStoreModel.Address,
                    Type = addStoreModel.Type,
                    InsertDate = DateTime.UtcNow,
                });
                await _storeRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult> Edit(int id, EditStoreModel editStoreModel)
        {
            if (string.IsNullOrEmpty(editStoreModel.Name) && string.IsNullOrEmpty(editStoreModel.Address) && editStoreModel.Type == null)
                return new ServiceResult(InvalidData);
            
            var store = await _storeRepository.FindAsync(id);
            if (store == null)
                return new ServiceResult(InvalidStoreId);

            if (!string.IsNullOrEmpty(editStoreModel.Name))
                store.Name = editStoreModel.Name;
            if (!string.IsNullOrEmpty(editStoreModel.Address))
                store.Address = editStoreModel.Address;
            if (editStoreModel.Type != null)
                store.Type = (StoreType) editStoreModel.Type;

            var possibleSameStore = await _storeRepository.GetAsync(x =>
                x.Name == store.Name && x.Address == store.Address && x.Type == store.Type);
            
            if (possibleSameStore != null)
                return new ServiceResult(AlreadyInserted);
            
            store.LastChanged = DateTime.UtcNow;

            try
            {
                _storeRepository.Update(store);
                await _storeRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult> Delete(int id)
        {
            var store = await _storeRepository.FindAsync(id);
            if (store == null)
                return new ServiceResult(InvalidStoreId);

            try
            {
                _storeRepository.Delete(store);
                await _storeRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult> AddProduct(int id, int productId, AddStoreProductModel addStoreProductModel)
        {
            if (addStoreProductModel.Price <= 0 || !addStoreProductModel.Barcode.Any() || !addStoreProductModel.Barcode.All(char.IsNumber))
                return new ServiceResult(InvalidData);
            
            var store = await _storeRepository.FindAsync(id);
            if (store == null)
                return new ServiceResult(InvalidStoreId);

            var getProductResult = await _productService.Get(productId);
            if (!getProductResult.Succeeded)
                return getProductResult;

            var product = getProductResult.Value;
            var storeProducts =
                await _storeProductsRepository.GetAsync(x => x.ProductId == product.Id && x.StoreId == store.Id);

            if (storeProducts != null)
                return new ServiceResult(ProductIsAlreadyInStore);

            try
            {
                await _storeProductsRepository.AddAsync(new StoreProducts()
                {
                    StoreId = store.Id,
                    ProductId = product.Id,
                    Price = addStoreProductModel.Price,
                    Barcode = addStoreProductModel.Barcode,
                    InsertDate = DateTime.UtcNow
                });
                await _storeProductsRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult<Store>> Get(int id)
        {
            var store = await _storeRepository.GetAsync(x => x.Id == id, y => y.StoreProducts );
            return store == null ? new ServiceResult<Store>(InvalidStoreId) : new ServiceResult<Store>(store);
        }

        public async Task<ServiceResult<Paginated<Store>>> GetPaginatedList(string like, int limit = 10, int count = 1)
        {
            if (limit < 1 || count < 1)
                return new ServiceResult<Paginated<Store>>(InvalidData);
            
            if (string.IsNullOrWhiteSpace(like)) return new ServiceResult<Paginated<Store>>(await _storeRepository.ToPaginatedAsync(limit, count));
            var pattern = $"%{like}%";
            return new ServiceResult<Paginated<Store>>(await _storeRepository.ToPaginatedAsync(x =>
                    Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Name, pattern)
                    || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Address, pattern)
                    || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Type.ToString(), pattern),
                limit,
                count));
        }

        public async Task<ServiceResult<Paginated<StoreProducts>>> GetPaginatedProducts(int storeId, string like, int limit = 10, int count = 1)
        {
            if (limit < 1 || count < 1)
                return new ServiceResult<Paginated<StoreProducts>>(InvalidData);

            if (string.IsNullOrWhiteSpace(like))
                return new ServiceResult<Paginated<StoreProducts>>(await _storeProductsRepository.ToPaginatedAsync(x => x.StoreId == storeId, limit, count, x => x.Product));
            else
            {
                var pattern = $"%{like}%";
                return new ServiceResult<Paginated<StoreProducts>>(await _storeProductsRepository.ToPaginatedAsync(x =>
                        x.StoreId == storeId &&
                        (Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Barcode, pattern)
                         || Microsoft.EntityFrameworkCore.EF.Functions.Like(
                             x.Price.ToString(),
                             pattern)
                         || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Product.Name, pattern)
                         || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Product.ProducerName, pattern)), limit,
                    count, x => x.Product));
            }
        }
        
        private static readonly ServiceResultError InvalidStoreId = new ServiceResultError("INVALID_STORE_ID");
        private static readonly ServiceResultError InvalidData = new ServiceResultError("INVALID_DATA");
        private static readonly ServiceResultError ProductIsAlreadyInStore = new ServiceResultError("PRODUCT_IS_ALREADY_IN_STORE");
        private static readonly ServiceResultError AlreadyInserted = new ServiceResultError("ALREADY_INSERTED");
    }
}