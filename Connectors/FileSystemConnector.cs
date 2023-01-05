using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using FileTransfer.Connectors;

namespace FileTransfer.Connectors
{
    internal class FileSystemConnector : Connector
    {
        public override bool Push(FileInfo file)
        {
            try
            {
                log.WriteLine("Push:" + file.FullName);
                File.Move(this.Configuration.FromPath, this.Configuration.ToPath);
            } 
            catch (Exception ex)
            {
                log.WriteLine("Error: " + ex.Message);
                return false;
            }
            return true;
        }

        public override bool Push(String fromPath, String toPath)
        {
            try
            {
                log.WriteLine("Push: " + fromPath); 
                File.Move(fromPath, toPath);
            }
            catch (Exception ex)
            {
                log.WriteLine("Error: " + ex.Message);
                return false;
            }
            return true;
        }

        public override bool Push(FileInfo file, String toPath)
        {
            try
            {
                log.WriteLine("Push: " + file.FullName); 
                File.Move(file.FullName, toPath + "/" + file.Name);
            }
            catch (Exception ex)
            {
                log.WriteLine("Error: " + ex.Message);
                return false;
            }
            return true;
        }
    }
}
