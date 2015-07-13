using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    public class EchoTcpServer : IDisposable
    {
        private readonly ITcpServerLogger _logger;

        private readonly TcpListener _tcpListener;

        private readonly int _simulatedPacketDelayInMilliseconds;

        public EchoTcpServer(ITcpServerLogger logger, IPAddress address, int port, int simulatedPacketDelayInMilliseconds)
        {
            _logger = logger;
            _simulatedPacketDelayInMilliseconds = simulatedPacketDelayInMilliseconds;

            _logger.WriteLine(string.Format(
                "Creating Tcp Listener on [{0}:{1}]",
                address, port));
            _logger.WriteLine("You may need to create a firewall rule before clients can connect.");

            _tcpListener = new TcpListener(address, port);
        }

        public void Start()
        {
            try
            {
                _tcpListener.Start();
            }
            catch (Exception e)
            {
                throw new Exception("Exception starting TcpListener: " + 
                    e.Message + Environment.NewLine + e.StackTrace, e);   
            }

            while (true)
            {
                try
                {
                    var client = _tcpListener.AcceptTcpClient();

                    using (var stream = client.GetStream())
                    {
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

                            if (_simulatedPacketDelayInMilliseconds > 0)
                                Thread.Sleep(_simulatedPacketDelayInMilliseconds);
                        }

                        var data = Encoding.UTF8.GetString(allData.ToArray(), 0, allData.Count);

                        _logger.WriteLine("Received: " + data);

                        var responseBuffer =
                            Encoding.UTF8.GetBytes("Echo: " + data);
                        
                        stream.Write(responseBuffer,0,responseBuffer.Length);

                        stream.Flush();
                    }

                    client.Close();
                }
                catch (Exception e)
                {
                    _logger.WriteLine(
                        "Exception reading / writing to tcp client:" +
                        e.Message + Environment.NewLine + e.StackTrace);

                    _logger.WriteLine("");
                    _logger.WriteLine("Server has suppressed exception and is still alive.");
                }
            }
        }

        public void Dispose()
        {
            if (null != _tcpListener)
            {
                _tcpListener.Stop();
            }
    }
    }
}
