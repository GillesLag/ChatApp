using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using ChatApp.NET.IO;
using System.Windows;

namespace ChatApp
{
    public class Server
    {
        private TcpClient _client;
        public PacketReader PacketReader { get; set; }

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

                    switch (opcode)
                    {
                        case 1:
                        case 2:
                            string message = PacketReader.ReadMessage();
                            
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

                        case 5:
                            MsgReceivedEvent.Invoke();
                            break;
                    }
                }
            });
        }
    }
}
