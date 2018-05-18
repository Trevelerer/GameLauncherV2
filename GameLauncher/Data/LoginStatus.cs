using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.Data
{
    [XmlRoot("LoginStatusVO")]
    public class LoginStatus
    {
        [XmlElement("UserId")]
        public uint UserID { get; set; }

        [XmlElement("LoginToken")]
        public string LoginToken { get; set; }

        [XmlElement("Description")]
        public string Message { get; set; }
    }
}
