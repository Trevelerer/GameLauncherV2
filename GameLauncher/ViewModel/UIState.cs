using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.ViewModel
{
    /// <summary>
    /// The different states that the main UI can be in.
    /// </summary>
    public enum UIState
    {
        Booting = 0,
        LoadingServers = 1,
        LoadingServer = 2,
        Authenticating = 4,
        CheckingFiles = 8,
        DownloadingFiles = 16,
        LoginFormDisabled = 32,
        LoginDisabled = 64,
        Ready = 128
    }
}
