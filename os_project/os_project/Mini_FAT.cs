using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace os_project
{
    class Mini_FAT
    {
        static string  paths;

        public Mini_FAT(string path)
        {
            paths = path;

        }

        static int[] FAT = new int[1024];

        private static int counter;

        public static void prepare_FAT()
        {

            for (int i = 0; i < 1024; i++)
            {
                if (i == 0 || i == 4)
                {
                    FAT[i] = -1;     // full
                }
                else if (i == 1 || i == 2 || i == 3)
                {
                    FAT[i] = 1 + i;    //linked list each number refer to the next index
                }
                else
                {
                    FAT[i] = 0;     //empty
                }
            }
        }


        int GetEmptyCluster()
        {
            for (int i = 5; i < 1024; i++)
            {
                if (FAT[i] == 0)
                {
                    return i;
                }
            }
            return -1;
        }
        public static int GetAvailableCluster()
        {
            counter = 0;
            for (int i = 5; i < 1024; i++)
            {

                if (FAT[i] == 0)
                {
                    counter++;
                }
            }
            return counter;
        }


        public static void SetClusterPointer(int cluster_index, int pointer)
        {
            FAT[cluster_index] = pointer;
        }

        public static int GetClusterPointer(int cluster_index)
        {
            return FAT[cluster_index];

        }


        void SetFatArray(int[] arr)
        {
            for (int i = 0; i < 1024; i++)
            {
                FAT[i] = arr[i];
            }
        }

        public static void Write_FAT()
        {
            //string path = @"C:\Users\aboba\Desktop\project_1\project_1\bin\Debug\file_system.txt";

            FileStream Virtual_disk_text = new FileStream(paths, FileMode.Open, FileAccess.ReadWrite);
            Virtual_disk_text.Seek(1024, SeekOrigin.Begin);
            Byte[] bt = new Byte[1024 * 4];
            Buffer.BlockCopy(FAT, 0, bt, 0, bt.Length);
            Virtual_disk_text.Write(bt, 0, bt.Length);
            Virtual_disk_text.Close();
        }


        public static int[] Read_FAT()

        {

            string path = @"C:\Users\aboba\Desktop\project_1\project_1\bin\Debug\file_system.txt";
            // FileInfo Virtual_disk_txt = new FileInfo(path);
            FileStream Virtual_disk_text = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            // StreamWriter rd = new StreamWriter(Virtual_disk_text);
            Virtual_disk_text.Seek(1024, SeekOrigin.Begin);
            Byte[] bt = new Byte[1024 * 4];
            Virtual_disk_text.Read(bt, 0, bt.Length);
            Buffer.BlockCopy(bt, 0, FAT, 0, bt.Length);

            Virtual_disk_text.Close();
            return FAT;
        }

    }
}
