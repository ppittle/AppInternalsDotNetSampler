namespace AppInternalsDotNetSampler.Core.Logging
{
    public interface IMethodLogger
    {
        void WriteMethodBegin(string s);
        void WriteMethodInfo(string s);
        void WriteMethodEnd(string s);
        void WriteError(string s);
    }
}
