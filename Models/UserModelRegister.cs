using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSiteCoreProject1.Models
{
    public partial class UserModelRegister : User 
    {
        // Inherit from the User class and some additional attributes and non-mapped properties.
        // See validation attributes https://docs.microsoft.com/en-us/aspnet/core/mvc/models/validation?view=aspnetcore-5.0
        
        [Required(ErrorMessage = "Please enter a password")]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match")]
        public override string UserPassword { get; set; }

        [NotMapped] // This lets me add additional properties that are not in the database like password confirmation.
        [Required(ErrorMessage = "Please confirm password")]
        [Compare("UserPassword", ErrorMessage = "Passwords do not match")]
        public string UserPasswordConfirmation { get; set; }
    }
}
