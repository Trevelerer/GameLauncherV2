using GalaSoft.MvvmLight.Ioc;
using GameLauncher.Services;

namespace GameLauncher.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IAuthService, AuthService>();
            SimpleIoc.Default.Register<IServerService, ServerService>();
            SimpleIoc.Default.Register<ILanguageService, LanguageService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        public MainViewModel Main => SimpleIoc.Default.GetInstance<MainViewModel>();

        public IDialogService DialogService => SimpleIoc.Default.GetInstance<IDialogService>();
        public IAuthService AuthService => SimpleIoc.Default.GetInstance<IAuthService>();
        public IServerService ServerService => SimpleIoc.Default.GetInstance<IServerService>();
        public ILanguageService LanguageService => SimpleIoc.Default.GetInstance<ILanguageService>();
    }
}
