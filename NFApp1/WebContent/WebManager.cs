using nanoFramework.WebServer;
using System;
using System.Diagnostics;
using System.Threading;

namespace NFApp1.WebContent
{
    public class WebManager
    {
        private WebServer server;
        public WebManager()
        {

            // Instantiate a new web server on port 80.
            // The Web pages controller must be the last one and the / route must be the last of all attributes
            server = new WebServer(80, HttpProtocol.Http, new Type[] { typeof(ControllerApi), typeof(ControllerWebpages) });
        }

        public void StartServer()
        {
            int count = 0;

            while (count++ <= 5)
            {
                try
                {
                    server.Start();
                    Thread.Sleep(Timeout.Infinite);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error starting WebServer, Message: {ex.Message}");
                }

                Debug.WriteLine($"Retry starting WebServer. Try: {count}/5");
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
            Debug.WriteLine($"Not possible to start WebServer");
        }
    }
}
