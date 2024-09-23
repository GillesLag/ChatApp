using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatApp.NET.IO;
using System.Windows;
using ChatApp.MVVM.Model;

namespace ChatApp
{
    public class Server
    {
        private TcpClient _client;
        public PacketReader PacketReader { get; set; }

        public delegate void NewUser(User user);
        public delegate void AllUsers(IEnumerable<User> users);

        public event AllUsers AllUsersEvent;
        public event NewUser NewUserEvent;
        public event Action MsgReceivedEvent;
        public event Action LoggedInEvent;
        public event Action DisconnectEvent;
        public event Action FailedLoginOrRegisterEvent;
        public Server()
        {
            _client = new TcpClient();
        }

        public async Task LogginOrRegister(byte opcode, string username, string password)
        {
            await ConnectToServer();

            var loginPacket = new PacketBuilder();
            loginPacket.WriteOpCode(opcode);
            loginPacket.WriteMessage($"{username};{password}");
            _client.Client.Send(loginPacket.GetPacketBytes());

            ReadPackets();
        }

        private async Task ConnectToServer()
        {
            if (!_client.Connected)
            {
                await _client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 5000);
                PacketReader = new PacketReader(_client.GetStream());
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                bool keepRunning = true;
                while (keepRunning)
                {
                    var opcode = PacketReader.ReadByte();
                    string message = string.Empty;

                    switch (opcode)
                    {
                        case 1:
                        case 2:
                            message = PacketReader.ReadMessage();
                            
                            if (message == "0")
                            {
                                FailedLoginOrRegisterEvent.Invoke();
                                _client.Close();
                                keepRunning = false;
                            }
                            else
                            {
                                LoggedInEvent.Invoke();
                            }
                            break;

                        case 3:
                            message = PacketReader.ReadMessage();
                            NewUserEvent.Invoke(new User(message));
                            break;

                        case 4:
                            string[] users = PacketReader.ReadMessage().Split(':');
                            IEnumerable<User> allUsers = users.Select(u => new User(u));
                            AllUsersEvent.Invoke(allUsers);
                            break;

                        case 5:
                            MsgReceivedEvent.Invoke();
                            break;
                    }
                }
            });
        }
    }
}
