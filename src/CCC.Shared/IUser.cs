using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CCC.Shared
{
    public interface IUser
    {
        [JsonPropertyName("uid")]
        public string Uid { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("displayName")]
        public string Name { get; set; }
        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; }
    }
}
