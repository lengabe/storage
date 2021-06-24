using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Core.Interfaces;
using Storage.API.Models.Products;

namespace Storage.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "ADMIN")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(string like, int count = 1, int limit = 10)
        {
            var result = await _productService.GetPaginatedList(like, limit, count);
            if (result.Succeeded)
                return Ok(result.Value);
            return BadRequest(result.Errors);
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> Get(int productId)
        {
            var getProductResult = await _productService.Get(productId);
            if (getProductResult.Succeeded)
                return Ok(getProductResult.Value);
            return BadRequest(getProductResult.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddProductModel model)
        {
            var postProductResult = await _productService.Add(model);
            if (postProductResult.Succeeded)
                return Ok();
            return BadRequest(postProductResult.Errors);
        }

        [HttpPost("{productId:int}/Image")]
        public async Task<IActionResult> PostImage(int productId, IFormFile file)
        {
            var postProductImageResult = await _productService.AddImage(productId, file);
            if (postProductImageResult.Succeeded)
                return Ok();
            return BadRequest(postProductImageResult.Errors);
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> Put(int productId, [FromBody] EditProductModel model)
        {
            var putProductResult = await _productService.Edit(productId, model);
            if (putProductResult.Succeeded)
                return Ok();
            return BadRequest(putProductResult.Errors);
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var deleteProductResult = await _productService.Delete(productId);
            if (deleteProductResult.Succeeded)
                return Ok();
            return BadRequest(deleteProductResult.Errors);
        }
    }
}