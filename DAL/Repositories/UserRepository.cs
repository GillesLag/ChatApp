using DAL.Database.Context;
using DAL.Models;
using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private ChatDbContext _dbContext;
        public UserRepository(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }

        public void DeleteUser(int id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
            }

            _dbContext.SaveChanges();
        }

        public User? GetUserById(int id)
        {
            return _dbContext.Users.FirstOrDefault(u => u.UserId == id);
        }

        public User? GetUserByUsername(string username)
        {
            return _dbContext.Users.FirstOrDefault(u => u.UserName == username);
        }

        public void UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }

        public bool Login(string username, string password)
        {
            int count = _dbContext.Users.Count(u => u.UserName == username && u.Password == password);

            return count == 1;
        }
    }
}
