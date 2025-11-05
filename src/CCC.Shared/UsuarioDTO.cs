using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CCC.Shared
{
    public class UsuarioDTO : IUser   
    {
        [JsonPropertyName("uid")]
        public required string Uid { get; set; }
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("displayName")]
        public required string Name { get; set; }
        [JsonPropertyName("photoUrl")]
        public string? PhotoUrl { get; set; } = null;
        [JsonPropertyName("providerId")]

        public List<string>? Providers { get; set; } = new List<string>();
    }
}
