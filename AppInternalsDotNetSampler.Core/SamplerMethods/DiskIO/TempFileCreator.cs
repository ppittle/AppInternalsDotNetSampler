using System;
using System.IO;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.DiskIO
{
    public class TempFileCreator
    {
        public string CreateTempFile()
        {
            try
            {
                return Path.GetTempFileName();
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Exception creating temp file via Path.GetTempFileName().  " +
                    "Try running with Administrator permissions: " + e.Message +
                    Environment.NewLine + e.StackTrace, e);
            }
        }
    }
}
