using DAL.Models;
using DAL.UnitOfWork.Interfacds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void RegisterUser(string username, string password)
        {
            User user = new User()
            {
                UserName = username,
                Password = password
            };

            _unitOfWork.Users.Add(user);
            _unitOfWork.SaveChanges();
        }

        public bool AuthenticateUser(string username, string password)
        {
            var users = _unitOfWork.Users.Find(u => u.UserName == username && u.Password == password);
            return users.Count() == 1;
        }
    }
}
