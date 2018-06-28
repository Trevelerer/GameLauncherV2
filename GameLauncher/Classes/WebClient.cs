using System.Net.Http;

namespace GameLauncher.Classes
{
    public class WebManager
    {
        private static HttpClient _client;
        private static readonly object ClientLock = new object();

        private WebManager() { }

        public static HttpClient Client
        {
            get
            {
                lock (ClientLock)
                {
                    return _client ?? (_client = new HttpClient());
                }
            }
        }
    }
}
