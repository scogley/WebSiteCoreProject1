using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSiteCoreProject1.Models
{
    public partial class UserModel : User
    {  
        // See validation attributes https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-5.0
        [Required(ErrorMessage = "Please enter a password")]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match")]
        public override string UserPassword { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match")]
        public string UserPassword2 { get; set; }
    }
}
