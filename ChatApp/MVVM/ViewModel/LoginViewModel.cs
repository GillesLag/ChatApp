using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChatApp.MVVM.Core;
using ChatApp.MVVM.View;

namespace ChatApp.MVVM.ViewModel
{
    public class LoginViewModel
    {
        private Server _server;
        private Window loginView;

        public string Username { get; set; }
        public string Password { get; set; }

        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public LoginViewModel()
        {
            _server = new Server();
            _server.FailedLoginOrRegisterEvent += FailedToLoginOrRegister;
            LoginCommand = new RelayCommand(Login, x => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
            RegisterCommand = new RelayCommand(Register, x => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
        }

        private void LoggedIn()
        {
            Application.Current.Dispatcher.Invoke(() => {
                Home view = new Home(_server, Username);
                loginView.Close();
                view.Show();
            });
        }   

        public async void Login(object view)
        {
            if (view is Window window)
            {
                loginView = window;
            }
            await _server.LogginOrRegister(1, Username, Password);
            RegisterEvents();
        }

        public async void Register(object view)
        {
            if (view is Window window)
            {
                loginView = window;
            }
            await _server.LogginOrRegister(2, Username, Password);
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _server.LoggedInEvent += LoggedIn;
        }

        private void FailedToLoginOrRegister()
        {
            _server = new Server();
            _server.FailedLoginOrRegisterEvent += FailedToLoginOrRegister;
        }
    }
}
