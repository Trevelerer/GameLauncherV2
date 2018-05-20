using System.Threading.Tasks;
using GameLauncher.Data;
using GameLauncher.ViewModel;

namespace GameLauncher.Services
{
    /// <summary>
    /// A service to handle authentication calls.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Attempt to log in to a server with the given credentials.
        /// </summary>
        /// <param name="server">The <see cref="Server"/> instance</param>
        /// <param name="email">The email address to use</param>
        /// <param name="password">The password to use</param>
        Task<LoginStatus> Login(Server server, string email, string password);

        /// <summary>
        /// Attempt to register a user on a server with the given credentials.
        /// </summary>
        /// <param name="server">The <see cref="Server"/> instance</param>
        /// <param name="email">The email address to use</param>
        /// <param name="password">The password to use</param>
        Task<LoginStatus> Register(Server server, string email, string password);
    }
}
