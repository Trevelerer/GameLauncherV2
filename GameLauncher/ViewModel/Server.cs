using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace GameLauncher.ViewModel
{
    public enum ServerStatus
    {
        Online,
        Offline,
        Unknown
    }

    public class Server : INotifyPropertyChanged
    {
        private string name;
        private Uri address;
        private string group;
        private BitmapImage banner;
        private ServerStatus status;
        private uint onlineUsers;
        private uint totalUsers;

        public string Name
        {
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public Uri Address
        {
            get => address;
            set
            {
                address = value;
                RaisePropertyChanged("Address");
            }
        }

        public string Group
        {
            get => group;

            set
            {
                group = value;
                RaisePropertyChanged("Group");
            }
        }

        public BitmapImage Banner
        {
            get => banner;
            set
            {
                banner = value;
                RaisePropertyChanged("Banner");
            }
        }

        public ServerStatus Status
        {
            get => status;
            set
            {
                status = value;
                RaisePropertyChanged("Status");
            }
        }

        public uint OnlineUsers
        {
            get => onlineUsers;
            set
            {
                onlineUsers = value;
                RaisePropertyChanged("OnlineUsers");
            }
        }

        public uint TotalUsers
        {
            get => totalUsers;
            set
            {
                totalUsers = value;
                RaisePropertyChanged("TotalUsers");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
