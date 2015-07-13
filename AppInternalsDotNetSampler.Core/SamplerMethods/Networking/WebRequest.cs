using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Networking
{
    public class WebRequest : ISamplerMethod
    {
        public string MethodName { get { return "Web Request Single Threaded"; } }

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
                    "sequentially using synchronous (blocking) api's";
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
            RequestRiverBedHomePage(
                logger,
                parameters.GetValue<int>(0));
        }

        private void RequestRiverBedHomePage(IMethodLogger logger, int numberOfRequestsToMake)
        {
            var requestTimes = new List<long>(numberOfRequestsToMake);
            int htmlLength = 0;

            try
            {
                for (var i = 0; i < numberOfRequestsToMake; i++)
                {
                    var requestStopWatch = Stopwatch.StartNew();

                    var webClient = new WebClient();
                    using (var stream = webClient.OpenRead("http://www.riverbed.com"))
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
                throw new Exception("Error getting http://www.riverbed.com: " + e.Message +
                    Environment.NewLine + e.StackTrace);
            }

            logger.WriteMethodInfo(
                string.Format("Html Length [{0:n0}] chars.  Request Times: Min [{1:n0}] Avg [{2:n0}] Max [{3:n0}]",
                    htmlLength, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));
        }
    }
}
