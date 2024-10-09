using BLL.DTO_s;
using DAL.Models;
using DAL.UnitOfWork.Interfacds;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class MessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void HandleMessage(string msg)
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
        }

        public Dictionary<string, ICollection<string>> GetAllUserConversations(string username)
        {
            var messages = _unitOfWork.Messages.GetAllUserMessages(username);
            messages = messages.OrderBy(m => m.Sent).ToList();

            Dictionary<string, ICollection<string>> msg = new Dictionary<string, ICollection<string>>();

            foreach (var item in messages)
            {
                if(item.Sender.UserName != username)
                {
                    string msgText = $"{item.Sent.ToString()} {item.Sender.UserName}: {item.MessageText}";
                    if (msg.TryGetValue(item.Sender.UserName, out ICollection<string> conversations))
                    {
                        conversations.Add(msgText);
                    }
                    else
                    {
                        msg.Add(item.Sender.UserName, new Collection<string>() { msgText });
                    }
                }
                else
                {
                    string msgText = $"{item.Sent.ToString()} {item.Receiver.UserName}: {item.MessageText}";
                    if (msg.TryGetValue(item.Receiver.UserName, out ICollection<string> conversations))
                    {
                        conversations.Add(msgText);
                    }
                    else
                    {
                        msg.Add(item.Receiver.UserName, new Collection<string>() { msgText });
                    }
                }
            }

            return msg;
        }
    }
}
