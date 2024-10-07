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
using DotNetEnv;
using System.IO;

namespace ChatApp
{
    public class Server
    {
        private TcpClient _client;
        public PacketReader PacketReader { get; set; }
        public int MyProperty { get; set; }

        public delegate void AllUsers(IEnumerable<User> users);
        
        public event AllUsers AllUsersEvent;
        public event Action NewUserEvent;
        public event Action MsgReceivedEvent;
        public event Action LoggedInEvent;
        public event Action DisconnectEvent;
        public event Action FailedLoginOrRegisterEvent;
        public Server()
        {
            _client = new TcpClient();
            Env.Load("D:\\Projects\\ChatApp\\ChatApp\\.env");
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
                Env.Load("D:\\Projects\\ChatApp\\ChatApp\\.env");
                var ip = Environment.GetEnvironmentVariable("SERVER_IP");

                if (ip is not null)
                {
                    await _client.ConnectAsync(IPAddress.Parse(ip), 5000);
                    PacketReader = new PacketReader(_client.GetStream());
                }
            }
        }

        public void SendMessage(string message)
        {
            var pb = new PacketBuilder();
            pb.WriteOpCode(5);
            pb.WriteMessage(message);

            _client.Client.Send(pb.GetPacketBytes());
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
                            NewUserEvent.Invoke();
                            break;

                        case 4:
                            string[] users = PacketReader.ReadMessage().Split(';');
                            IEnumerable<User> allUsers = users.Select(u => new User(u));
                            AllUsersEvent.Invoke(allUsers);
                            break;

                        case 5:
                            MsgReceivedEvent.Invoke();
                            break;

                        case 10:
                            DisconnectEvent.Invoke();
                            break;
                    }
                }
            });
        }
    }
}
