using System;
using Newtonsoft.Json;

namespace Storage.API.Core
{
    public class StoreProducts : PaginatedEntities
    {
        public int StoreId { get; set; }
        public int ProductId { get; set; }

        public float Price { get; set; }
        public string Barcode { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public Store Store { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public Product Product { get; set; }
    }
}
