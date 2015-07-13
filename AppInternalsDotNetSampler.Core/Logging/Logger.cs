namespace AppInternalsDotNetSampler.Core.Logging
{
    public interface IMethodLogger
    {
        void WriteMethodDescription(string s);
        void WriteMethodBegin(string s);
        void WriteMethodInfo(string s);
        void WriteMethodEnd(string s);
    }
}
