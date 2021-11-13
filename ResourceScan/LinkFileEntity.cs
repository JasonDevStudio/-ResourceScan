using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceScan
{
    public class LinkFileEntity
    {
        public string FullName { get; set; }

        public string Name { get; set; }

        public string DirectoryName { get; set; }

        public FileInfo File { get; set; }

        public int State { get; set; }

        public string Extension { get; set; }

        public LinkFileEntity(FileInfo file)
        {
            this.FullName = file.FullName;
            this.Name = file.Name;
            this.DirectoryName = file.DirectoryName;
            this.Extension = file.Extension;
            this.State = 0;
        }
    }
}
