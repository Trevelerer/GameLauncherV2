using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLauncher.Utils;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace GameLauncher.Classes
{
    /// <summary>
    /// Manages the launcher-client info bridge. It's a WebSocket.
    /// </summary>
    public class GameInfoBridge : Singleton<GameInfoBridge>
    {
        private readonly WebSocketServer _server;

        /// <summary>
        /// The constructor
        /// </summary>
        public GameInfoBridge()
        {
            _server = new WebSocketServer("ws://127.0.0.1:7172") {KeepClean = false};
            _server.AddWebSocketService<InfoBridge>("/");
        }

        /// <summary>
        /// Start the info bridge.
        /// </summary>
        public void Start()
        {
            Debug.Assert(!_server.IsListening);
            _server.Start();
        }

        /// <summary>
        /// Stop the info bridge.
        /// </summary>
        public void Stop()
        {
            Debug.Assert(_server.IsListening);
            _server.Stop();
        }

        internal class InfoBridge : WebSocketBehavior
        {
            protected override void OnOpen()
            {
                Console.WriteLine("Opened!");
            }

            protected override void OnMessage(MessageEventArgs e)
            {
                Console.WriteLine("Got message!");
            }
        }
    }
}
