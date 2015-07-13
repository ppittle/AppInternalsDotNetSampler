using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using AppInternalsDotNetSampler.Core.Logging;

namespace AppInternalsDotNetSampler.Core
{
    public class EchoTcpServer : IDisposable
    {
        private readonly ITcpServerLogger _logger;

        private readonly TcpListener _tcpListener;

        public EchoTcpServer(ITcpServerLogger logger, IPAddress address, int port)
        {
            _logger = logger;

            _logger.WriteLine(string.Format(
                "Starting Tcp Listener on [{0}:{1}]",
                address, port));

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
                        string data;
                        using (var sr = new StreamReader(stream))
                        {
                            data = sr.ReadToEnd();

                            _logger.WriteLine("Received: " + data);
                        }

                        using (var sw = new StreamWriter(stream))
                        {
                            sw.Write("Echo: " + data);
                        }
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
