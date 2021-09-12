using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epl_Assesment.Model.Dtos
{
    public class UserInfo
    {
        public UserInfo(string email,string username)
        {
            Email = email;
            UserName = username;
        }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Token { get; set; }
    }
}
