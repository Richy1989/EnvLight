using nanoFramework.WebServer;
using System;
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
            server.Start();
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
