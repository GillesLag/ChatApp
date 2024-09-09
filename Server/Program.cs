using DAL;
using Server.NET.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static List<Client> users = new List<Client>();
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
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
            if (DatabaseOperations.Login(username, password))
            {
                Console.WriteLine($"{username} has logged in.");

                return true;
            }

            return false;
        }

        public static void Disconnect(Client client)
        {
            users.Remove(client);
            client.ClientSocket.Close();
        }
    }
}