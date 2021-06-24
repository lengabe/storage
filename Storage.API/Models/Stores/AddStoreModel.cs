using System.ComponentModel.DataAnnotations;
using Storage.API.Core;

namespace Storage.API.Models.Stores
{
    public class AddStoreModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public StoreType Type { get; set; }
    }
}