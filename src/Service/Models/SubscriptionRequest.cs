using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Service.Models
{
    public sealed class SubscriptionRequest
    {
        [Required]
        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
    }
}
