using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using FileTransfer.Connectors;
using System.Net;
using FluentFTP;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;

namespace FileTransfer.Connectors
{
    internal class SftpConnector : Connector
    {
        public struct Query
        {
            public string user;
            public string pass;
            public string host;
            public string port;
            public string path;
        }

        private Query ParseQueryString(String queryString)
        {
            string[] qa = queryString.Split(";");
            
            Query query = new Query();
            query.user = Array.Find(qa, element => element.StartsWith("user:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.pass = Array.Find(qa, element => element.StartsWith("pass:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.host = Array.Find(qa, element => element.StartsWith("host:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.port = Array.Find(qa, element => element.StartsWith("port:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.path = Array.Find(qa, element => element.StartsWith("path:", StringComparison.InvariantCultureIgnoreCase))[5..];

            return query;
        }

        public override bool Push(FileInfo file)
        {
            try
            {
                log.WriteLine("Push: " + file.FullName);
                Query query = ParseQueryString(this.Configuration.ToPath);
                FtpClient client = new FtpClient(query.host, int.Parse(query.port));
                client.Credentials = new NetworkCredential(query.user, query.pass);
                client.Connect();
                client.UploadFile(file.FullName, file.Name);
                client.Disconnect();
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

                FileInfo file = new FileInfo(fromPath);
                Query query = ParseQueryString(toPath);
                FtpClient client = new FtpClient(query.host, int.Parse(query.port));
                client.Credentials = new NetworkCredential(query.user, query.pass);
                client.Connect();
                client.UploadFile(file.FullName, file.Name);
                client.Disconnect();
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
                Query query = ParseQueryString(toPath);
                FtpClient client = new FtpClient(query.host, int.Parse(query.port));
                client.Credentials = new NetworkCredential(query.user, query.pass);
                client.Connect();
                client.UploadFile(file.FullName, file.Name);
                client.Disconnect();
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
