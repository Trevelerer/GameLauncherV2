using System;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace GameLauncher.ViewModel
{
    public enum ServerStatus
    {
        Online,
        Offline,
        Unknown
    }

    public enum PingStatus
    {
        Pinged,
        PingFailed,
        Pinging,
        Unknown,
    }

    public class Server : INotifyPropertyChanged
    {
        private string _name;
        private Uri _address;
        private string _group;
        private BitmapImage _banner;
        private ServerStatus _status;
        private uint _onlineUsers;
        private uint _totalUsers;
        private long _ping;
        private PingStatus _pingStatus;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        public Uri Address
        {
            get => _address;
            set
            {
                _address = value;
                RaisePropertyChanged("Address");
            }
        }

        public string Group
        {
            get => _group;

            set
            {
                _group = value;
                RaisePropertyChanged("Group");
            }
        }

        public BitmapImage Banner
        {
            get => _banner;
            set
            {
                _banner = value;
                RaisePropertyChanged("Banner");
            }
        }

        public ServerStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                RaisePropertyChanged("Status");
            }
        }

        public uint OnlineUsers
        {
            get => _onlineUsers;
            set
            {
                _onlineUsers = value;
                RaisePropertyChanged("OnlineUsers");
            }
        }

        public uint TotalUsers
        {
            get => _totalUsers;
            set
            {
                _totalUsers = value;
                RaisePropertyChanged("TotalUsers");
            }
        }

        public long Ping
        {
            get => _ping;
            set
            {
                _ping = value;
                RaisePropertyChanged("Ping");
            }
        }

        public PingStatus PingStatus
        {
            get => _pingStatus;
            set
            {
                _pingStatus = value;
                RaisePropertyChanged("PingStatus");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
