﻿using System;
using System.Collections.Generic;

namespace WebSiteCoreProject1
{
    public partial class User
    {
        public User()
        {
            UserClass = new HashSet<UserClass>();
        }

        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public bool UserIsAdmin { get; set; }
        public string UserSalt { get; set; }

        public ICollection<UserClass> UserClass { get; set; }
    }
}
