﻿using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        IEnumerable<Message> GetAllUserMessages(string username); 
    }
}
