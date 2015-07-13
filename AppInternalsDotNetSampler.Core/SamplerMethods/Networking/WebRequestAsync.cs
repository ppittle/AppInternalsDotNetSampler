using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Networking
{
    public class WebRequestAsync : ISamplerMethod
    {
        public string MethodName { get { return "Web Request Async"; } }

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
                    "concurrently using asynchronous (non blocking) api's";
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
            RequestRiverBedHomePageAsync(
                logger,
                parameters.GetValue<int>(0));
        }

        private void RequestRiverBedHomePageAsync(
            IMethodLogger logger, int numberOfRequestsToMake)
        {
            var requestTimes = new ConcurrentBag<long>();
            int htmlLength = 0;

            try
            {
                var tasks = 
                    Enumerable.Range(0, numberOfRequestsToMake)
                        .Select(async x =>
                        {
                            var requestStopWatch = Stopwatch.StartNew();

                            var webClient = new WebClient();
                            using (var stream = await webClient.OpenReadTaskAsync(new Uri("http://www.riverbed.com")))
                            using (var sr = new StreamReader(stream))
                            {
                                var html = await sr.ReadToEndAsync();
                                htmlLength = html.Length;
                            }

                            requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
                        });

                Task.WaitAll(tasks.ToArray());
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