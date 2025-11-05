using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCC.Shared
{
    public class ControledException : Exception
    {
        public ControledException(string message) : base(message) { }
        public ControledException(string message, Exception inner) : base(message, inner) { }
    }
}
