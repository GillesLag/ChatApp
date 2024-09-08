using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.NET.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _stream;
        public PacketReader(NetworkStream input) : base(input)
        {
            _stream = input;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            int lenght = ReadInt32();
            msgBuffer = new byte[lenght];
            _stream.Read(msgBuffer, 0, lenght);

            string msg = Encoding.ASCII.GetString(msgBuffer);

            return msg;
        }
    }
}
