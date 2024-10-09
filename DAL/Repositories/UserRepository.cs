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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public ChatDbContext ChatDbContext 
        {
            get { return _dbContext as ChatDbContext; } 
        }

        public UserRepository(ChatDbContext context) : base(context)
        {

        }

        public User GetByUsername(string username)
        {
            return ChatDbContext.Users.First(u => u.UserName == username);
        }
    }
}
