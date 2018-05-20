using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GameLauncher.Classes;
using GameLauncher.Data;
using GameLauncher.Utils;
using GameLauncher.ViewModel;

namespace GameLauncher.Services
{
    public class AuthService : IAuthService
    {
        public async Task<LoginStatus> Login(Server server, string email, string password)
        {
            var baseUrl = server.Address
                .Append(
                    $"User/authenticateUser?email={email}&password={WebUtility.UrlEncode(Hasher.HashSHA1(password).ToLower())}");
            var response = await WebManager.Client.GetAsync(baseUrl);
            var data = await response.Content.ReadAsStringAsync();
            var serializer = new XmlSerializer(typeof(LoginStatus));

            using (TextReader reader = new StringReader(data))
            {
                var loginStatus = (LoginStatus)serializer.Deserialize(reader);

                return loginStatus;
            }
        }

        public async Task<LoginStatus> Register(Server server, string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
