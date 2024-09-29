using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.NET.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChatApp.MVVM.ViewModel
{
    public class HomeViewModel
    {
        private Server _server;
        public string Username { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public User Receiver { get; set; }
        public string Message { get; set; }

        public ICommand SendMsgCommand { get; set; }


        public HomeViewModel(Server server)
        {
            Users = new ObservableCollection<User>();
            _server = server;
            _server.NewUserEvent += NewUserEvent;
            _server.AllUsersEvent += AllUsersEvent;
            _server.MsgReceivedEvent += MsgReceivedEvent;
            _server.DisconnectEvent += UserDisconnectedEvent;

            SendMsgCommand = new RelayCommand(SendMessage, () => Receiver != null);
        }

        private void UserDisconnectedEvent()
        {
            string msg = _server.PacketReader.ReadMessage();
            User user = Users.First(x => x.Username == msg);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Remove(user);
            });
        }

        private void MsgReceivedEvent(string sender, string message)
        {
            var user = Users.First(x => x.Username == sender);

            Application.Current.Dispatcher.Invoke(() =>
            {
                user.Messages.Add(message);
            });
        }

        private void AllUsersEvent(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Add(user);
                });
            }
        }

        private void NewUserEvent(User user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Add(user);
            });
        }

        public void SendMessage()
        {
            if (string.IsNullOrWhiteSpace(Message))
            {
                return;
            }

            var time = DateTime.Now;
            string msg = $"{time} {Username}: {Message}";
            Receiver.Messages.Add(msg);

            string msgToSend = $"{Username};{Receiver.Username};{msg}";
            _server.SendMessage(msgToSend);
        }
    }
}
