using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace os_project
{
    class Directory_Entry
    {
        // char[] dir_name = new char[11];
        public string dir_name;
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_firstcluster;
        public int dir_fileSize;

        public Directory_Entry(string dir_name, byte dir_attr, byte[] dir_empty, int dir_firstcluster, int dir_fileSize)
        {
            this.dir_name = dir_name;
            this.dir_attr = dir_attr;
            this.dir_firstcluster = dir_firstcluster;
            this.dir_fileSize = dir_fileSize;

        }
    }
}
