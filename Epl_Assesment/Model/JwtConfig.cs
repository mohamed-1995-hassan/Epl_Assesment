using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epl_Assesment.Model
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuser { get; set; }
        public string Audience { get; set; }
    }
}
