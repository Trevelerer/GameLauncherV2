﻿using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using GameLauncher.Data;
using GameLauncher.ViewModel;

namespace GameLauncher.Services
{
    /// <summary>
    /// A service to retrieve server data.
    /// </summary>
    public interface IServerService
    {
        /// <summary>
        /// Fetch the server list asynchronously.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> returning a list of <see cref="Server"/> instances.</returns>
        Task<List<Server>> FetchServers();

        /// <summary>
        /// Asynchronously fetch information for the given server.
        /// </summary>
        /// <param name="address">The full URL to the GetServerInformation endpoint of the server.</param>
        /// <returns>A <see cref="Task{TResult}"/> returning a <see cref="ServerInformation"/> instance.</returns>
        Task<ServerInformation> FetchServer(string address);

        /// <summary>
        /// Asynchronously generate a <see cref="BitmapImage"/> of the given server's banner.
        /// </summary>
        /// <param name="serverInformation">The <see cref="ServerInformation"/> entity containing the banner URL.</param>
        /// <returns></returns>
        Task<BitmapImage> FetchServerBanner(ServerInformation serverInformation);

        /// <summary>
        /// Ping the given server.
        /// </summary>
        /// <param name="server">The <see cref="Server"/> entity to ping.</param>
        /// <returns></returns>
        Task<PingReply> PingServer(Server server);
    }
}
