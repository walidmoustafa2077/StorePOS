namespace StorePOS.Client.Wpf.Commands
{
    public sealed class RelayCommand : CommandBase
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = _ => execute();
            if (canExecute is not null)
                _canExecute = _ => canExecute();
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public override bool CanExecute(object? parameter) =>
            _canExecute?.Invoke(parameter) ?? true;

        public override void Execute(object? parameter) =>
            _execute(parameter);
    }
}
