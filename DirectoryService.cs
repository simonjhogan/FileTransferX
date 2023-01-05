using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;

namespace FileTransfer
{
    internal class DirectoryService
    {
        public bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return true;
            }
            return false;
        }

        public FileInfo[] List(string path, string search = "*", bool recursibe = false)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            FileInfo[] files = directory.GetFiles(search, (recursibe ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly));

            return files;
        }

        public bool Create(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return true;
            }

            return false;
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public bool Delete(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path);
                return true;
            }

            return false;
        }

        public bool Move(string fromPath, string toPath)
        {
            if (Directory.Exists(fromPath))
            {
                if (Directory.Exists(toPath))
                {

                    return true;
                }
            }

            return false;
        }

        public bool Copy(string fromPath, string toPath)
        {
            if (Directory.Exists(fromPath))
            {
                if (Directory.Exists(toPath))
                {

                    return true;
                }
            }

            return false;
        }
    }
}
