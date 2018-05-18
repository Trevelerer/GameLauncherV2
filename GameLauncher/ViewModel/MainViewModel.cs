using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.Net;
using System.Xml.Serialization;
using System.IO;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;

using GameLauncher.Classes;
using GameLauncher.Data;
using GameLauncher.Utils;
using GameLauncher.Services;
using LibSpeedLoad.Core;
using LibSpeedLoad.Core.Download;
using LibSpeedLoad.Core.Download.Sources;
using LibSpeedLoad.Core.Download.Sources.StaticCDN;
using LibSpeedLoad.Core.Utils;

namespace GameLauncher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private const string DefaultBanner = "/Resources/Slideshow/Default_Banner.png";

        private Server selectedServer;
        private string statusText;
        private bool loadingServerInfo;
        private AuthState authState;
        private DownloadState downloadState;
        private string email, password;
        private bool loginEnabled, loginInputEnabled;

        private DownloadManager downloadManager;
        private IDialogService dialogService;

        public RelayCommand FetchServersCommand { get; private set; }

        public RelayCommand<Server> SelectServerCommand { get; private set; }

        public RelayCommand LoginCommand { get; private set; }
        public RelayCommand LogoutCommand { get; private set; }
        public RelayCommand PlayCommand { get; private set; }
        public RelayCommand CheckDownloadCommand { get; private set; }

        public ObservableCollection<Server> Servers { get; set; }

        public bool LoginInputEnabled
        {
            get => loginInputEnabled;
            set
            {
                loginInputEnabled = value;
                RaisePropertyChanged("LoginInputEnabled");
            }
        }

        public bool LoginEnabled
        {
            get => loginEnabled;
            set
            {
                loginEnabled = value;
                RaisePropertyChanged("LoginEnabled");
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");

                UpdateLoginStatus();
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged("Password");

                UpdateLoginStatus();
            }
        }

        public AuthState AuthState
        {
            get => authState;
            set
            {
                authState = value;
                RaisePropertyChanged("AuthState");
            }
        }

        public DownloadState DownloadState
        {
            get => downloadState;
            set
            {
                downloadState = value;
                RaisePropertyChanged("DownloadState");
            }
        }

        public Server SelectedServer
        {
            get => selectedServer;
            set
            {
                selectedServer = value;
                RaisePropertyChanged("SelectedServer");
            }
        }

        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value.ToUpper();
                RaisePropertyChanged("StatusText");
            }
        }

        public bool LoadingServer
        {
            get => loadingServerInfo;
            set
            {
                loadingServerInfo = value;
                RaisePropertyChanged("LoadingServer");
            }
        }

        /// <summary>
        /// Set up the view model.
        /// </summary>
        /// <param name="dialogService">The instance of IDialogService.</param>
        public MainViewModel(IDialogService dialogService = null)
        {
            this.dialogService = dialogService ?? SimpleIoc.Default.GetInstance<IDialogService>();

            if (this.dialogService is null)
            {
                throw new ArgumentNullException(nameof(dialogService));
            }

            downloadManager = new DownloadManager();

            FetchServersCommand = new RelayCommand(FetchServers);
            SelectServerCommand = new RelayCommand<Server>(s => SelectServer(s));
            LoginCommand = new RelayCommand(DoLogin);
            CheckDownloadCommand = new RelayCommand(CheckDownload);

            AuthState = new AuthState();
            DownloadState = new DownloadState();
            Servers = new ObservableCollection<Server>();
            Email = string.Empty;
            Password = string.Empty;
            LoginEnabled = false;
            LoginInputEnabled = false;
            StatusText = "Loading...";
        }

        /// <summary>
        /// Check the game directory to see if we have to download the game files.
        /// If we do, then download.
        /// </summary>
        private async void CheckDownload()
        {
            var directory = Configuration.Instance.Config.GameDirectory;

            // If the directory in the config file doesn't exist, create it
            if (!File.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(Path.Combine(directory, "nfsw.exe")))
            {
                var cdnSource = new StaticCdnSource(new CDNDownloadOptions
                {
                    Download = DownloadData.All,
                    GameDirectory = directory,
                    GameVersion = "1614b",
                    GameLanguage = "en"
                });

                cdnSource.ProgressUpdated.Add(OnProgressUpdated);

                downloadManager.Sources.Add(cdnSource);
                downloadManager.DownloadFailed.Add((error) =>
                {
                    Console.WriteLine($"ERROR: {error.Message}");
                });

                downloadState.Phase = DownloadPhase.Downloading;
                await downloadManager.Download();
                GC.Collect();
                downloadState.Phase = DownloadPhase.Inactive;

                StatusText = "Download complete!";
                UpdateLoginStatus();
            }
            else
            {
                // Verify hashes - coming soon

                StatusText = "Ready!";
            }
        }

        /// <summary>
        /// Fetch information about a server and update its list entry.
        /// </summary>
        /// <param name="server">The server to select.</param>
        private async void SelectServer(Server server)
        {
            var infoUrl = server.Address.Append("GetServerInformation").ToString();
            var index = Servers.IndexOf(server);

            LoginEnabled = false;
            LoginInputEnabled = false;
            LoadingServer = true;

            try
            {
                var infoResponse = await WebManager.Client.GetStringAsync(infoUrl);
                var serverInfo = JsonConvert.DeserializeObject<ServerInformation>(infoResponse);

                var banner = new BitmapImage();
                banner.BeginInit();
                banner.UriSource = new Uri("/Resources/Slideshow/Aaron_Lewis_Banner.png", UriKind.Relative);
                banner.EndInit();

                Servers[index].Status = ServerStatus.Online;
                Servers[index].Banner = banner;
                Servers[index].OnlineUsers = serverInfo.OnlineUsers;
                Servers[index].TotalUsers = serverInfo.TotalUsers;

                LoginInputEnabled = true;
                UpdateLoginStatus();
            } catch (HttpRequestException)
            {
                Servers[index].Status = ServerStatus.Offline;
            }
            
            LoadingServer = false;
        }

        /// <summary>
        /// Fetch the list of servers.
        /// </summary>
        private async void FetchServers()
        {
            var serversResponse = await WebManager.Client.GetStringAsync("http://launcher.soapboxrace.world/serverlist.txt");

            var lines = serversResponse.Split('\n');
            var banner = new BitmapImage();
            banner.BeginInit();
            banner.UriSource = new Uri(DefaultBanner, UriKind.Relative);
            banner.EndInit();
            string currentGroup = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("<GROUP>") && line.EndsWith("</GROUP>"))
                {
                    currentGroup = line.Replace("<GROUP>", "").Replace("</GROUP>", "").Replace(";", "");
                    continue;
                }

                if (line.Contains(';'))
                {
                    // Split the info into parts.
                    // First part is the name, second is the address
                    var parts = line.Split(';');
                    var name = parts[parts.Length - 2];
                    var address = parts[parts.Length - 1];

                    Servers.Add(new Server
                    {
                        Name = name,
                        Address = new Uri(address),
                        Group = currentGroup,
                        Banner = banner,
                        Status = ServerStatus.Unknown
                    });
                }
            }
            
            Servers.Add(new Server
            {
                Name = "Testing",
                Address = new Uri("http://192.168.6.13:8680"),
                Group = "Dev",
                Status = ServerStatus.Unknown,
                Banner = banner
            });
        }
        
        /// <summary>
        /// Attempt to log in, and update state if the login is successful.
        /// </summary>
        private async void DoLogin()
        {
            if (SelectedServer == null) return;

            LoginEnabled = false;
            LoginInputEnabled = false;

            var baseUrl = SelectedServer.Address
                .Append(string.Format("User/authenticateUser?email={0}&password={1}",
                    Email,
                    WebUtility.UrlEncode(Hasher.HashSHA1(Password).ToLower())));
            // UserId, LoginToken, Description
            var response = await WebManager.Client.GetAsync(baseUrl);
            var data = await response.Content.ReadAsStringAsync();

            XmlSerializer serializer = new XmlSerializer(typeof(LoginStatus));

            using (TextReader reader = new StringReader(data))
            {
                var loginStatus = (LoginStatus) serializer.Deserialize(reader);

                if (loginStatus.UserID == 0)
                {
                    dialogService.ShowError(loginStatus.Message);
                } else
                {
                    AuthState.LoggedIn = true;
                    AuthState.LoginToken = loginStatus.LoginToken;
                    AuthState.UserID = loginStatus.UserID;
                    AuthState.ServerID = Servers.IndexOf(SelectedServer);
                    AuthState.Email = Email;

                    dialogService.ShowNotification("Logged in!");
                }
            }

            LoginInputEnabled = true;
            UpdateLoginStatus();
        }

        /// <summary>
        /// Helper method to update the enabled-state of the login button.
        /// </summary>
        private void UpdateLoginStatus()
        {
            LoginEnabled = (!string.IsNullOrWhiteSpace(Email)) 
                && (!string.IsNullOrWhiteSpace(Password));
        }

        /// <summary>
        /// Handles progress update events.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="downloaded"></param>
        /// <param name="compressedLength"></param>
        /// <param name="file"></param>
        private void OnProgressUpdated(ulong length, ulong downloaded, ulong compressedLength, string file)
        {
            this.StatusText = $"Downloading: {file} ({FormatUtil.SizeSuffix(downloaded)} / {FormatUtil.SizeSuffix(length)})";

            // progress bar is 519px

            var frac = downloaded / (double)length;
            var bar = 519 - (519 * frac);

            this.DownloadState.BarPercentage = new System.Windows.Thickness(0, 0, bar, 0);
        }
    }
}
