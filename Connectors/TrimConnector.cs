using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using FileTransfer.Connectors;
using TRIM.SDK;
//using static System.Net.WebRequestMethods;

namespace FileTransfer.Connectors
{
    internal class TrimConnector : Connector
    {
        public String workgroupServerName = "";
        public String id = "";
        public String container = "";
        public String containerType = "";
        public String recordType = "";

        private void setConnection(string toPath)
        {
            string[] args = toPath.Split("/");
            this.workgroupServerName = args[0];
            this.id = args[1];
            this.container = args[2];
            this.containerType = args[3];
            this.recordType = args[4];
        }

        private bool createRecord(string fromPath)
        {
            // connection(toPath) = WorkgroupServerName/Id/Container/RecordType
            log.WriteLine("Push:" + fromPath + " => " + this.Configuration.ToPath);

            Database database = null;

            try
            {
                database = new Database
                {
                    Id = this.id,
                    WorkgroupServerName = this.workgroupServerName
                };
                database.Connect();

                RecordType recordType = new RecordType(database, this.recordType);
                Record record = new Record(database, recordType);
                RecordType containerType = new RecordType(database, this.containerType);
                Record container = new Record(database, containerType);
                record.SetContainer(container, true);
                InputDocument document = new InputDocument();
                document.SetAsFile(fromPath);
                record.SetDocument(document, false, false, "Created via SDK");
                record.Save();

                // Check file transfer succesfful and delete or move "from" file to a deletion que

                return true;
            }
            catch (Exception e)
            {
                log.WriteLine("Error:" + e.ToString());
                return false;
            }
            finally
            {
                if (database != null)
                {
                    database.Dispose();

                }
            }
        }

        public override bool Push()
        {
            this.setConnection(this.Configuration.ToPath);
            return this.createRecord(this.Configuration.FromPath);
        }

        public override bool Push(String fromPath, String toPath)
        {
            this.setConnection(toPath);
            return this.createRecord(fromPath);
        }

        public override bool Push(FileInfo file, String toPath)
        {
            this.setConnection(toPath);
            return this.createRecord(file.FullName);
        }
    }
}
