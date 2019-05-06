using System;
using System.Windows.Input;
using DataTracker.Model;

namespace DataTracker.Utility
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _action;
        readonly Func<bool> _canExecute;
        private Action<Changeset> requestCodeReview_OnButtonClick;

        public DelegateCommand(Action<object> execute)
        : this(execute, null)
        {
            _action = execute;
        }

        public DelegateCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));

            _action = execute;
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<Changeset> requestCodeReview_OnButtonClick)
        {
            this.requestCodeReview_OnButtonClick = requestCodeReview_OnButtonClick;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute.Invoke();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
