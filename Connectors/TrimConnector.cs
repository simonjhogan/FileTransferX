using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FileTransfer.Connectors;

namespace FileTransfer.Connectors
{
    internal class TrimConnector : Connector
    {
        public override bool Push(FileInfo file)
        {
            Console.WriteLine("PUSH:\t" + this.Configuration.FromPath + " -> " + this.Configuration.ToPath, true);
            File.Move(this.Configuration.FromPath, this.Configuration.ToPath);
            return true;
        }

        public override bool Push(String fromPath, String toPath)
        {
            Console.WriteLine("PUSH:\t" + fromPath + " -> " + toPath, true);
            File.Move(fromPath, toPath);
            return true;
        }

        public override bool Push(FileInfo file, String toPath)
        {
            Console.WriteLine("PUSH:\t" + file.FullName + " -> " + toPath);
            File.Move(file.FullName, toPath + "/" + file.Name, true);
            return true;
        }
    }
}
