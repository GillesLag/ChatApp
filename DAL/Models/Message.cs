using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public string MessageText { get; set; }
        [Required]
        public DateTime Sent { get; set; }
        [Required]
        public int SenderId { get; set; }
        [Required]
        public int ReceiverId { get; set; }

        //Navigation properties
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
