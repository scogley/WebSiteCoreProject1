using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // For SelectListItem

namespace WebSiteCoreProject1.Models
{
    public class EnrollInClassModel
    {
        public List<SelectListItem> ClassNameSelItemList { get; set; }

        public EnrollInClassModel()
        {
            ClassNameSelItemList = new List<SelectListItem>();
        }
    }
}
