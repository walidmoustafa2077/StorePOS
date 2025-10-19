using System;
using System.Threading.Tasks;

namespace StorePOS.Client.Wpf.Commands
{
    /// <summary>
    /// An asynchronous implementation of the ICommand interface that executes async operations.
    /// This command automatically manages execution state to prevent concurrent executions
    /// and provides proper CanExecute state management during async operations.
    /// 
    /// <para>
    /// The AsyncRelayCommand is designed for scenarios where the command execution involves
    /// asynchronous operations such as API calls, file I/O, or database operations.
    /// It ensures that the command cannot be executed again while a previous execution is still running.
    /// </para>
    /// 
    /// <para>
    /// Key features:
    /// - Automatic execution state management (_isExecuting flag)
    /// - Prevents concurrent executions of the same command
    /// - Automatic CanExecute state updates during execution
    /// - Support for both parameterized and non-parameterized commands
    /// - Exception handling with proper state cleanup
    /// </para>
    /// </summary>
    /// <example>
    /// Usage without parameters:
    /// <code>
    /// public AsyncRelayCommand SaveCommand { get; }
    /// 
    /// public MyViewModel()
    /// {
    ///     SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
    /// }
    /// 
    /// private async Task SaveAsync()
    /// {
    ///     // Perform save operation
    ///     await _dataService.SaveAsync();
    /// }
    /// 
    /// private bool CanSave() => !string.IsNullOrEmpty(Name);
    /// </code>
    /// 
    /// Usage with parameters:
    /// <code>
    /// public AsyncRelayCommand&lt;string&gt; LoadCommand { get; }
    /// 
    /// public MyViewModel()
    /// {
    ///     LoadCommand = new AsyncRelayCommand&lt;string&gt;(LoadAsync);
    /// }
    /// 
    /// private async Task LoadAsync(string fileName)
    /// {
    ///     // Perform load operation
    ///     await _dataService.LoadAsync(fileName);
    /// }
    /// </code>
    /// </example>
    public sealed class AsyncRelayCommand : CommandBase
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Predicate<object?>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class with a parameterless execute method.
        /// This constructor is convenient for commands that don't need parameters.
        /// </summary>
        /// <param name="executeAsync">
        /// The asynchronous method to execute when the command is invoked.
        /// This method will be called without parameters.
        /// </param>
        /// <param name="canExecute">
        /// An optional method that determines whether the command can execute.
        /// If null, the command can always execute (when not already executing).
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executeAsync"/> is null.</exception>
        /// <remarks>
        /// This constructor internally converts the parameterless delegates to parameterized ones
        /// for consistency with the main constructor.
        /// </remarks>
        public AsyncRelayCommand(Func<Task> executeAsync, Func<bool>? canExecute = null)
        {
            if (executeAsync == null)
                throw new ArgumentNullException(nameof(executeAsync));

            _executeAsync = _ => executeAsync();
            if (canExecute is not null)
                _canExecute = _ => canExecute();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand"/> class with a parameterized execute method.
        /// This is the primary constructor that supports command parameters.
        /// </summary>
        /// <param name="executeAsync">
        /// The asynchronous method to execute when the command is invoked.
        /// This method receives the command parameter as its argument.
        /// </param>
        /// <param name="canExecute">
        /// An optional predicate that determines whether the command can execute for a given parameter.
        /// If null, the command can always execute (when not already executing).
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executeAsync"/> is null.</exception>
        /// <remarks>
        /// The command parameter is passed through to both the execute and canExecute methods,
        /// allowing for parameter-dependent execution logic.
        /// </remarks>
        public AsyncRelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Determines whether the command can execute with the specified parameter.
        /// The command cannot execute if it is currently executing or if the custom canExecute predicate returns false.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to pass to the canExecute predicate.
        /// This is the same parameter that would be passed to the Execute method.
        /// </param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>.
        /// Returns <c>false</c> if the command is currently executing, regardless of other conditions.
        /// </returns>
        /// <remarks>
        /// This method is called by the WPF command system to determine the enabled state of UI elements
        /// bound to this command. The method automatically returns false when the command is executing
        /// to prevent concurrent executions.
        /// </remarks>
        public override bool CanExecute(object? parameter) =>
            !_isExecuting && (_canExecute?.Invoke(parameter) ?? true);

        /// <summary>
        /// Executes the asynchronous command with the specified parameter.
        /// This method manages the execution state and ensures that the command cannot be executed concurrently.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to pass to the command execution method.
        /// This can be null if the command doesn't require a parameter.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method follows these steps:
        /// 1. Checks if the command can execute (calls CanExecute)
        /// 2. Sets the _isExecuting flag to true and raises CanExecuteChanged
        /// 3. Executes the async operation
        /// 4. Resets the _isExecuting flag and raises CanExecuteChanged again
        /// </para>
        /// <para>
        /// The execution state management ensures that:
        /// - UI elements bound to this command are automatically disabled during execution
        /// - Multiple concurrent executions are prevented
        /// - The execution state is properly reset even if an exception occurs
        /// </para>
        /// <para>
        /// Note: This method is marked as async void because it implements the ICommand.Execute method
        /// which has a void signature. Exceptions that occur during execution will be handled by
        /// the application's global exception handling.
        /// </para>
        /// </remarks>
        public override async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) 
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the command is currently executing an asynchronous operation.
        /// This property can be used for additional state management in ViewModels.
        /// </summary>
        /// <value>
        /// <c>true</c> if the command is currently executing; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This property is useful for binding to loading indicators or other UI elements
        /// that should show different states during command execution.
        /// </remarks>
        public bool IsExecuting => _isExecuting;
    }
}
