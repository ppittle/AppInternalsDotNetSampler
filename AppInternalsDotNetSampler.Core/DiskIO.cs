using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    /// <summary>
    /// Simulates disk io by writing and reading to/from a temp file.
    /// </summary>
    public class DiskIO
    {
        private readonly IMethodLogger _logger;

        public DiskIO(IMethodLogger logger)
        {
            _logger = logger;
        }

        public void WriteAndReadToDisk(int numberOfCharactersPerRow, long numberOfRows)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin WriteAndReadToDisk.  " +
                             "Chars per Row: [" + numberOfCharactersPerRow + "]" +
                             "Number of Rows: [" + numberOfRows + "].");

            var tempFile = CreateTempFile();

            _logger.WriteMethodInfo("Temp File: [" + tempFile + "]");

            var random = new Random(DateTime.Now.Second);
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            random.Next();

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

                        sw.WriteLine(row.ToString());
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

            _logger.WriteMethodInfo("Wrote file in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");

            try
            {
                using (var fs = File.OpenRead(tempFile))
                using (var sr = new StreamReader(fs))
                {
                    var allText = sr.ReadToEnd();

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

            _logger.WriteMethodEnd("End WriteAndReadToDisk.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        public async Task WriteAndReadToDiskAsync(int numberOfCharactersPerRow, long numberOfRows)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin WriteAndReadToDiskAsync.  " +
                             "Chars per Row: [" + numberOfCharactersPerRow + "]" +
                             "Number of Rows: [" + numberOfRows + "].");

            var tempFile = CreateTempFile();

            _logger.WriteMethodInfo("Temp File: [" + tempFile + "]");

            var random = new Random(DateTime.Now.Second);
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            random.Next();

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

            _logger.WriteMethodInfo("Wrote file in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");

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

            _logger.WriteMethodEnd("End WriteAndReadToDiskAsync.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        private string CreateTempFile()
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
