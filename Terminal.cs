using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer
{
    internal class Terminal
    {
        public void WriteLine(String text)
        {
            AnsiConsole.Markup("[red]" + DateTime.Now.ToString() + ":[/]\t" + text + "\n");
        }
    }
}
