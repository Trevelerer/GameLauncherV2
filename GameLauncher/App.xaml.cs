using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

using GameLauncher.Classes;

namespace GameLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // Check to see if the config file exists.
            // If not, we'll create a folder selection dialog.
            // If it does, we'll continue normally.
            if (!Configuration.Instance.Exists)
            {
                var dialog = new CommonOpenFileDialog
                {
                    Title = "Select game directory",
                    IsFolderPicker = true,
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    AddToMostRecentlyUsedList = false,
                    AllowNonFileSystemItems = false,
                    DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    EnsureFileExists = false,
                    EnsurePathExists = false,
                    EnsureReadOnly = false,
                    EnsureValidNames = true,
                    Multiselect = false,
                    ShowPlacesList = false,
                };

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (File.Exists(Path.Combine(dialog.FileName, "nfsw.exe")))
                    {
                        MessageBox.Show("Selected folder already contains Need for Speed: World.", "Error", MessageBoxButton.OK);
                        Current.Shutdown(-1);
                        return;
                    }

                    Configuration.Instance.Create(dialog.FileName);

                    var mainWindow = new MainWindow();
                    Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                    Current.MainWindow = mainWindow;
                    mainWindow.Show();
                } else
                {
                    MessageBox.Show("Please restart the launcher. You must select a folder.", "Error", MessageBoxButton.OK);
                    Current.Shutdown(-1);
                }
            } else
            {
                if (Process.GetProcessesByName("nfsw.exe").Length >= 1)
                {
                    MessageBox.Show("An instance of NFSW is already running.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Current.Shutdown(-1);
                    return;
                }

                var mainWindow = new MainWindow();
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                Current.MainWindow = mainWindow;
                mainWindow.Show();
                mainWindow.Activate();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ServerProxy.Instance.Stop();
            Process.GetCurrentProcess().Kill();
        }
    }
}
