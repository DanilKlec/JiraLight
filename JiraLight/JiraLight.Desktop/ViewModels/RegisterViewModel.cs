using JiraLight.Desktop.Services;
using ReactiveUI;
using System;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels
{
    public class RegisterViewModel : ReactiveObject
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

        private readonly Action _onSuccess;
        public ReactiveCommand<Unit, Unit> RegisterCommand { get; }

        public RegisterViewModel(Action onSuccess)
        {
            _onSuccess = onSuccess;

            RegisterCommand = ReactiveCommand.Create(() =>
            {
                LocalDataService.Register(Username, Password);
                _onSuccess?.Invoke();
            });
        }
    }

}
