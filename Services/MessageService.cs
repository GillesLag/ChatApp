using DAL.Models;
using DAL.UnitOfWork.Interfacds;
using Server.NET.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Services
{
    public class MessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public byte[] HandleMessage(string msg)
        {
            var msgArray = msg.Split(';');
            string sender = msgArray[0];
            string receiver = msgArray[1];
            DateTime date = DateTime.Parse(msgArray[2]);
            string msgText = msgArray[3];

            User senderUser = _unitOfWork.Users.GetByUsername(sender);
            User receiverUser = _unitOfWork.Users.GetByUsername(receiver);

            Message message = new Message()
            {
                Sender = senderUser,
                Receiver = receiverUser,
                MessageText = msgText,
                Sent = date
            };

            _unitOfWork.Messages.Add(message);
            _unitOfWork.SaveChanges();

            var pb = new PacketBuilder();
            pb.WriteOpCode(5);
            pb.WriteMessage(msg);

            return pb.GetPacketBytes();
        }
    }
}
