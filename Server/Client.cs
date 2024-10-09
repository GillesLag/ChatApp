using DAL;
using Microsoft.VisualBasic;
using Server.NET.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private PacketReader _packetReader;
        public TcpClient ClientSocket { get; set; }
        public string Username { get; set; }
        public Client(TcpClient client)
        {
            ClientSocket = client;
            _packetReader = new PacketReader(ClientSocket.GetStream());

            Task.Run(Process);
        }

        private void Process()
        {
            bool keepRunning = true;
            while (keepRunning)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();
                    switch (opcode)
                    {
                        //Authenticate user
                        case 1:
                            PacketBuilder packets = new PacketBuilder();

                            string[] message = _packetReader.ReadMessage().Split(';');
                            string username = message[0];
                            string password = message[1];

                            if (!Program.AuthenticateUser(username, password))
                            {
                                packets.WriteOpCode(1);
                                packets.WriteMessage("0");
                                ClientSocket.Client.Send(packets.GetPacketBytes());

                                Program.Disconnect(this);
                                Console.WriteLine("Wrong credentials, client disconnected");

                                keepRunning = false;
                            }
                            else
                            {
                                Username = username;
                                packets.WriteOpCode(1);
                                packets.WriteMessage("1");
                                ClientSocket.Client.Send(packets.GetPacketBytes());
                                Program.BroadcastMessage(this);
                                Program.GetAllUsers(this);
                            }

                            break;

                        //Register user
                        case 2:
                            message = _packetReader.ReadMessage().Split(';');
                            username = message[0];
                            password = message[1];

                            packets = new PacketBuilder();

                            if (!Program.RegisterUser(username, password))
                            {
                                packets.WriteOpCode(2);
                                packets.WriteMessage("0");
                                ClientSocket.Client.Send(packets.GetPacketBytes());

                                Program.Disconnect(this);
                                Console.WriteLine("Username already exist, client disconnected");

                                keepRunning = false;
                            }
                            else
                            {
                                Username = username;
                                packets.WriteOpCode(2);
                                packets.WriteMessage("1");
                                ClientSocket.Client.Send(packets.GetPacketBytes());
                                Program.BroadcastMessage(this);
                                Program.GetAllUsers(this);
                            }

                            break;

                        //Send message to user
                        case 5:
                            string msg = _packetReader.ReadMessage();
                            Program.SendMsgToUser(msg);
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"{Username} has disconnected");
                    Program.Disconnect(this);
                    ClientSocket.Close();
                    keepRunning = false;
                }
            }
        }
    }
}
