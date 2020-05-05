using Scanner.Extensions.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Scanner.Extensions
{
    /// <summary>
    /// https://johnthiriet.com/mvvm-going-async-with-async-command/
    /// </summary>
    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;
        private readonly IErrorHandler errorHandler;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            IErrorHandler errorHandler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute()
        {
            return !isExecuting && (canExecute?.Invoke() ?? true);
        }

        /// <summary>
        /// Этот метод публичный для команд, которые не используют связывание.
        /// Но лучше использовать метод "Execute", так как там есть обёртка с обработкой исключений
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    isExecuting = true;
                    await execute();
                }
                finally
                {
                    isExecuting = false;
                }
            }
            //TODO: Куда девать запрос, если CanExecute == false
            //Usually what we do is that we call the RaiseCanExecuteChanged method to force a revaluation of the CanExecute value.
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        /// <summary>
        /// В этом методе аргумент не используется и нужен только для реализации интерфейса
        /// </summary>
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        /// <summary>
        /// В этом методе аргумент не используется и нужен только для реализации интерфейса
        /// </summary>
        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(errorHandler);
        }
        #endregion
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool isExecuting;
        private readonly Func<T, Task> execute;
        private readonly Func<T, bool> canExecute;
        private readonly IErrorHandler errorHandler;

        public AsyncCommand(Func<T, Task> execute, Func<T, bool> canExecute = null, IErrorHandler errorHandler = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
            this.errorHandler = errorHandler;
        }

        public bool CanExecute(T parameter)
        {
            return !isExecuting && (canExecute?.Invoke(parameter) ?? true);
        }

        /// <summary>
        /// Этот метод публичный для команд, которые не используют связывание.
        /// Но лучше использовать метод "Execute", так как там есть обёртка с обработкой исключений
        /// </summary>
        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }

            //TODO: Куда девать запрос если CanExecute == false
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations
        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(errorHandler);
        }
        #endregion
    }
}
