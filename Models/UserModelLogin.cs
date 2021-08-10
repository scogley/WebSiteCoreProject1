using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSiteCoreProject1.Models
{
    public partial class UserModelLogin
    {
        public string UserEmail { get; set; }
        public virtual string UserPassword { get; set; }
    }
}
