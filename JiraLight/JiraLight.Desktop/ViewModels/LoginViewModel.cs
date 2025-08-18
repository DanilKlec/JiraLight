using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Services;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private string _username;
        public string Username
        {
            get => _username;
            set => this.RaiseAndSetIfChanged(ref _username, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => this.RaiseAndSetIfChanged(ref _password, value);
        }

        public ReactiveCommand<Unit, Unit> LoginCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenRegisterCommand { get; }

        private readonly Action _onSuccess;

        public LoginViewModel(Action onSuccess)
        {
            _onSuccess = onSuccess;

            LoginCommand = ReactiveCommand.Create(() =>
            {
                // Здесь можно добавить реальную проверку
                if (!string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password))
                {
                    if (LocalDataService.Login(Username, Password))
                        _onSuccess?.Invoke();
                }
            });

            OpenRegisterCommand = ReactiveCommand.Create(() =>
            {
                if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    var registerWindow = new RegisterWindow();
                    registerWindow.DataContext = new RegisterViewModel(() =>
                    {
                        // После регистрации сразу логинимся
                        LocalDataService.Login(Username, Password);

                        // А дальше можно вызвать общий onSuccess (например, переход на Dashboard)
                        _onSuccess?.Invoke();
                    });

                    registerWindow.Show();
                }
            });
        }
    }

}
