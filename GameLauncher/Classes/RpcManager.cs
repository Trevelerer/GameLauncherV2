using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;

namespace GameLauncher.Classes
{
    public static class RpcManager
    {
        public static readonly DiscordRpcClient DiscordClient = new DiscordRpcClient("459357727961907200", true, -1);

        public static void Init()
        {
            DiscordClient.Logger = new ConsoleLogger() { Level = LogLevel.Info };
            DiscordClient.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            //Connect to the RPC
            DiscordClient.Initialize();

            //Set the rich presence
            DiscordClient.SetPresence(new RichPresence()
            {
                Details = "In the launcher"
            });

            new Thread(() =>
            {
                while (DiscordClient != null)
                {
                    DiscordClient.Invoke();
                    Thread.Sleep(10);
                }
            }).Start();
        }
    }
}
