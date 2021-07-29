using System;
using System.Collections.Generic;

namespace WebSiteCoreProject1.Models
{
    public partial class UserModel : User
    {
        public UserModel()
        {
            UserClass = new HashSet<UserClass>();
        }
    }
}
