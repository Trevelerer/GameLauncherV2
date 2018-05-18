using System.ComponentModel;

namespace GameLauncher.ViewModel
{
    public class AuthState : INotifyPropertyChanged
    {
        private bool loggedIn;
        private uint userID;
        private string loginToken, email;
        private int serverID;

        public bool LoggedIn
        {
            get => loggedIn;
            set
            {
                loggedIn = value;
                RaisePropertyChanged("LoggedIn");
            }
        }

        public uint UserID
        {
            get => userID;
            set
            {
                userID = value;
                RaisePropertyChanged("UserID");
            }
        }

        public string LoginToken
        {
            get => loginToken;
            set
            {
                loginToken = value;
                RaisePropertyChanged("LoginToken");
            }
        }

        public int ServerID
        {
            get => serverID;
            set
            {
                serverID = value;
                RaisePropertyChanged("ServerID");
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public AuthState()
        {
            LoggedIn = false;
            LoginToken = string.Empty;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
