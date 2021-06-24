using Storage.API.Core;

namespace Storage.API.Models.Stores
{
    public class EditStoreModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public StoreType? Type { get; set; }
    }
}