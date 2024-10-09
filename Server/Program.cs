using DAL;
using DAL.Models;
using DAL.UnitOfWork;
using DAL.UnitOfWork.Interfacds;
using Server.NET.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static List<Client> users = new List<Client>();
        static IUnitOfWork unitOfWork;
        static void Main(string[] args)
        {
            unitOfWork = new UnitOfWork(new DAL.Database.Context.ChatDbContext());
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
            var user = unitOfWork.Users.Find(u => u.UserName == username && u.Password == password);
            if (user.Count() == 1)
            {
                Console.WriteLine($"{username} has logged in.");

                return true;
            }

            return false;
        }

        public static bool RegisterUser(string username, string password)
        {
            Console.WriteLine("Client connected, trying to register.");

            var user = new User();
            user.UserName = username;
            user.Password = password;

            try
            {
                unitOfWork.Users.Add(user);
                unitOfWork.SaveChanges();
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
            var sb = new StringBuilder();

            foreach (var user in users.Where(u => u != client))
            {
                sb.Append(user.Username);
                sb.Append(';');
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            else
            {
                return;
            }

            pb.WriteMessage(sb.ToString());
            client.ClientSocket.Client.Send(pb.GetPacketBytes());
        }

        public static void SendMsgToUser(string receiver, string message)
        {
            var user = users.First(u => u.Username == receiver);
            var pb = new PacketBuilder();
            pb.WriteOpCode(5);
            pb.WriteMessage(message);

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