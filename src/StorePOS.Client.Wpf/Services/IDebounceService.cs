using System.Collections.Concurrent;

namespace StorePOS.Client.Wpf.Services
{
    /// <summary>
    /// Interface for debouncing frequent operations to improve performance.
    /// Follows Single Responsibility Principle by handling only debouncing logic.
    /// </summary>
    public interface IDebounceService : IDisposable
    {
        /// <summary>
        /// Debounces an action by the specified delay.
        /// If the same key is called multiple times within the delay, only the last call executes.
        /// </summary>
        /// <param name="key">Unique key for the debounced action</param>
        /// <param name="action">The action to execute after debouncing</param>
        /// <param name="delay">The debounce delay</param>
        void Debounce(string key, Func<Task> action, TimeSpan delay);

        /// <summary>
        /// Cancels any pending debounced action for the specified key.
        /// </summary>
        /// <param name="key">The key of the action to cancel</param>
        void Cancel(string key);

        /// <summary>
        /// Cancels all pending debounced actions.
        /// </summary>
        void CancelAll();
    }

    /// <summary>
    /// Implementation of IDebounceService for managing debounced operations.
    /// Thread-safe implementation that prevents UI blocking during rapid user input.
    /// </summary>
    public class DebounceService : IDebounceService, IDisposable
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _pending = new();
        private bool _disposed;

        /// <inheritdoc />
        public void Debounce(string key, Func<Task> action, TimeSpan delay)
        {
            if (_disposed) return;

            // Cancel any existing operation for this key
            Cancel(key);

            var cts = new CancellationTokenSource();
            _pending[key] = cts;

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(delay, cts.Token);
                    
                    if (!cts.Token.IsCancellationRequested)
                    {
                        await action();
                    }
                }
                catch (OperationCanceledException)
                {
                    // Expected when debounced operation is cancelled
                }
                catch (Exception ex)
                {
                    // Log the exception in a real application
                    System.Diagnostics.Debug.WriteLine($"Debounced action failed: {ex.Message}");
                }
                finally
                {
                    _pending.TryRemove(key, out _);
                    cts.Dispose();
                }
            });
        }

        /// <inheritdoc />
        public void Cancel(string key)
        {
            if (_pending.TryRemove(key, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }
        }

        /// <inheritdoc />
        public void CancelAll()
        {
            foreach (var kvp in _pending.ToArray())
            {
                Cancel(kvp.Key);
            }
        }

        /// <summary>
        /// Disposes all pending operations and releases resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            
            _disposed = true;
            CancelAll();
        }
    }
}