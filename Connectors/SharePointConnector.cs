using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using FileTransfer.Connectors;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;

namespace FileTransfer.Connectors
{
    internal class SharePointConnector : Connector
    {
        public struct Query
        {
            public string user;
            public string pass;
            public string host;
            public SecureString securePassword;
        }

        private Query ParseQueryString(String queryString)
        {
            string[] qa = queryString.Split(";");

            Query query = new Query();
            query.user = Array.Find(qa, element => element.StartsWith("user:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.pass = Array.Find(qa, element => element.StartsWith("pass:", StringComparison.InvariantCultureIgnoreCase))[5..];
            query.host = Array.Find(qa, element => element.StartsWith("host:", StringComparison.InvariantCultureIgnoreCase))[5..];

            query.securePassword = new SecureString();
            foreach (char c in query.pass)
            {
                query.securePassword.AppendChar(c);
            }

            return query;
        }

        public override bool Push(FileInfo file)
        {
            try
            {
                log.WriteLine("Push: " + file.FullName);
                Query query = ParseQueryString(this.Configuration.ToPath);
                ClientContext client = new ClientContext(query.host);
                client.Credentials = new NetworkCredential(query.user, query.securePassword);
                Web web = client.Web;
                client.Load(web, a => a.ServerRelativeUrl);
                client.ExecuteQuery();
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
                ClientContext client = new ClientContext(query.host);
                client.Credentials = new NetworkCredential(query.user, query.securePassword);
                Web web = client.Web;
                client.Load(web, a => a.ServerRelativeUrl);
                client.ExecuteQuery();
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
                //ClientContext client = new ClientContext(query.host);
                //Web web = client.Web;
                //client.Load(web, a => a.ServerRelativeUrl);
                //client.ExecuteQuery();


                ClientContext client = new ClientContext(query.host);
                client.Credentials = new NetworkCredential(query.user, query.securePassword);


                Web web = client.Web; 
                ListCollection listColl = web.Lists;
                client.Load(listColl); 
                client.ExecuteQuery();

                // Loop through all the list  
                foreach (List list in listColl)
                { 
                    Console.WriteLine("List Name: " + list.Title + "; ID: " + list.Id);
                }
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

