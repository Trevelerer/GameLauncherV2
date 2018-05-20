using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.Net.Http;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GameLauncher.Classes;
using GameLauncher.Data;
using GameLauncher.Utils;
using GameLauncher.Services;
using LibSpeedLoad.Core;
using LibSpeedLoad.Core.Download.Sources;
using LibSpeedLoad.Core.Download.Sources.StaticCDN;

namespace GameLauncher.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly Dictionary<int, BitmapImage> _bannerCache;

        private int _nfswProcessId;
        private Server _selectedServer;
        private string _statusText;
        private bool _loadingServerInfo;
        private AuthState _authState;
        private DownloadState _downloadState;
        private string _email, _password;
        private bool _loginEnabled, _loginInputEnabled;
        private bool _gameRunning;

        private string _loginButtonText;
        private string _logoutButtonText;
        private string _playButtonText;
        private string _registerButtonText;
        private string _forgotPasswordText;
        private string _rememberCredentialsText;
        private string _loggedInAsText;
        private string _playersOnlineText;
        private string _pingSuccessText, _pingFailedText, _pingDisabledText;

        private readonly DownloadManager _downloadManager;

        private readonly IDialogService _dialogService;
        private readonly IAuthService _authService;
        private readonly IServerService _serverService;
        private readonly ILanguageService _languageService;

        public RelayCommand FetchServersCommand { get; }
        public RelayCommand<Server> SelectServerCommand { get; }
        public RelayCommand LoginCommand { get; }
        public RelayCommand LogoutCommand { get; }
        public RelayCommand PlayCommand { get; }
        public RelayCommand CheckDownloadCommand { get; }
        public RelayCommand OpenSettingsCommand { get; }

        public LanguagePack LanguagePack => _languageService.GetLanguagePack(
            Configuration.Instance.Config.LauncherLanguage);

        public string LoginButtonText
        {
            get => _loginButtonText;
            set
            {
                _loginButtonText = value;
                RaisePropertyChanged();
            }
        }

        public string LogoutButtonText
        {
            get => _logoutButtonText;
            set
            {
                _logoutButtonText = value;
                RaisePropertyChanged();
            }
        }

        public string PlayButtonText
        {
            get => _playButtonText;
            set
            {
                _playButtonText = value;
                RaisePropertyChanged();
            }
        }

        public string RegisterButtonText
        {
            get => _registerButtonText;
            set
            {
                _registerButtonText = value;
                RaisePropertyChanged();
            }
        }

        public string ForgotPasswordText
        {
            get => _forgotPasswordText;
            set
            {
                _forgotPasswordText = value;
                RaisePropertyChanged();
            }
        }

        public string RememberCredentialsText
        {
            get => _rememberCredentialsText;
            set
            {
                _rememberCredentialsText = value;
                RaisePropertyChanged();
            }
        }

        public string LoggedInAsText
        {
            get => _loggedInAsText;
            set
            {
                _loggedInAsText = value;
                RaisePropertyChanged();
            }
        }

        public string PlayersOnlineText
        {
            get => _playersOnlineText;
            set
            {
                _playersOnlineText = value;
                RaisePropertyChanged();
            }
        }
        public string PingSuccessText
        {
            get => _pingSuccessText;
            set
            {
                _pingSuccessText = value;
                RaisePropertyChanged();
            }
        }
        public string PingFailedText
        {
            get => _pingFailedText;
            set
            {
                _pingFailedText = value;
                RaisePropertyChanged();
            }
        }
        public string PingDisabledText
        {
            get => _pingDisabledText;
            set
            {
                _pingDisabledText = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Server> Servers { get; set; }

        public bool LoginInputEnabled
        {
            get => _loginInputEnabled;
            set
            {
                _loginInputEnabled = value;
                RaisePropertyChanged();
            }
        }

        public bool LoginEnabled
        {
            get => _loginEnabled;
            set
            {
                _loginEnabled = value;
                RaisePropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChanged();

                UpdateLoginStatus();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged();

                UpdateLoginStatus();
            }
        }

        public AuthState AuthState
        {
            get => _authState;
            set
            {
                _authState = value;
                RaisePropertyChanged();
            }
        }

        public DownloadState DownloadState
        {
            get => _downloadState;
            set
            {
                _downloadState = value;
                RaisePropertyChanged();
            }
        }

        public Server SelectedServer
        {
            get => _selectedServer;
            set
            {
                _selectedServer = value;
                RaisePropertyChanged();
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                _statusText = value.ToUpper();
                RaisePropertyChanged();
            }
        }

        public bool LoadingServer
        {
            get => _loadingServerInfo;
            set
            {
                _loadingServerInfo = value;
                RaisePropertyChanged();
            }
        }

        public bool GameRunning
        {
            get => _gameRunning;
            set
            {
                _gameRunning = value;
                RaisePropertyChanged();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Set up the view model.
        /// </summary>
        /// <param name="dialogService">An instance of <see cref="IDialogService"/></param>
        /// <param name="authService">An instance of <see cref="IAuthService"/></param>
        /// <param name="serverService">An instance of <see cref="IServerService"/></param>
        /// <param name="languageService">An instance of <see cref="ILanguageService"/></param>
        public MainViewModel(
            IDialogService dialogService = null, 
            IAuthService authService = null, 
            IServerService serverService = null,
            ILanguageService languageService = null
        )
        {
            _dialogService = dialogService ?? SimpleIoc.Default.GetInstance<IDialogService>();
            _authService = authService ?? SimpleIoc.Default.GetInstance<IAuthService>();
            _serverService = serverService ?? SimpleIoc.Default.GetInstance<IServerService>();
            _languageService = languageService ?? SimpleIoc.Default.GetInstance<ILanguageService>();

            if (_dialogService is null)
            {
                throw new ArgumentNullException(nameof(dialogService));
            }

            if (_authService is null)
            {
                throw new ArgumentNullException(nameof(authService));
            }

            if (_serverService is null)
            {
                throw new ArgumentNullException(nameof(serverService));
            }

            if (_languageService is null)
            {
                throw new ArgumentNullException(nameof(languageService));
            }

            _bannerCache = new Dictionary<int, BitmapImage>();
            _downloadManager = new DownloadManager();

            // Create commands
            FetchServersCommand = new RelayCommand(FetchServers);
            SelectServerCommand = new RelayCommand<Server>(SelectServer);
            LoginCommand = new RelayCommand(DoLogin);
            CheckDownloadCommand = new RelayCommand(CheckDownload);
            LogoutCommand = new RelayCommand(DoLogout);
            PlayCommand = new RelayCommand(RunGame);
            OpenSettingsCommand = new RelayCommand(OpenSettings);

            // Set up state
            AuthState = new AuthState();
            DownloadState = new DownloadState();
            Servers = new ObservableCollection<Server>();
            Email = string.Empty;
            Password = string.Empty;
            LoginEnabled = false;
            LoginInputEnabled = false;
            GameRunning = false;

            _languageService.LoadPacks();

            StatusText = LanguagePack.GetPhrase("status.loading");
            LoginButtonText = LanguagePack.GetPhrase("labels.login");
            LogoutButtonText = LanguagePack.GetPhrase("labels.logout");
            PlayButtonText = LanguagePack.GetPhrase("labels.play");
            RegisterButtonText = LanguagePack.GetPhrase("labels.register");
            ForgotPasswordText = LanguagePack.GetPhrase("labels.forgot_password");
            RememberCredentialsText = LanguagePack.GetPhrase("labels.remember_credentials");
            LoggedInAsText = LanguagePack.GetPhrase("labels.logged_in_as");
            PlayersOnlineText = LanguagePack.GetPhrase("labels.players_online");
            PingSuccessText = LanguagePack.GetPhrase("ping.success");
            PingFailedText = LanguagePack.GetPhrase("ping.failed");
            PingDisabledText = LanguagePack.GetPhrase("ping.disabled");
        }

        /// <summary>
        /// Check the game directory to see if we have to download the game files.
        /// If we do, then download.
        /// </summary>
        private async void CheckDownload()
        {
            var directory = Configuration.Instance.Config.GameDirectory;
            var cdnSource = new StaticCdnSource(new CDNDownloadOptions
            {
                Download = DownloadData.All,
                GameDirectory = directory,
                GameVersion = "1614b",
                GameLanguage = "en"
            });

            _downloadManager.Sources.Add(cdnSource);

            // If the directory in the config file doesn't exist, create it
            if (!File.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(Path.Combine(directory, "nfsw.exe")))
            {
                cdnSource.ProgressUpdated.Add(OnProgressUpdated);
                
                _downloadState.Phase = DownloadPhase.Downloading;
                await _downloadManager.Download();
                GC.Collect();
                cdnSource.VerificationProgressUpdated.Add(OnVerificationProgressUpdated);
                cdnSource.VerificationFailed.Add(OnVerificationFailed);

                _downloadState.Phase = DownloadPhase.Verifying;
                StatusText = LanguagePack.GetPhrase("status.verifying");
                await _downloadManager.VerifyHashes();
                StatusText = LanguagePack.GetPhrase("status.download.complete");

                _downloadState.Phase = DownloadPhase.Inactive;
                UpdateLoginStatus();
            }

            StatusText = LanguagePack.GetPhrase("status.ready");
        }

        /// <summary>
        /// Fetch information about a server and update its list entry.
        /// </summary>
        /// <param name="server">The server to select.</param>
        private async void SelectServer(Server server)
        {
            // We don't want logins to occur while loading
            LoginEnabled = false;
            LoginInputEnabled = false;
            LoadingServer = true;

            var infoUrl = server.Address.Append("GetServerInformation").ToString();
            var serverIndex = Servers.IndexOf(server);
            var listServer = Servers[serverIndex];

            listServer.Status = ServerStatus.Unknown;

            try
            {
                var serverInfo = await _serverService.FetchServer(infoUrl);

                if (!_bannerCache.ContainsKey(serverIndex))
                {
                    var banner = await _serverService.FetchServerBanner(serverInfo);

                    listServer.Banner = banner;

                    _bannerCache[serverIndex] = banner;
                }

                listServer.OnlineUsers = serverInfo.OnlineUsers;
                listServer.TotalUsers = serverInfo.TotalUsers;
                listServer.Status = ServerStatus.Online;

                LoginInputEnabled = true;
                UpdateLoginStatus();
            }
            catch (HttpRequestException)
            {
                listServer.Status = ServerStatus.Offline;
            }

            LoadingServer = false;
        }

        /// <summary>
        /// Fetch the list of servers.
        /// </summary>
        private async void FetchServers()
        {
            Servers.Clear();

            foreach (var server in await _serverService.FetchServers())
            {
                Servers.Add(server);
            }
        }
        
        /// <summary>
        /// Attempt to log in, and update state if the login is successful.
        /// </summary>
        private async void DoLogin()
        {
            if (SelectedServer == null) return;

            LoginEnabled = false;
            LoginInputEnabled = false;

            var result = await _authService.Login(SelectedServer, _email, _password);

            if (result.UserID == 0)
            {
                _dialogService.ShowError(result.Message, "Login");
            }
            else
            {
                AuthState.ServerID = Servers.IndexOf(SelectedServer);
                AuthState.LoggedIn = true;
                AuthState.LoginToken = result.LoginToken;
                AuthState.UserID = result.UserID;
                AuthState.Email = _email;

                _dialogService.ShowNotification(LanguagePack.GetPhrase("login.success"));
                //_dialogService.ShowNotification("Login successful!", "Login");
            }

            LoginInputEnabled = true;
            UpdateLoginStatus();
        }

        /// <summary>
        /// Open the settings window.
        /// </summary>
        private void OpenSettings()
        {
            var window = new SettingsWindow { Owner = Application.Current.MainWindow };
            window.Show();
            window.Activate();
        }

        /// <summary>
        /// Reset the authentication state.
        /// </summary>
        private void DoLogout()
        {
            _authState.UserID = 0;
            _authState.LoginToken = string.Empty;
            _authState.LoggedIn = false;

            UpdateLoginStatus();
        }

        /// <summary>
        /// Run the game.
        /// </summary>
        private void RunGame()
        {
            if (!_authState.LoggedIn)
            {
                _dialogService.ShowError(LanguagePack.GetPhrase("errors.not_logged_in"));
                //_dialogService.ShowError("You're not logged in. Huh?");
                return;
            }

            var directory = Configuration.Instance.Config.GameDirectory;

            if (!File.Exists(Path.Combine(directory, "nfsw.exe")))
            {
                _dialogService.ShowError(LanguagePack.GetPhrase("errors.game_not_found"));
                //_dialogService.ShowError("Couldn't find the game executable. Please restart the launcher.");
                Environment.Exit(1);
            }

            var server = Servers[AuthState.ServerID];
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(directory, "nfsw.exe"),
                UseShellExecute = true,
                Arguments = $"US {server.Address} {AuthState.LoginToken} {AuthState.UserID}",
                WorkingDirectory = directory
            };

            var process = Process.Start(psi);

            if (process == null)
            {
                _dialogService.ShowError(LanguagePack.GetPhrase("errors.launch_failed"));
                return;
            }

            process.EnableRaisingEvents = true;
            _nfswProcessId = process.Id;
            GameRunning = true;
            process.Exited += OnGameExited;
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
            StatusText = LanguagePack.GetPhrase("status.download.progress",
                file,
                FormatUtil.SizeSuffix(downloaded),
                FormatUtil.SizeSuffix(length));
            //StatusText = $"Downloading: {file} ({FormatUtil.SizeSuffix(downloaded)} / {FormatUtil.SizeSuffix(length)})";

            // progress bar is 519px

            var frac = downloaded / (double)length;
            var bar = 519 - (519 * frac);

            DownloadState.BarPercentage = new Thickness(0, 0, bar, 0);
        }

        /// <summary>
        /// Handles verification progress update events.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="displayFile"></param>
        /// <param name="filenumber"></param>
        /// <param name="totalFiles"></param>
        private void OnVerificationProgressUpdated(string file, string displayFile, uint filenumber, uint totalFiles)
        {
            StatusText = LanguagePack.GetPhrase("status.verifying.progress", displayFile);

            // progress bar is 519px

            var frac = filenumber / (double)totalFiles;
            var bar = 519 - (519 * frac);

            DownloadState.BarPercentage = new Thickness(0, 0, bar, 0);
        }

        /// <summary>
        /// Handles the verification failed event.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="expectedhash"></param>
        /// <param name="actualhash"></param>
        private void OnVerificationFailed(string file, string expectedhash, string actualhash)
        {
            _dialogService.ShowError(
                LanguagePack.GetPhrase("verification.failed",
                    file,
                    expectedhash,
                    actualhash
                ), 
                "Downloader"
            );
            //_dialogService.ShowError($"Verifying file {file} failed. Expected hash {expectedhash}, got {actualhash}. Please delete your game files and try again.", "Downloader");
            Environment.Exit(1);
        }

        /// <summary>
        /// Called when the game process exits.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGameExited(object sender, EventArgs e)
        {
            if (!(sender is Process process))
            {
                return;
            }

            if (process.ExitCode != 0)
            {
                _dialogService.ShowError(LanguagePack.GetPhrase("errors.game_crashed"));
            }

            _nfswProcessId = 0;
            GameRunning = false;

            DoLogout();
        }
    }
}
