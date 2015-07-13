using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Networking
{
    public class WebRequestMultiThreaded : ISamplerMethod
    {
        public string MethodName { get { return "Web Request Multi Threaded"; } }

        public SamplerMethodCategories Category
        {
            get
            {
                return SamplerMethodCategories.Networking;
            }
        }

        public string Description
        {
            get
            {
                return
                    "Generates Http Requests against https://www.riverbed.com " +
                    "using multiple threads (TLP api's).";
            }
        }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new IntParam("numberOfRequestsToMake")
                    {
                        DefaultValue = "10",
                        Description = "Number of times to load the RiverBed Home Page"
                    },
                    new IntParam("numberOfThreads")
                    {
                        DefaultValue = "4"
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
            RequestRiverBedHomePageMultiThreaded(
                logger,
                parameters.GetValue<int>(0),
                parameters.GetValue<int>(1));
        }

        private void RequestRiverBedHomePageMultiThreaded(
            IMethodLogger logger, int numberOfRequestsToMake, int numberOfThreads)
        {
            var requestTimes = new ConcurrentBag<long>();
            int htmlLength = 0;

            try
            {
                Parallel.For(0, numberOfRequestsToMake, 
                    new ParallelOptions { MaxDegreeOfParallelism = numberOfThreads },
                    i =>
                    {
                        var requestStopWatch = Stopwatch.StartNew();

                        var webClient = new WebClient();
                        using (var stream = webClient.OpenRead(new Uri("http://www.riverbed.com")))
                        // ReSharper disable once AssignNullToNotNullAttribute -- will handle in parent catch
                        using (var sr = new StreamReader(stream))
                        {
                            var html = sr.ReadToEnd();
                            htmlLength = html.Length;
                        }

                        requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
                    });
            }
            catch (Exception e)
            {
                throw new Exception("Error getting http://www.riverbed.com: " + e.Message +
                    Environment.NewLine + e.StackTrace);
            }

            logger.WriteMethodInfo(
                string.Format("Html Length [{0:n0}] chars.  Request Times: Min [{1:n0}] Avg [{2:n0}] Max [{3:n0}]",
                    htmlLength, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

            logger.WriteMethodInfo("");
        }
    }
}
