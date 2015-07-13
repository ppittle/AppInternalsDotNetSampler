using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    /// <summary>
    /// Simulates network activity by request http://www.riverbed.com
    /// </summary>
    public class WebRequestor
    {
        private readonly IMethodLogger _logger;

        public WebRequestor(IMethodLogger logger)
        {
            _logger = logger;
        }

        public void RequestRiverBedHomePage(int numberOfRequestsToMake)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin RequestRiverBedHomePage.  Making [" + numberOfRequestsToMake + "] requests.");

            var requestTimes = new List<long>(numberOfRequestsToMake);
            int htmlLength = 0;

            try
            {
                for (var i = 0; i < numberOfRequestsToMake; i++)
                {
                    var requestStopWatch = Stopwatch.StartNew();

                    var webClient = new WebClient();
                    using (var stream = webClient.OpenRead("http://wwww.riverbed.com"))
                    // ReSharper disable once AssignNullToNotNullAttribute -- will handle in catch clause
                    using (var sr = new StreamReader(stream))
                    {
                        var html = sr.ReadToEnd();
                        htmlLength = html.Length;
                    }

                    requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error getting http://wwww.riverbed.com: " + e.Message + 
                    Environment.NewLine + e.StackTrace);
            }

            _logger.WriteMethodInfo(
                string.Format("Html Length [{0}] chars.  Request Times: Min [{1} Avg [{2}] Max [{3}]",
                    htmlLength, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

            _logger.WriteMethodEnd("End RequestRiverBedHomePage.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }

        public void RequestRiverBedHomePageMultiThreaded(int numberOfRequestsToMake, int numberOfThreads)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin("Begin RequestRiverBedHomePage.  " +
                                     "Making [" + numberOfRequestsToMake + "] requests " +
                                     "with [" + numberOfThreads + "].");

            var requestTimes = new ConcurrentBag<long>();
            int htmlLength = 0;

            try
            {
                Parallel.For(0, numberOfRequestsToMake, new ParallelOptions {MaxDegreeOfParallelism = numberOfThreads},
                    async i =>
                    {
                        var requestStopWatch = Stopwatch.StartNew();

                        var webClient = new WebClient();
                        using (var stream = await webClient.OpenReadTaskAsync(new Uri("http://wwww.riverbed.com")))
                        using (var sr = new StreamReader(stream))
                        {
                            var html = await sr.ReadToEndAsync();
                            htmlLength = html.Length;
                        }

                        requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
                    });
            }
            catch (Exception e)
            {
                throw new Exception("Error getting http://wwww.riverbed.com: " + e.Message +
                    Environment.NewLine + e.StackTrace);
            }

            _logger.WriteMethodInfo(
                string.Format("Html Length [{0}] chars.  Request Times: Min [{1} Avg [{2}] Max [{3}]",
                    htmlLength, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

            _logger.WriteMethodEnd("End RequestRiverBedHomePage.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }
    }
}
