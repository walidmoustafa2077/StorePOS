using System;
using System.Threading.Tasks;

namespace StorePOS.Client.Wpf.Commands
{
    /// <summary>
    /// A strongly-typed asynchronous implementation of the ICommand interface that executes async operations with typed parameters.
    /// This generic version provides compile-time type safety for command parameters while maintaining all the benefits
    /// of the non-generic AsyncRelayCommand.
    /// 
    /// <para>
    /// This command automatically manages execution state to prevent concurrent executions
    /// and provides proper CanExecute state management during async operations.
    /// The generic type parameter ensures that the command parameter is strongly typed,
    /// reducing runtime errors and improving code maintainability.
    /// </para>
    /// 
    /// <para>
    /// Key features:
    /// - Strong typing for command parameters (compile-time safety)
    /// - Automatic execution state management
    /// - Prevents concurrent executions of the same command
    /// - Automatic CanExecute state updates during execution
    /// - Exception handling with proper state cleanup
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the command parameter.</typeparam>
    /// <example>
    /// Usage with strongly-typed parameters:
    /// <code>
    /// public AsyncRelayCommand&lt;string&gt; LoadFileCommand { get; }
    /// public AsyncRelayCommand&lt;Customer&gt; SaveCustomerCommand { get; }
    /// 
    /// public MyViewModel()
    /// {
    ///     LoadFileCommand = new AsyncRelayCommand&lt;string&gt;(LoadFileAsync, CanLoadFile);
    ///     SaveCustomerCommand = new AsyncRelayCommand&lt;Customer&gt;(SaveCustomerAsync);
    /// }
    /// 
    /// private async Task LoadFileAsync(string fileName)
    /// {
    ///     if (string.IsNullOrEmpty(fileName)) return;
    ///     await _fileService.LoadAsync(fileName);
    /// }
    /// 
    /// private bool CanLoadFile(string fileName) => !string.IsNullOrEmpty(fileName);
    /// 
    /// private async Task SaveCustomerAsync(Customer customer)
    /// {
    ///     await _customerService.SaveAsync(customer);
    /// }
    /// </code>
    /// </example>
    public sealed class AsyncRelayCommand<T> : CommandBase
    {
        private readonly Func<T?, Task> _executeAsync;
        private readonly Predicate<T?>? _canExecute;
        private bool _isExecuting;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncRelayCommand{T}"/> class.
        /// </summary>
        /// <param name="executeAsync">
        /// The asynchronous method to execute when the command is invoked.
        /// This method receives a strongly-typed parameter of type <typeparamref name="T"/>.
        /// </param>
        /// <param name="canExecute">
        /// An optional predicate that determines whether the command can execute for a given parameter.
        /// If null, the command can always execute (when not already executing).
        /// The predicate receives a strongly-typed parameter of type <typeparamref name="T"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="executeAsync"/> is null.</exception>
        /// <remarks>
        /// The strong typing provided by this generic command helps catch parameter type mismatches
        /// at compile time rather than at runtime, improving code reliability and maintainability.
        /// </remarks>
        public AsyncRelayCommand(Func<T?, Task> executeAsync, Predicate<T?>? canExecute = null)
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
        /// This parameter will be cast to type <typeparamref name="T"/> if possible.
        /// </param>
        /// <returns>
        /// <c>true</c> if the command can execute; otherwise, <c>false</c>.
        /// Returns <c>false</c> if the command is currently executing or if the parameter cannot be cast to type <typeparamref name="T"/>.
        /// </returns>
        /// <remarks>
        /// This method performs type checking on the parameter before passing it to the canExecute predicate.
        /// If the parameter is not of type <typeparamref name="T"/> and cannot be converted to that type,
        /// the method returns false to prevent runtime errors.
        /// </remarks>
        public override bool CanExecute(object? parameter)
        {
            if (_isExecuting)
                return false;

            // Type safety check
            if (parameter is not T typedParameter && parameter is not null)
                return false;

            return _canExecute?.Invoke((T?)parameter) ?? true;
        }

        /// <summary>
        /// Executes the asynchronous command with the specified parameter.
        /// This method manages the execution state and ensures that the command cannot be executed concurrently.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to pass to the command execution method.
        /// This parameter will be cast to type <typeparamref name="T"/> before being passed to the execution method.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method performs the following steps:
        /// 1. Checks if the command can execute (calls CanExecute)
        /// 2. Performs type safety checks on the parameter
        /// 3. Sets the _isExecuting flag to true and raises CanExecuteChanged
        /// 4. Executes the async operation with the strongly-typed parameter
        /// 5. Resets the _isExecuting flag and raises CanExecuteChanged again
        /// </para>
        /// <para>
        /// If the parameter cannot be cast to type <typeparamref name="T"/>, the execution is aborted
        /// to prevent runtime type errors. This provides an additional layer of safety beyond
        /// the compile-time type checking.
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

            // Additional type safety check for execute
            if (parameter is not T typedParameter && parameter is not null)
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _executeAsync((T?)parameter);
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