using System.ComponentModel.DataAnnotations;

namespace Storage.API.Models.Products
{
    public class AddProductModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string ProducerName { get; set; }
    }
}