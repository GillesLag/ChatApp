using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO_s
{
    public class UserMessageDto
    {
        public IEnumerable<string> Users { get; set; }
        public Dictionary<string, ICollection<string>> Messages { get; set; }
    }
}
