using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppInternalsDotNetSampler.Core.Discoverability;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core.SamplerMethods.Networking
{
    public class TcpClient : ISamplerMethod
    {
        public string MethodName { get { return "Send TCP Traffic"; } }

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
                    "Generates TCP Traffic using a System.Net.Sockets.TcpClient.  " +
                    "Note:  Requires the bundled TcpServer be running.";
            }
        }

        public List<SamplerMethodParameter> Parameters
        {
            get
            {
                return new List<SamplerMethodParameter>
                {
                    new StringParam("address")
                    {
                        DefaultValue = "127.0.0.1"
                    },
                    new IntParam("port")
                    {
                        DefaultValue = "8080"
                    },
                    new StringParam("message")
                    {
                        DefaultValue = "Hello World",
                        Description  = "Text that is sent across the wire to the server."
                    },
                    new IntParam("numberOfTimes")
                    {
                        DefaultValue = "5",
                        Description = "Number of Times the Message is sent to the server."
                    }
                };
            }
        }

        public void WarmUp()
        {
            //No warm up necessary
        }

        public void Execute(IMethodLogger logger, List<SamplerMethodParameter> parameters)
        {
            SendTcpTraffic(
                logger,
                parameters.GetValue<string>(0),
                parameters.GetValue<int>(1),
                parameters.GetValue<string>(2),
                parameters.GetValue<int>(3));
        }

        private void SendTcpTraffic(
            IMethodLogger logger,
            string address, int port,
            string message, int numberOfTimes)
        {
            string serverResponse = "";
            var requestTimes = new List<long>(numberOfTimes);

            for (var i = 0; i < numberOfTimes; i++)
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();

                var requestStopWatch = Stopwatch.StartNew();

                try
                {
                    client.Connect(address, port);
                }
                catch (Exception e)
                {
                    throw new Exception(
                        string.Format(
                        "Failed to connect Tcp Client to [{0}:{1}]: {2} {3} {4}",
                        address, port, e.Message, Environment.NewLine, e.StackTrace));
                }

                try
                {
                    using (var stream = client.GetStream())
                    {
                        using (var sr = new StreamWriter(stream))
                            sr.Write(message);

                        using (var sw = new StreamReader(stream))
                            serverResponse = sw.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Exception talking with server: " +
                        e.Message + Environment.NewLine + e.StackTrace);
                }

                if (!serverResponse.Contains(message))
                    throw new Exception(
                        "Server returned junk.  Expected it to echo message but it did not." +
                        "Expected it to contain [" + message + "]. Response: [" + serverResponse + "]");

                requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
            }

            logger.WriteMethodInfo(
                string.Format("Server Response [{0}].  Request Times: Min [{1} Avg [{2}] Max [{3}]",
                    serverResponse, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

        }
    }
}
