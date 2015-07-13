using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    public class TcpClient
    {
        private readonly IMethodLogger _logger;

        public TcpClient(IMethodLogger logger)
        {
            _logger = logger;
        }

        public void SendTcpTraffic(
            string address, int port,
            string message, int numberOfTimes)
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WriteMethodBegin(
                string.Format(
                    "Begin RequestRiverBedHomePage.  " +
                    "Making [{0}] requests to [{1}:{2}] with message [{3}]",
                    numberOfTimes, address, port, message));

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

            _logger.WriteMethodInfo(
                string.Format("Server Response [{0}].  Request Times: Min [{1} Avg [{2}] Max [{3}]",
                    serverResponse, requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

            _logger.WriteMethodEnd("End SendTcpTraffic.  Completed in [" + stopwatch.ElapsedMilliseconds + " ] milliseconds.");
        }
    }
}
