using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    interface IUserRepository
    {
        User? GetUserById(int id);
        User? GetUserByUsername(string username);
        void AddUser(User user);
        void DeleteUser(int id);
        void UpdateUser(User user);
        bool Login(string username, string password);
    }
}
