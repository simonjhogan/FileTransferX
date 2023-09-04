using System;
using System.Text;
using System.IO;

namespace FileTransfer.Connectors
{
    internal class Connector
    {
        public Configuration Configuration { get; set; }
        public Logging log { get; set; }

        public Connector() { }

        public Connector(Configuration configuration)
        {
            this.Configuration = configuration;
        }

        public virtual bool Push()
        {
            return true;
        }

        public virtual bool Push(string fromPath, string toPath)
        {
            return true;
        }
        public virtual bool Push(FileInfo file, string toPath)
        {
            return true;
        }
    }
}
