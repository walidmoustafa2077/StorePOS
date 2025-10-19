using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace StorePOS.Client.Wpf.Behaviors
{
    /// <summary>
    /// Behavior that automatically selects all text in a TextBox when it receives focus.
    /// Handles both keyboard focus (Tab navigation) and mouse click scenarios.
    /// </summary>
    public class SelectAllTextOnFocusBehavior : Behavior<TextBox>
    {
        /// <summary>
        /// Called when the behavior is attached to a TextBox
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotKeyboardFocus += OnGotKeyboardFocus;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
        }

        /// <summary>
        /// Called when the behavior is detached from a TextBox
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.GotKeyboardFocus -= OnGotKeyboardFocus;
                AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            }
        }

        /// <summary>
        /// Handles keyboard focus (e.g., Tab navigation)
        /// </summary>
        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Handles mouse click focus - ensures text is selected even when clicking
        /// </summary>
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                // If the textbox is not focused, focus it and select all
                textBox.Focus();
                e.Handled = true;
            }
        }
    }
}
