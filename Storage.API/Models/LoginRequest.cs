using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Storage.API.Models
{
    public struct LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [BindRequired]
        public string Password { get; set; }
    }

    public struct LoginResponse
    {
        public string Username { get; set; }
        public object Token { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; }
        public long Expiration { get; set; }
    }
}
