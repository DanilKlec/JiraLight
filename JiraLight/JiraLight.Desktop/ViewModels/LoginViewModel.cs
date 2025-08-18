using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenRegisterCommand { get; }

        public LoginViewModel()
        {
            OpenRegisterCommand = ReactiveCommand.Create(() =>
            {
                var register = new Views.RegisterWindow();

                if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    // Передаём главное окно как родительское
                    register.ShowDialog(desktop.MainWindow);
                }
                else
                {
                    register.Show(); // или просто Show() без родителя
                }
            });
        }
    }
}
