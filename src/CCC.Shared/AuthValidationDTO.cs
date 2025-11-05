using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCC.Shared
{
    public class AuthValidationDTO
    {
        public UsuarioDTO? User { get; set; }
        public string? IdToken { get; set; }
    }
}
