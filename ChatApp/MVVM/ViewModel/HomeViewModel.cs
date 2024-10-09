using BLL.DTO_s;
using ChatApp.MVVM.Core;
using ChatApp.MVVM.Model;
using ChatApp.NET.IO;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
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

        private void MsgReceivedEvent()
        {
            var message = _server.PacketReader.ReadMessage().Split(';');
            var user = Users.First(x => x.Username == message[0]);

            Application.Current.Dispatcher.Invoke(() =>
            {
                user.Messages.Add($"{message[2]} {message[0]}: {message[3]}");
            });
        }

        private void AllUsersEvent()
        {
            string message = _server.PacketReader.ReadMessage();
            UserMessageDto msgDto = JsonConvert.DeserializeObject<UserMessageDto>(message);

            foreach (var user in msgDto.Users)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Add(new User(user));
                });
            }

            foreach (var msg in msgDto.Messages)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var user = Users.FirstOrDefault(u => u.Username == msg.Key);
                    foreach (var item in msg.Value)
                    {
                        user.Messages.Add(item);
                    }
                });
            }
        }

        private void NewUserEvent()
        {
            string user = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Add(new User(user));
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

            string msgToSend = $"{Username};{Receiver.Username};{time};{Message}";
            _server.SendMessage(msgToSend);
        }
    }
}
