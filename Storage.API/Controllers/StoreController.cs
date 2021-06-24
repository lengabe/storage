using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.API.Core.Interfaces;
using Storage.API.Models.Stores;

namespace Storage.API.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(string like, int count = 1, int limit = 10)
        {
            var result = await _storeService.GetPaginatedList(like, limit, count);
            if (result.Succeeded)
                return Ok(result.Value);
            return BadRequest(result.Errors);
        }

        [HttpGet("{storeId:int}")]
        public async Task<IActionResult> Get(int storeId)
        {
            var getResult = await _storeService.Get(storeId);
            if (getResult.Succeeded)
                return Ok(getResult.Value);
            return BadRequest(getResult.Errors);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddStoreModel model)
        {
            var postResult = await _storeService.Add(model);
            if (postResult.Succeeded)
                return Ok();
            return BadRequest(postResult.Errors);
        }
        
        [HttpPut("{storeId:int}")]
        public async Task<IActionResult> Put(int storeId, [FromBody] EditStoreModel model)
        {
            var putResult = await _storeService.Edit(storeId, model);
            if (putResult.Succeeded)
                return Ok();
            return BadRequest(putResult.Errors);
        }

        [HttpDelete("{storeId:int}")]
        public async Task<IActionResult> Delete(int storeId)
        {
            var deleteResult = await _storeService.Delete(storeId);
            if (deleteResult.Succeeded)
                return Ok();
            return BadRequest(deleteResult.Errors);
        }

        [HttpGet("{storeId:int}/products")]
        public async Task<IActionResult> GetProducts(int storeId, string like, int count = 1, int limit = 10)
        {
            var getProductResult = await _storeService.GetPaginatedProducts(storeId, like, limit, count);
            if (getProductResult.Succeeded)
                return Ok(getProductResult.Value);
            return BadRequest(getProductResult.Errors);
        }
        
        [HttpPost("{storeId:int}/products/{productId:int}")]
        public async Task<IActionResult> PostProduct(int storeId, int productId, [FromBody] AddStoreProductModel model)
        {
            var postProductImageResult = await _storeService.AddProduct(storeId, productId, model);
            if (postProductImageResult.Succeeded)
                return Ok();
            return BadRequest(postProductImageResult);
        }
    }
}