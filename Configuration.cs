using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer
{
    internal class Configuration
    {
        public string Command { get; set; }
        public string Action { get; set; }
        public string FromPath { get; set; }
        public string ToPath { get; set; }
        public int ReplayCount { get; set; }
        public int Delay { get; set; }
        public int LogLevel { get; set; }
        public bool Interactive { get; set; }
        public string Query { get; set; }
        public bool Recurisve { get; set; }
        public bool Username { get; set; }
        public bool Password{ get; set; }

        public override string ToString()
        {
            return "\n--------------------------------------\n" +
                "Command:\t" + this.Command + "\n" +
                "Action: \t" + this.Action + "\n" +
                "From:   \t" + this.FromPath + "\n" +
                "To:     \t" + this.ToPath + "\n" +
                "Replay: \t" + this.ReplayCount.ToString() + "\n" +
                "Delay:  \t" + this.Delay.ToString() + "\n" +
                "Logging:\t" + this.LogLevel.ToString() + "\n" +
                "Interactive:\t" + this.Interactive.ToString() + "\n" +
                "Query:\t" + this.Query + "\n" +
                "Recursive:\t" + this.Recurisve.ToString() + "\n" +
                "--------------------------------------\n";
        }
    }
}
