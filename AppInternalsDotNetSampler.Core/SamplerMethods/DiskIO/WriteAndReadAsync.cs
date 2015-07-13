using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.DiskIO
{
    public class WriteAndReadAsync : ISamplerMethod
    {
        public string MethodName
        {
            get { return "Write and Read to Disk Async"; }
        }

        public SamplerMethodCategories Category
        {
            get { return SamplerMethodCategories.DiskIO; }
        }

        public string Description
        {
            get
            {
                return "Writes random data to disk and then reads it" +
                       "back using asynchronous (non-blocking) IO api's";
            }
        }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new IntParam("numberOfCharactersPerRow")
                    {
                        DefaultValue = "25"
                    },
                    new LongParam("numberOfRows")
                    {
                        DefaultValue = "10000"
                    }
                };
            }
        }

        public void WarmUp()
        {
            //nothing to do here
        }

        public void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters)
        {
            WriteAndReadToDiskAsync(
                logger,
                parameters.GetValue<int>(0),
                parameters.GetValue<long>(1))
            .Wait();
        }

        private async Task WriteAndReadToDiskAsync(
            IMethodLogger logger, int numberOfCharactersPerRow, long numberOfRows)
        {
            var tempFile = new TempFileCreator().CreateTempFile();

            logger.WriteMethodInfo("Temp File: [" + tempFile + "]");

            var random = new Random(DateTime.Now.Second);
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            random.Next();

            var stopwatch = Stopwatch.StartNew();

            try
            {
                using (var fs = File.OpenWrite(tempFile))
                using (var sw = new StreamWriter(fs))
                {
                    for (long r = 0; r < numberOfRows; r++)
                    {
                        var row = new StringBuilder(numberOfCharactersPerRow);

                        for (var i = 0; i < numberOfCharactersPerRow; i++)
                        {
                            row.Append(char.ConvertFromUtf32(random.Next(48, 122)));
                        }

                        await sw.WriteLineAsync(row.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Exception writing to temp file [" + tempFile + "]. " +
                    "Try running with Administrator permissions: " + e.Message +
                    Environment.NewLine + e.StackTrace, e);
            }

            logger.WriteMethodInfo("Wrote file in [" + stopwatch.ElapsedMilliseconds + "] milliseconds.");

            try
            {
                using (var fs = File.OpenRead(tempFile))
                using (var sr = new StreamReader(fs))
                {
                    var allText = await sr.ReadToEndAsync();

                    if (string.IsNullOrEmpty(allText))
                        throw new Exception(
                            "This exception is here to ensure the compiler does not 'optimize' away" +
                            "the sr.ReadToEnd() call. This exception should never be hit.");
                }
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Exception reading from temp file [" + tempFile + "]. " +
                    "Try running with Administrator permissions: " + e.Message +
                    Environment.NewLine + e.StackTrace, e);
            }

            try
            {
                File.Delete(tempFile);
            }
            catch (Exception e)
            {
                throw new Exception(
                    "Exception deleting temp file [" + tempFile + "]. " +
                    "Try running with Administrator permissions: " + e.Message +
                    Environment.NewLine + e.StackTrace, e);
            }
        }
    }
}
