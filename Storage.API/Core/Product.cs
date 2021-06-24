using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Storage.API.Core
{
    public class Product : PaginatedEntities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProducerName { get; set; }

        public DateTime? LastChange { get; set; }

        public string ImagePath { get; set; }

        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public List<StoreProducts> StoreProducts { get; set; }
    }
}
