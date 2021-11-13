using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;

namespace ResourceScan
{    public class ProjectEntity
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FullName { get; set; } 

        public bool IsChecked { get; set; }

        public string RelativePath { get; set; }

        public Project CurrProject { get; set; }
    }
}
