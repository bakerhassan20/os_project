using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace os_project
{
    class Directory : Directory_Entry
    {
        //public static List<int> Directory_Entry = new List<int>();
        public static List<Directory_Entry> DirOrFiles;
        public Directory parent;


        //constructor
        public Directory(string dir_name, byte dir_attr, byte[] dir_empty, int dir_firstcluster, int dir_fileSize, Directory parent) : base(dir_name, dir_attr, dir_empty, dir_firstcluster, dir_fileSize)
        {

            DirOrFiles = new List<Directory_Entry>();

            this.parent = parent;

            Directory_Entry info = new Directory_Entry(dir_name, dir_attr, dir_empty, dir_firstcluster, dir_fileSize);

            DirOrFiles.Add(info);

        }



        public Directory_Entry GetDirectoryEntry()
        {        //return object that has dir_name, dir_attr, dir_empty, dir_firstcluster , dir_fileSize

            Directory_Entry info = new Directory_Entry(this.dir_name, this.dir_attr, this.dir_empty, this.dir_firstcluster, this.dir_fileSize);

            info.dir_name = this.dir_name;
            info.dir_attr = this.dir_attr;
            info.dir_empty = this.dir_empty;
            info.dir_fileSize = this.dir_fileSize;
            return info;

        }



        private void EmptyMyCluster()
        {
            if (this.dir_firstcluster != 0)
            {

                int cluster = this.dir_firstcluster;
                int next = Mini_FAT.GetClusterPointer(cluster);

                if (cluster == 5 && next == 0)       // if cluster is root(first cluster of root = 5)
                    return;

                do
                {
                    Mini_FAT.SetClusterPointer(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                        next = Mini_FAT.GetClusterPointer(cluster);

                }
                while (cluster != -1);
            }
        }


        public void write_Directory()
        {
            Directory_Entry o = GetDirectoryEntry();

            if (DirOrFiles.Count != 0)
            {
                byte[] dirorfile = new byte[DirOrFiles.Count * 32];
                for (int i = 0; i < DirOrFiles.Count; i++)
                {
                    //convert each iteam in  list to array of size 32
                    //byte[] b = System.Convert.ToByte(DirOrFiles[i]);  //error 

                     byte[] b = Converter.Directory_EntryToBytes(DirOrFiles[i]);

                    for (int j = i * 32, k = 0; k > b.Length; k++, j++)
                        dirorfile[j] = dirorfile[k];

                }

                //split b array to list of arrays of bytes 
                // each arrays of bytes 1024
                List<byte[]> splitByteArray = new List<byte[]>();

                for (int i = 0; i < DirOrFiles.Count * 32; i += 1024)
                {
                    byte[] buffer = new byte[1024];
                    for (int j = 0; j < 1024; j++)
                    {
                        buffer[j] = dirorfile[j];
                    }
                    splitByteArray.Add(buffer);

                }
                int cluster_index;

                if (this.dir_firstcluster != 0)
                {

                    cluster_index = this.dir_firstcluster;

                }
                else
                {

                    cluster_index = Mini_FAT.GetAvailableCluster();

                    this.dir_firstcluster = cluster_index;
                }

                //loop on the list of 1024 bytes array
                int last_claster = -1;
                for (int j = 0; j < splitByteArray.Count; j++)
                {
                    Virtual_Disk.Write_cluster(cluster_index, splitByteArray[j]);
                    Mini_FAT.SetClusterPointer(cluster_index, -1);

                    if (cluster_index != -1)
                    {
                        Virtual_Disk.Write_cluster(cluster_index, splitByteArray[j]);
                        Mini_FAT.SetClusterPointer(cluster_index, -1);
                        if (last_claster != -1)
                            Mini_FAT.SetClusterPointer(last_claster, cluster_index);
                        last_claster = cluster_index;
                        cluster_index = Mini_FAT.GetAvailableCluster();

                    }

                }
            }
            if (DirOrFiles.Count == 0)
            {
                if (this.dir_firstcluster != 0)
                {
                    EmptyMyCluster();
                }
                if (parent != null)
                {
                    this.dir_firstcluster = 0;
                }
            }
            Directory_Entry n = GetDirectoryEntry();
            if (this.parent != null)
            {
                this.parent.UpdataContent(o, n);
                this.parent.write_Directory();


            }
            Mini_FAT.Write_FAT();

        }








        public void Read_Directory()
        {

            if (this.dir_firstcluster != 0)
            {
                DirOrFiles = new List<Directory_Entry>();
                int cluster = this.dir_firstcluster;
                int next = Mini_FAT.GetClusterPointer(cluster);
                if (cluster == 5 && next == 0)
                {

                }
            }

        }

        public int SearchDirectory(string dirOrfilename)
        {

            for (int i = 0; i < DirOrFiles.Count; i++)
            {
                if (DirOrFiles[i].dir_name == dirOrfilename)
                {
                    return i;
                }
            }
            return -1;

        }


        public void UpdataContent(Directory_Entry Old_dir, Directory_Entry New_dir)
        {
            Read_Directory();
            int index = SearchDirectory(Old_dir.dir_name);
            DirOrFiles.RemoveAt(index);
            DirOrFiles.Add(New_dir);
        }



        public void RemoveEntry(Directory_Entry dir)
        {

            Read_Directory();
            int index = SearchDirectory(dir.dir_name);
            DirOrFiles.RemoveAt(index);
            write_Directory();

        }


        public void addEntry(Directory_Entry dir)
        {

            DirOrFiles.Add(dir);
            write_Directory();

        }

        public void DeleteDir(Directory_Entry dir)
        {

            // first delete the content of dir
            EmptyMyCluster();

            // delete dir from parent
            if (this.parent != null)
            {
                this.parent.RemoveEntry(GetDirectoryEntry());
            }

            // if dir is current dir
            /*
            if (Program.current == this.parent)
            {
                if (this.parent != null)
                {
                    Program.current = this.parent;

                    Program.currentPath =
                    Program.current.Read_Directory();
 
                }
            }
            */
            Mini_FAT.Write_FAT();
        }



        public int getMySizeOnDisk()
        {

            int size = 0;

            if (this.dir_firstcluster != 0)
            {
                int cluster = this.dir_firstcluster;
                int next = Mini_FAT.GetClusterPointer(cluster);

                do
                {
                    size++;
                    cluster = next;
                    if (cluster != -1)
                        next = Mini_FAT.GetClusterPointer(cluster);

                }
                while (cluster != -1);

            }
            return size;
        }



        public bool canAddEntry(Directory_Entry dir)
        {

            bool can = false;
            int neededSize = (DirOrFiles.Count - 1) * 32;
            int neddedClusters = neededSize * 1024;

            int Remaining = neededSize % 1024;
            if (Remaining > 0)
                neddedClusters++;

            neddedClusters += dir.dir_fileSize / 1024;

            int Remaining2 = dir.dir_fileSize % 1024;
            if (Remaining > 0)
                neddedClusters++;

            if (getMySizeOnDisk() + Mini_FAT.GetAvailableCluster() >= neddedClusters)
                can = true;

            return can;
        }





    }
}
