using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace GameLauncher.Data
{
    public class ServerInformation
    {
        [JsonProperty("messageSrv")]
        public string Message { get; set; }

        [JsonProperty("numberOfRegistered")]
        public uint TotalUsers { get; set; }

        [JsonProperty("onlineNumber")]
        public uint OnlineUsers { get; set; }

        [JsonProperty("requireTicket")]
        public bool TicketRequired { get; set; }
    }
}
