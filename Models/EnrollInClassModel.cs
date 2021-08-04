using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSiteCoreProject1.Models
{
    public class EnrollInClassModel
    {
        public List<string> ClassNameList { get; set; }
        public bool WillAttend { get; set; }

        public EnrollInClassModel()
        {
            ClassNameList = new List<string>();
        }
    }
}
