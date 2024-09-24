using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatApp.MVVM.Core
{
    public class RelayCommand : ICommand
    {
        Action<object> _executeParam;
        Func<object, bool> _canExecuteParam;

        Action _execute;
        Func<bool> _canExecute;

        public Action SendMessage { get; }
        public Func<object, bool> Value { get; }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _executeParam = execute;
            _canExecuteParam = canExecute;
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }


        public bool CanExecute(object? parameter)
        {
            if (_canExecute != null)
                return _canExecute();
            return _canExecuteParam == null || _canExecuteParam(parameter);
        }

        public void Execute(object? parameter)
        {
            if (_execute != null)
                _execute();
            else
                _executeParam?.Invoke(parameter);
        }
    }
}
