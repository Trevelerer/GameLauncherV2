namespace GameLauncher.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Show an informative MessageBox.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="title">The title to use.</param>
        void ShowNotification(string message, string title = "Notification");

        /// <summary>
        /// Show an error MessageBox.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="title">The title to use.</param>
        void ShowError(string message, string title = "Error");
    }
}
