using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Storage.API.Core
{
    public class Store : PaginatedEntities
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public StoreType Type { get; set; }

        public DateTime? LastChanged { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public List<StoreProducts> StoreProducts { get; set; }
    }

    public enum StoreType
    {
        Market,
        SuperMarket,
        HyperMarket,
        Other
    }
}
