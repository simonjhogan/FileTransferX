using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Configuration;
using TRIM.SDK;

namespace FileTransfer
{
    internal class TaskLocker
    {
        public String LockFilePath { get; }

        public TaskLocker(String lockFilePath, string prefix)
        {
            this.LockFilePath = lockFilePath + prefix + ".lock";   
        }

        public bool IsLocked()
        {
            return File.Exists(this.LockFilePath);
        }

        public bool Lock()
        {
            if (File.Exists(this.LockFilePath))
            {
                return false;
            }

            File.WriteAllText(this.LockFilePath, DateTime.Today.ToString());
            return true;
        }

        public bool Unlock()
        {
            if (File.Exists(this.LockFilePath))
            {
                File.Delete(this.LockFilePath);
                return true;
            }

            return false;
        }
    }
}
