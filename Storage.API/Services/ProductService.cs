using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Storage.API.Core;
using Storage.API.Core.Interfaces;
using Storage.API.Models;
using Storage.API.Models.Products;

namespace Storage.API.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly string _storedImagePath;

        public ProductService(IRepository<Product> productRepository, IConfiguration configuration)
        {
            _productRepository = productRepository;
            _storedImagePath = configuration["StoredImagesPath"];
        }

        public async Task<ServiceResult> Add(AddProductModel addProductModel)
        {
            if (string.IsNullOrWhiteSpace(addProductModel.Name) || string.IsNullOrWhiteSpace(addProductModel.ProducerName))
                return new ServiceResult(InvalidData);
            try
            {
                var product = await _productRepository.GetAsync(x =>
                    x.Name == addProductModel.Name && x.ProducerName == addProductModel.ProducerName);
                if (product != null)
                    return new ServiceResult(AlreadyInserted);
                
                await _productRepository.AddAsync(new Product()
                {
                    Name = addProductModel.Name,
                    ProducerName = addProductModel.ProducerName,
                    InsertDate = DateTime.UtcNow
                });
                await _productRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult> Edit(int id, EditProductModel editProductModel)
        {
            if (string.IsNullOrWhiteSpace(editProductModel.Name) && string.IsNullOrWhiteSpace(editProductModel.ProducerName))
                return new ServiceResult(InvalidData);
            
            var product = await _productRepository.FindAsync(id);
            if (product == null)
                return new ServiceResult(InvalidProductId);

            if (!string.IsNullOrWhiteSpace(editProductModel.Name))
                product.Name = editProductModel.Name;
            if (!string.IsNullOrWhiteSpace(editProductModel.ProducerName))
                product.ProducerName = editProductModel.ProducerName;

            var possibleSameProduct = await _productRepository.GetAsync(x =>
                x.Name == editProductModel.Name && x.ProducerName == editProductModel.ProducerName);
            
            if (possibleSameProduct != null)
                return new ServiceResult(AlreadyInserted);
            
            product.LastChange = DateTime.UtcNow;

            try
            {
                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }

        }

        public async Task<ServiceResult> Delete(int id)
        {
            var product = await _productRepository.FindAsync(id);
            if (product == null)
                return new ServiceResult(InvalidProductId);

            try
            {
                _productRepository.Delete(product);
                await _productRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult> AddImage(int id, IFormFile file)
        {
            var product = await _productRepository.FindAsync(id);
            if (product == null)
                return new ServiceResult(InvalidProductId);
            
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !PermittedExtensions.Contains(ext))
                return new ServiceResult(InvalidImageExtension);

            if (file.Length <= 0)
                return new ServiceResult(InvalidImage);
            
            // Upload the file if less than 2 MB
            if (file.Length > 2097152)
                return new ServiceResult(TooLargeImage);

            var randomImageName = Path.GetRandomFileName() + ext;
            
            var imagePath = Path.Combine(_storedImagePath,
                randomImageName);

            try
            {
                await using var stream = File.Create(imagePath);
                await file.CopyToAsync(stream);
                
                product.ImagePath = imagePath;
                _productRepository.Update(product);
                await _productRepository.SaveChangesAsync();
                return ServiceResult.Success;
            }
            catch (Exception e)
            {
                return new ServiceResult(new  ServiceResultError(e.Message));
            }
        }

        public async Task<ServiceResult<Product>> Get(int id)
        {
            var product = await _productRepository.FindAsync(id);
            return product == null ? new ServiceResult<Product>(InvalidProductId) : new ServiceResult<Product>(product);
        }

        public async Task<ServiceResult<Paginated<Product>>> GetPaginatedList(string like, int limit = 10, int count = 1)
        {
            if (limit < 1 || count < 1)
                return new ServiceResult<Paginated<Product>>(InvalidData);
            
            if (string.IsNullOrWhiteSpace(like)) return new ServiceResult<Paginated<Product>>(await _productRepository.ToPaginatedAsync(limit, count));
            var pattern = $"%{like}%";
            return new ServiceResult<Paginated<Product>>(await _productRepository.ToPaginatedAsync(x =>
                    Microsoft.EntityFrameworkCore.EF.Functions.Like(x.Name, pattern)
                    || Microsoft.EntityFrameworkCore.EF.Functions.Like(x.ProducerName, pattern),
                limit,
                count));
        }

        private static readonly ServiceResultError InvalidProductId = new ServiceResultError("INVALID_PRODUCT_ID");
        private static readonly ServiceResultError InvalidData = new ServiceResultError("INVALID_DATA");
        private static readonly ServiceResultError InvalidImageExtension =
            new ServiceResultError("INVALID_IMAGE_EXTENSIONS");

        private static readonly ServiceResultError AlreadyInserted = new ServiceResultError("ALREADY_INSERTED");

        private static readonly ServiceResultError TooLargeImage = new ServiceResultError("TOO_LARGE_IMAGE");
        private static readonly ServiceResultError InvalidImage = new ServiceResultError("INVALID_IMAGE");
        private static readonly HashSet<string> PermittedExtensions = new HashSet<string>() {".jpg", ".jpeg", ".ico"};
    }
}