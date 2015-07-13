using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
                        Description  = "Text that is sent across the wire to the server. "
                    },
                    new IntParam("messageInflationCount")
                    {
                        DefaultValue = "1",
                        Description = "Number of times to copy the message before sending to server. " +
                                      "Enables you to easily send a very large payload.  For example," +
                                      "if set to 2 message will be 'Hello WorldHello World'"
                    },
                    new IntParam("numberOfTimesToSendMessage")
                    {
                        DefaultValue = "5",
                        Description = "Number of Times the Message is sent to the server."
                    },
                    new IntParam("simulatedPacketDelayInMilliseconds")
                    {
                        DefaultValue = "0",
                        Description = "Time in Milliseconds to artificially delay reading packets (default is 1024 byte chunks)." +
                                      "You can use this to simulate latency.  Note: Delay is only added if message is " +
                                      "larger than read buffer (1024 bytes).  Use messageInflationCount to easily increase" +
                                      "the size of the message."
                    },
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
                parameters.GetValue<int>(3),
                parameters.GetValue<int>(4),
                parameters.GetValue<int>(5));
        }

        private void SendTcpTraffic(
            IMethodLogger logger,
            string address, int port,
            string message, int messageInflationCount,
            int numberOfTimes, int simulatedPacketDelayInMilliseconds)
        {
            string serverResponse = "";
            var requestTimes = new List<long>(numberOfTimes);

            var sb = new StringBuilder(message, message.Length * messageInflationCount);

            for (var i = 1; i < messageInflationCount; i++)
            {
                sb.Append(message);
            }
            message = sb.ToString();

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
                    var data = Encoding.UTF8.GetBytes(message);

                    using (var stream = client.GetStream())
                    {
                        stream.Write(data, 0, data.Length);

                        stream.Flush();

                        var allData = new List<byte>();
                        var receiveBuffer = new byte[1024];

                        while (true)
                        {
                            var chunkLength = stream.Read(receiveBuffer, 0, receiveBuffer.Length);

                            if (chunkLength < receiveBuffer.Length)
                            {
                                //end of message
                                allData.AddRange(receiveBuffer.Take(chunkLength));
                                break;
                            }

                            allData.AddRange(receiveBuffer);

                            if (simulatedPacketDelayInMilliseconds > 0)
                                Thread.Sleep(simulatedPacketDelayInMilliseconds);
                        }

                        serverResponse = Encoding.UTF8.GetString(allData.ToArray(), 0, allData.Count);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Exception talking with server: " +
                        e.Message + Environment.NewLine + e.StackTrace);
                }

                var checkMessage =
                    message.Length > 100
                        ? message.Substring(0, 100)
                        : message;

                if (!serverResponse.Contains(checkMessage))
                    throw new Exception(
                        "Server returned junk.  Expected it to echo message but it did not." +
                        "Expected it to contain [" + checkMessage + "]. Response: [" + serverResponse + "]");

                requestTimes.Add(requestStopWatch.ElapsedMilliseconds);
            }

            logger.WriteMethodInfo("");

            logger.WriteMethodInfo(
                string.Format("Server Response [{0}]",serverResponse));

            logger.WriteMethodInfo(
                string.Format("Request Times: Min [{0:n0}] Avg [{1:n0}] Max [{2:n0}]",
                    requestTimes.Min(), requestTimes.Average(), requestTimes.Max()));

        }
    }
}
