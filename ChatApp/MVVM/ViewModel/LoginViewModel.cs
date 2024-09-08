using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChatApp.MVVM.Core;

namespace ChatApp.MVVM.ViewModel
{
    public class LoginViewModel
    {
        private Server _server;

        public string Username { get; set; }
        public string Password { get; set; }

        public RelayCommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(o => Login(), x => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password));
        }

        private void LoggedIn()
        {
            throw new NotImplementedException();
        }

        private void MessageReceived()
        {
            string message = _server.PacketReader.ReadMessage();
        }

        public async void Login()
        {
            _server = new Server();
            _server.MsgReceivedEvent += MessageReceived;
            _server.LoggedInEvent += LoggedIn;
            await _server.ConnectToServerAndLoggin(Username, Password);
        }


    }
}
