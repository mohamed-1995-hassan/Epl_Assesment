using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epl_Assesment.Model.Data
{

    public class AppUser : IdentityUser
    {
        public double balance { get; set; }
    }

    
}
