using System;
using System.Windows.Input;

namespace Launcher.Commands
{
    // https://jebarson.dev/blog/writing-an-asynchronous-relaycommand-implementing-icommand/
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _executeDelegate;
        private readonly Func<T, bool> _canExecuteDelegate;

        #region Ctor

        public RelayCommand(Action<T> executeDelegate, Func<T, bool> canExecuteDelegate = null)
        {
            _executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
            _canExecuteDelegate = canExecuteDelegate;
        }

        #endregion

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => _canExecuteDelegate?.Invoke((T) parameter) ?? true;
        public void Execute(object parameter) => _executeDelegate((T) parameter);
        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}