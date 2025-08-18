using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Services;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels
{
    public class RegisterViewModel : ReactiveObject
    {
        private string _username;
        private string _password;
        private string _confirmPassword;
        private readonly Action _onSuccess;

        public string Username { get => _username; set => this.RaiseAndSetIfChanged(ref _username, value); }
        public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }
        public string ConfirmPassword { get => _confirmPassword; set => this.RaiseAndSetIfChanged(ref _confirmPassword, value); }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegisterViewModel(Action onSuccess)
        {
            _onSuccess = onSuccess;

            RegisterCommand = ReactiveCommand.Create(() =>
            {
                if (Password != ConfirmPassword)
                {
                    Console.WriteLine("Passwords do not match");
                    return;
                }

                // Сохраняем пользователя
                LocalDataService.Register(Username, Password);

                // После регистрации можно вызвать success (например, логин)
                _onSuccess?.Invoke();
            });
        }
    }

}
