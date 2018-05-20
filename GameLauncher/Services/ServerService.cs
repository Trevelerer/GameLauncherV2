using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

using GameLauncher.Classes;
using GameLauncher.Data;
using GameLauncher.ViewModel;

namespace GameLauncher.Services
{
    public class ServerService : IServerService
    {
        public async Task<List<Server>> FetchServers()
        {
            var serversResponse = await WebManager.Client.GetStringAsync("http://launcher.soapboxrace.world/serverlist.txt");
            var lines = serversResponse.Split('\n');
            var servers = new List<Server>();

            var banner = new BitmapImage();
            banner.BeginInit();
            banner.UriSource = new Uri("/Resources/Banners/Default_Banner.png", UriKind.Relative);
            banner.EndInit();
            string currentGroup = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("<GROUP>") && line.EndsWith("</GROUP>"))
                {
                    currentGroup = line.Replace("<GROUP>", "").Replace("</GROUP>", "").Replace(";", "");
                    continue;
                }

                if (!line.Contains(';')) continue;

                // Split the info into parts.
                // First part is the name, second is the address
                var parts = line.Split(';');
                var name = parts[parts.Length - 2];
                var address = parts[parts.Length - 1];

                servers.Add(new Server
                {
                    Name = name,
                    Address = new Uri(address),
                    Group = currentGroup,
                    Banner = banner,
                    Status = ServerStatus.Unknown
                });
            }

            return servers;
        }

        public async Task<ServerInformation> FetchServer(string address)
        {
            var infoResponse = await WebManager.Client.GetStringAsync(address);

            return JsonConvert.DeserializeObject<ServerInformation>(infoResponse);
        }

        public async Task<BitmapImage> FetchServerBanner(ServerInformation serverInformation)
        {
            var banner = new BitmapImage();

            if (serverInformation.BannerUrl.Length == 0)
            {
                banner.BeginInit();
                banner.UriSource = new Uri("/Resources/Banners/Aaron_Lewis_Banner.png", UriKind.Relative);
                banner.EndInit();
            }
            else
            {
                var data = await WebManager.Client.GetByteArrayAsync(serverInformation.BannerUrl);
                banner.BeginInit();
                banner.CacheOption = BitmapCacheOption.OnLoad;
                banner.StreamSource = new MemoryStream(data);
                banner.EndInit();
            }

            return banner;
        }
    }
}
