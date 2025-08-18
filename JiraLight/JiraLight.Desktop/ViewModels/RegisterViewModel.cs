using Avalonia.Controls.ApplicationLifetimes;
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

        public string Username { get => _username; set => this.RaiseAndSetIfChanged(ref _username, value); }
        public string Password { get => _password; set => this.RaiseAndSetIfChanged(ref _password, value); }
        public string ConfirmPassword { get => _confirmPassword; set => this.RaiseAndSetIfChanged(ref _confirmPassword, value); }

        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegisterViewModel()
        {
            RegisterCommand = ReactiveCommand.Create(() =>
            {
                if (Password != ConfirmPassword)
                {
                    // Можно добавить всплывающее сообщение
                    Console.WriteLine("Passwords do not match");
                    return;
                }

                // Простейшая регистрация (логика может быть расширена)
                Console.WriteLine($"User {Username} registered");

                // Закрываем окно после регистрации
                if (App.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    foreach (var window in desktop.Windows)
                        if (window is RegisterWindow)
                            window.Close();
                }
            });
        }
    }
}
