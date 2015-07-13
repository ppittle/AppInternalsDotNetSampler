using AppInternalsDotNetSampler.Core.Console;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Console
{
    public class ConsoleMethodLogger : IMethodLogger
    {
        public void WriteMethodBegin(string s)
        {
            System.Console.WriteLine("--------------------------------------");
            System.Console.WriteLine(s);
            System.Console.WriteLine("--------------------------------------");
        }

        public void WriteMethodInfo(string s)
        {
            System.Console.WriteLine(s);
        }

        public void WriteMethodEnd(string s)
        {
            System.Console.WriteLine(s);
            System.Console.WriteLine(HeaderPrinter.Stars);
        }

        public void WriteError(string s)
        {
            System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
            System.Console.WriteLine(s);
            System.Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
            System.Console.WriteLine();
        }
    }
}
