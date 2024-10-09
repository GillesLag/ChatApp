using BLL.DTO_s;
using BLL.Services;
using DAL;
using DAL.Models;
using DAL.UnitOfWork;
using DAL.UnitOfWork.Interfacds;
using Newtonsoft.Json;
using Server.NET.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;

namespace Server
{
    class Program
    {
        static List<Client> users = new List<Client>();
        static MessageService messageService;
        static UserService userService;
        static void Main(string[] args)
        {
            var unitOfWork = new UnitOfWork(new DAL.Database.Context.ChatDbContext());
            messageService = new MessageService(unitOfWork);
            userService = new UserService(unitOfWork);
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 5000);
            tcpListener.Start();
            Console.WriteLine("Server started, now listening...");

            while (true)
            {
                Client client = new Client(tcpListener.AcceptTcpClient());
                users.Add(client);
            }
        }

        public static bool AuthenticateUser(string username, string password)
        {
            Console.WriteLine("Client connected, trying to login.");
            if (userService.AuthenticateUser(username, password))
            {
                Console.WriteLine($"{username} has logged in.");
                return true;
            }

            return false;
        }

        public static bool RegisterUser(string username, string password)
        {
            Console.WriteLine("Client connected, trying to register.");

            try
            {
                userService.RegisterUser(username, password);
                Console.WriteLine($"{username} has register and is logged in.");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void BroadcastMessage(Client client)
        {
            var pb = new PacketBuilder();
            pb.WriteOpCode(3);
            pb.WriteMessage(client.Username);

            foreach (var user in users.Where(u => u != client))
            {
                user.ClientSocket.Client.Send(pb.GetPacketBytes());
            }
        }

        public static void GetAllUsers(Client client)
        {
            var pb = new PacketBuilder();
            pb.WriteOpCode(4);

            var message = messageService.GetAllUserConversations(client.Username);
            var users = userService.GetAllUsers();

            UserMessageDto userMsgDto = new UserMessageDto()
            {
                Users = users,
                Messages = message
            };

            pb.WriteMessage(JsonConvert.SerializeObject(userMsgDto));
            client.ClientSocket.Client.Send(pb.GetPacketBytes());
        }

        public static void SendMsgToUser(string message)
        {
            var msgArray = message.Split(';'); 
            string receiver = msgArray[1];

            messageService.HandleMessage(message);

            var pb = new PacketBuilder();
            pb.WriteOpCode(5);
            pb.WriteMessage(message);

            var user = users.First(u => u.Username == receiver);
            user.ClientSocket.Client.Send(pb.GetPacketBytes());
        }

        public static void Disconnect(Client client)
        {
            users.Remove(client);

            if (client.Username == null)
            {
                return;
            }
            foreach (Client user in users)
            {
                var pb = new PacketBuilder();
                pb.WriteOpCode(10);
                pb.WriteMessage(client.Username);
                user.ClientSocket.Client.Send(pb.GetPacketBytes());
            }
        }
    }
}