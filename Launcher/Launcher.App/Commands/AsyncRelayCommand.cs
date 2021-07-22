using System;
using System.Threading.Tasks;
using System.Windows.Input;

// ReSharper disable MemberCanBePrivate.Global

namespace Launcher.Commands
{
    // https://mike-ward.net/2013/08/09/asynccommand-implementation-in-wpf/
    public class AsyncRelayCommand<T> : ICommand
    {
        private readonly Func<T, Task> _executeDelegate;
        private readonly Func<T, bool> _canExecuteDelegate;

        private bool _isExecuting;

        #region Ctor

        public AsyncRelayCommand(Func<T, Task> executeDelegate, Func<T, bool> canExecuteDelegate = null)
        {
            _executeDelegate = executeDelegate ?? throw new ArgumentNullException(nameof(executeDelegate));
            _canExecuteDelegate = canExecuteDelegate;
        }

        #endregion

        #region ICommand

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
            => _isExecuting == false && (_canExecuteDelegate?.Invoke((T) parameter) ?? true);

        public async void Execute(object parameter)
        {
            // set internal state as busy
            _isExecuting = true;

            // trigger refresh CanExecute
            OnCanExecuteChanged();
            
            try
            {
                await _executeDelegate((T) parameter);
            }
            finally
            {
                _isExecuting = false;
                OnCanExecuteChanged();
            }
        }
        public void OnCanExecuteChanged()
            => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}