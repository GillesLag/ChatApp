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
    }
}
