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
        private object _currentPage;
        public object CurrentPage
        {
            get => _currentPage;
            set => this.RaiseAndSetIfChanged(ref _currentPage, value);
        }
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

        private readonly MainViewModel _mainViewModel;
        private readonly Action _onSuccess;

        public LoginViewModel(Action onSuccess, MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
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
                // Создаём страницу регистрации и её ViewModel
                var registerPage = new RegisterPage
                {
                    DataContext = new RegisterViewModel(() =>
                    {
                        // После успешной регистрации сразу логинимся
                        LocalDataService.Login(Username, Password);

                        // Переходим на Dashboard
                        _onSuccess?.Invoke();
                    })
                };

                // Меняем текущую страницу MainViewModel
                _mainViewModel.CurrentPage = registerPage;
            });
            _mainViewModel = mainViewModel;
        }
    }

}
