using System;
using System.Windows.Input;

namespace InAppSearch.Commands
{
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action executeAction;
        private Predicate<object> executePredicate;

        public RelayCommand(Action action, Predicate<object> predicate)
        {
            executeAction = action;
            executePredicate = predicate;
        }

        public RelayCommand(Action action) : this(action, null) { }

        public bool CanExecute(object parameter)
        {
            return executePredicate == null || executePredicate.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            executeAction?.Invoke();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> executeAction;
        private Predicate<T> executePredicate;

        public RelayCommand(Action<T> action, Predicate<T> predicate)
        {
            executeAction = action;
            executePredicate = predicate;
        }

        public RelayCommand(Action<T> action) : this(action, null) { }

        public bool CanExecute(object parameter)
        {
            return executePredicate == null || executePredicate.Invoke((T)parameter);
        }

        public void Execute(object parameter)
        {
            if (parameter is T)
                executeAction?.Invoke((T)parameter);
        }
    }
}
