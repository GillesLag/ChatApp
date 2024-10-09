using DAL.Database.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public ChatDbContext? ChatDbContext { get { return _dbContext as ChatDbContext; } }
        public MessageRepository(ChatDbContext context) : base(context)
        {
        }

        public IEnumerable<Message> GetAllUserMessages(string username)
        {
            return ChatDbContext.Messages
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.Sender.UserName == username || m.Receiver.UserName == username);
        }
    }
}
