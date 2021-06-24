using System.Collections.Generic;

namespace Storage.API.Models
{
    public class Paginated<T>
    {
        public int Count { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
}