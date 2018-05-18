using System.ComponentModel;
using System.Windows;
using LibSpeedLoad.Core;

namespace GameLauncher.ViewModel
{
    /// <summary>
    /// The different phases of the download process.
    /// </summary>
    public enum DownloadPhase
    {
        Inactive = 1,
        Downloading = 2,
        Verifying = 4
    }

    public class DownloadState : INotifyPropertyChanged
    {
        private DownloadPhase downloadPhase;
        private string currentFile;
        private ulong downloadSize;
        private ulong bytesDownloaded;
        private Thickness barPercentage;

        public DownloadPhase Phase
        {
            get => downloadPhase;
            set
            {
                downloadPhase = value;
                RaisePropertyChanged("Phase");
            }
        }

        public string CurrentFile
        {
            get => currentFile;
            set
            {
                currentFile = value;
                RaisePropertyChanged("CurrentFile");
            }
        }

        public ulong DownloadSize
        {
            get => downloadSize;
            set
            {
                downloadSize = value;
                RaisePropertyChanged("DownloadSize");
            }
        }

        public ulong BytesDownloaded
        {
            get => bytesDownloaded;
            set
            {
                bytesDownloaded = value;
                RaisePropertyChanged("BytesDownloaded");
            }
        }

        public Thickness BarPercentage
        {
            get => barPercentage;
            set
            {
                barPercentage = value;
                RaisePropertyChanged("BarPercentage");
            }
        }

        /// <summary>
        /// Initialize the download state.
        /// </summary>
        public DownloadState()
        {
            Phase = DownloadPhase.Inactive;
            BarPercentage = new Thickness(0, 0, 0, 0);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            handlers(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
