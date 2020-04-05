using Newtonsoft.Json;

namespace Infrastructure.Models
{
    public sealed class Subscription
    {
        [JsonProperty("callbackUrl")]
        public string CallbackUrl { get; set; }
    }
}
