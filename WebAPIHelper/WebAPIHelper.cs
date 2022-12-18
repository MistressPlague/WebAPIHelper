using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Plagues_Booty.Libraries
{
    internal class WebAPIHelper
    {
        /// <summary>
        /// The internal listener instance.
        /// </summary>
        internal HttpListener Listener = new HttpListener();

        private Action<string, RequestType, HttpListenerContext> OnReceived;

        /// <summary>
        /// The type of request being/to be handled.
        /// </summary>
        internal enum RequestType
        {
            GET,
            POST,
            PUT,
            PATCH,
            DELETE,
            HEAD,
            OPTIONS
        }

        private RequestType ListenFor;

        /// <summary>
        /// A Helper class for making web API's.
        /// </summary>
        /// <param name="OnReceived">What to do once data is received from a client. Be sure to response by calling SendResponse in this class through this event code.</param>
        /// <param name="DomainDir">The domain directory to bind to, such as "test" would host on http://*:{Port}/test/. Hosts on root if null.</param>
        /// <param name="Port">The port to host on, note this will need to be an opened port.</param>
        /// <param name="ListenFor">The type(s) of requests to listen for. You can OR multiple together, such as RequestType.GET | RequestType.POST.</param>
        internal WebAPIHelper(Action<string, RequestType, HttpListenerContext> OnReceived, string DomainDir, int Port, RequestType ListenFor = RequestType.POST)
        {
            Listener.Prefixes.Add($"http://*:{Port}" + (DomainDir != null ? $"/{DomainDir}/" : ""));

            this.ListenFor = ListenFor;
            this.OnReceived = OnReceived;
            Listener.Start();

            Task.Run(CheckForData);
        }

        private void CheckForData()
        {
            var context = Listener.GetContext();

            foreach (RequestType value in Enum.GetValues(typeof(RequestType)))
            {
                if ((ListenFor & value) == value)
                {
                    if (context.Request.HttpMethod == Enum.GetName(typeof(RequestType), value))
                    {
                        var jsonData = JsonConvert.SerializeObject(QueryStringHelper.QueryStringToDict(new StreamReader(context.Request.InputStream, context.Request.ContentEncoding).ReadToEnd()));

                        OnReceived?.Invoke(jsonData, value, context);

                        break;
                    }
                }
            }

            CheckForData();
        }

        /// <summary>
        /// Sends a response to the remote client.
        /// </summary>
        /// <param name="Context">The context, which defines what client is being responded to, etc.</param>
        /// <param name="Response">The data to respond with. Note this is turned to UTF8 internally.</param>
        /// <param name="ResponseStatusCode">What status code to respond with, such as OK.</param>
        internal void SendResponse(HttpListenerContext Context, string Response, HttpStatusCode ResponseStatusCode)
        {
            var response = Encoding.UTF8.GetBytes(Response);
            Context.Response.StatusCode = (int)ResponseStatusCode;

            Context.Response.ContentLength64 = response.Length;
            Context.Response.OutputStream.Write(response, 0, response.Length);
            Context.Response.OutputStream.Close();
        }
    }
}
