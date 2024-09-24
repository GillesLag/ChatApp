using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.MVVM.Model
{
    public class User
    {
        public string Username { get; set; }
        public ObservableCollection<string> Messages { get; set; }
        public User(string username)
        {
            Username = username;
            Messages = new ObservableCollection<string>();
        }
    }
}
