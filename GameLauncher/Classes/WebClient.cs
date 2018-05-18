using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Classes
{
    public class WebManager
    {
        private static HttpClient client = null;
        private static readonly object _clientLock = new object();

        WebManager() { }

        public static HttpClient Client
        {
            get
            {
                lock (_clientLock)
                {
                    if (client == null)
                    {
                        client = new HttpClient();
                    }

                    return client;
                }
            }
        }
    }
}
