using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace StorePOS.Client.Wpf.MVVM.ViewModels
{
    /// <summary>
    /// Abstract base class for all ViewModels in the StorePOS application.
    /// Implements INotifyPropertyChanged for data binding support and IDisposable for resource management.
    /// Provides common functionality for property change notifications, busy state management, and cleanup.
    /// 
    /// <para>
    /// This class follows the MVVM pattern and provides a foundation for all ViewModels in the application.
    /// It includes built-in support for:
    /// - Property change notifications via INotifyPropertyChanged
    /// - Busy state management for asynchronous operations
    /// - Title property for view titles
    /// - Proper resource cleanup via IDisposable
    /// </para>
    /// 
    /// <example>
    /// Example usage in a derived ViewModel:
    /// <code>
    /// public class ExampleViewModel : ViewModelBase
    /// {
    ///     private string _name = string.Empty;
    ///     
    ///     public string Name
    ///     {
    ///         get => _name;
    ///         set => SetProperty(ref _name, value);
    ///     }
    ///     
    ///     protected override void Dispose(bool disposing)
    ///     {
    ///         if (disposing)
    ///         {
    ///             // Dispose managed resources
    ///         }
    ///         base.Dispose(disposing);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// This event is used by the WPF data binding system to update the UI when properties change.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isBusy;
        private bool _disposed;

        /// <summary>
        /// Gets or sets a value indicating whether the ViewModel is currently performing a busy operation.
        /// This property is typically used to show loading indicators in the UI.
        /// When set to true, UI elements can disable themselves to prevent user interaction during operations.
        /// </summary>
        /// <value>
        /// <c>true</c> if the ViewModel is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string? _title;

        /// <summary>
        /// Gets or sets the title of the view associated with this ViewModel.
        /// This property is commonly bound to window titles, page headers, or navigation elements.
        /// </summary>
        /// <value>
        /// The title string, or <c>null</c> if no title is set.
        /// </value>
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for the specified property.
        /// This method is called automatically by <see cref="SetProperty{T}"/> but can also be called manually
        /// when property changes cannot be handled by the SetProperty method.
        /// </summary>
        /// <param name="propertyName">
        /// The name of the property that changed. This parameter is automatically provided
        /// when called from a property setter using the <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        /// <remarks>
        /// The <see cref="CallerMemberNameAttribute"/> ensures that the property name is automatically
        /// provided when this method is called from a property setter, reducing the chance of errors
        /// and improving maintainability.
        /// </remarks>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Sets the value of a field and raises the <see cref="PropertyChanged"/> event if the value has changed.
        /// This method provides a standardized way to implement property setters with change notification.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="field">A reference to the field that stores the property value.</param>
        /// <param name="value">The new value to set.</param>
        /// <param name="propertyName">
        /// The name of the property. This parameter is automatically provided
        /// when called from a property setter using the <see cref="CallerMemberNameAttribute"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if the property value was changed; <c>false</c> if the new value
        /// was equal to the existing value.
        /// </returns>
        /// <remarks>
        /// This method uses <see cref="object.Equals(object, object)"/> to compare values,
        /// which works correctly with null values and reference equality.
        /// </remarks>
        /// <example>
        /// Typical usage in a property setter:
        /// <code>
        /// private string _name = string.Empty;
        /// public string Name
        /// {
        ///     get => _name;
        ///     set => SetProperty(ref _name, value);
        /// }
        /// </code>
        /// </example>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) 
                return false;
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// This method implements the Dispose pattern for proper resource cleanup.
        /// </summary>
        /// <remarks>
        /// This method is safe to call multiple times. Subsequent calls after the first will have no effect.
        /// The implementation follows the standard Dispose pattern with a protected virtual Dispose method
        /// that derived classes can override to perform their own cleanup.
        /// </remarks>
        public void Dispose()
        {
            if (_disposed) 
                return;
            
            Dispose(true);
            GC.SuppressFinalize(this);
            _disposed = true;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="ViewModelBase"/> and optionally releases the managed resources.
        /// This method is called by the public <see cref="Dispose()"/> method and can be overridden in derived classes
        /// to provide custom cleanup logic.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources;
        /// <c>false</c> to release only unmanaged resources.
        /// When called from the finalizer, this parameter is <c>false</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Override this method in derived classes to dispose of resources specific to the derived class.
        /// Always call the base implementation to ensure proper cleanup of base class resources.
        /// </para>
        /// <para>
        /// When overriding this method:
        /// - Only dispose managed resources when <paramref name="disposing"/> is <c>true</c>
        /// - Always dispose unmanaged resources regardless of the <paramref name="disposing"/> parameter
        /// - Call the base class implementation at the end of your override
        /// </para>
        /// </remarks>
        /// <example>
        /// Example override in a derived class:
        /// <code>
        /// protected override void Dispose(bool disposing)
        /// {
        ///     if (disposing)
        ///     {
        ///         // Dispose managed resources
        ///         _timer?.Dispose();
        ///         _cancellationTokenSource?.Dispose();
        ///     }
        ///     
        ///     // Dispose unmanaged resources here if any
        ///     
        ///     base.Dispose(disposing);
        /// }
        /// </code>
        /// </example>
        protected virtual void Dispose(bool disposing)
        {
            // Base implementation - override in derived classes for specific cleanup
            // No resources to dispose in the base class currently
        }
    }
}
