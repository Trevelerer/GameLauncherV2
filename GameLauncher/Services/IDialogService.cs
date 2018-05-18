using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Show an informative MessageBox.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowNotification(string message);

        /// <summary>
        /// Show an error MessageBox.
        /// </summary>
        /// <param name="message">The message to show.</param>
        void ShowError(string message);
    }
}
