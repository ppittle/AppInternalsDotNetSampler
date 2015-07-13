using System;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.TcpServer
{
    public class TcpServerLogger : ITcpServerLogger
    {
        public void WriteLine(string s)
        {
            Console.WriteLine(s);
        }
    }
}
