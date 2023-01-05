using System;
using System.Collections.Generic;
using System.Text;

namespace FileTransfer
{
    internal class Logging
    {
        public void WriteLine(String text, int level = 1)
        {
            if (level > 0)
            {
                Console.WriteLine(DateTime.Now.ToString() + ": " + text);
            }
        }
    }
}
