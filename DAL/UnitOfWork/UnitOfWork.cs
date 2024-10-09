using DAL.Database.Context;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using DAL.UnitOfWork.Interfacds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatDbContext _context;
        public IUserRepository Users {  get; private set; }

        public IMessageRepository Messages {  get; private set; }

        public UnitOfWork(ChatDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            Messages = new MessageRepository(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
