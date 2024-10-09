using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        //Navigation properties
        public virtual IEnumerable<Message> SentMessages { get; set; }
        public virtual IEnumerable<Message> ReceivedMessages { get; set; }
    }
}
