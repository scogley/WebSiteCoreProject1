using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem
using System.ComponentModel.DataAnnotations.Schema;

namespace WebSiteCoreProject1.Models
{
    public class EnrollInClassModel
    {
        // This removes the mapping for the db so we can use this model to create an entity to write to db.
        //[NotMapped]
        public List<SelectListItem> ClassNameSelItemList { get; set; }

        public int ClassId { get; set; }

        public EnrollInClassModel()
        {
            ClassNameSelItemList = new List<SelectListItem>();
        }
    }
}
