using ChatApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ChatApp.MVVM.ViewModel
{
    public class HomeViewModel
    {
        private Server _server;
        public string Username { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public HomeViewModel(Server server)
        {
            Users = new ObservableCollection<User>();
            _server = server;
            _server.NewUserEvent += NewUserEvent;
            _server.AllUsersEvent += AllUsersEvent;
        }

        private void AllUsersEvent(IEnumerable<User> users)
        {
            foreach (var user in users)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Users.Add(user);
                });
            }
        }

        private void NewUserEvent(User user)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Users.Add(user);
            });
        }
    }
}
