using System.Windows;
using System.Windows.Input;

namespace StorePOS.Client.Wpf.Helpers
{
    /// <summary>
    /// Helper class that provides attached properties for focus management.
    /// Allows clicking on background elements to remove focus from input controls.
    /// </summary>
    public static class FocusHelper
    {
        #region ClickToDefocus Attached Property

        /// <summary>
        /// Attached property that enables click-to-defocus behavior on an element.
        /// When this property is set to true, clicking on the element will move focus to itself,
        /// effectively removing focus from any previously focused controls.
        /// </summary>
        public static readonly DependencyProperty ClickToDefocusProperty =
            DependencyProperty.RegisterAttached(
                "ClickToDefocus",
                typeof(bool),
                typeof(FocusHelper),
                new PropertyMetadata(false, OnClickToDefocusChanged));

        /// <summary>
        /// Gets the value of the ClickToDefocus attached property for the specified element.
        /// </summary>
        /// <param name="element">The element to get the property value for.</param>
        /// <returns>True if click-to-defocus is enabled, false otherwise.</returns>
        public static bool GetClickToDefocus(DependencyObject element)
        {
            return (bool)element.GetValue(ClickToDefocusProperty);
        }

        /// <summary>
        /// Sets the value of the ClickToDefocus attached property for the specified element.
        /// </summary>
        /// <param name="element">The element to set the property value for.</param>
        /// <param name="value">True to enable click-to-defocus, false to disable.</param>
        public static void SetClickToDefocus(DependencyObject element, bool value)
        {
            element.SetValue(ClickToDefocusProperty, value);
        }

        /// <summary>
        /// Called when the ClickToDefocus property changes.
        /// Attaches or detaches the mouse down event handler.
        /// </summary>
        /// <param name="d">The element that the property changed on.</param>
        /// <param name="e">Event arguments containing the old and new values.</param>
        private static void OnClickToDefocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    // Enable click-to-defocus behavior
                    element.MouseDown += OnElementMouseDown;
                    element.Focusable = true;
                    element.FocusVisualStyle = null; // Remove focus visual style
                }
                else
                {
                    // Disable click-to-defocus behavior
                    element.MouseDown -= OnElementMouseDown;
                }
            }
        }

        /// <summary>
        /// Handles the mouse down event for elements with click-to-defocus enabled.
        /// Moves focus to the clicked element, removing focus from any input controls.
        /// </summary>
        /// <param name="sender">The element that was clicked.</param>
        /// <param name="e">Mouse event arguments.</param>
        private static void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Move focus to this element to defocus any input controls
                element.Focus();
                
                // Mark the event as handled to prevent it from bubbling up
                // Only if the clicked element is the target (not a child element)
                if (e.OriginalSource == element)
                {
                    e.Handled = true;
                }
            }
        }

        #endregion

        #region FocusOnLoad Attached Property

        /// <summary>
        /// Attached property that automatically sets focus to an element when it's loaded.
        /// Useful for setting initial focus to specific controls like username textbox.
        /// </summary>
        public static readonly DependencyProperty FocusOnLoadProperty =
            DependencyProperty.RegisterAttached(
                "FocusOnLoad",
                typeof(bool),
                typeof(FocusHelper),
                new PropertyMetadata(false, OnFocusOnLoadChanged));

        /// <summary>
        /// Gets the value of the FocusOnLoad attached property for the specified element.
        /// </summary>
        /// <param name="element">The element to get the property value for.</param>
        /// <returns>True if focus-on-load is enabled, false otherwise.</returns>
        public static bool GetFocusOnLoad(DependencyObject element)
        {
            return (bool)element.GetValue(FocusOnLoadProperty);
        }

        /// <summary>
        /// Sets the value of the FocusOnLoad attached property for the specified element.
        /// </summary>
        /// <param name="element">The element to set the property value for.</param>
        /// <param name="value">True to enable focus-on-load, false to disable.</param>
        public static void SetFocusOnLoad(DependencyObject element, bool value)
        {
            element.SetValue(FocusOnLoadProperty, value);
        }

        /// <summary>
        /// Called when the FocusOnLoad property changes.
        /// Attaches or detaches the loaded event handler.
        /// </summary>
        /// <param name="d">The element that the property changed on.</param>
        /// <param name="e">Event arguments containing the old and new values.</param>
        private static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.Loaded += OnElementLoaded;
                }
                else
                {
                    element.Loaded -= OnElementLoaded;
                }
            }
        }

        /// <summary>
        /// Handles the loaded event for elements with focus-on-load enabled.
        /// Sets focus to the element once it's fully loaded.
        /// </summary>
        /// <param name="sender">The element that was loaded.</param>
        /// <param name="e">Routed event arguments.</param>
        private static void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                // Use dispatcher to ensure the element is fully rendered
                element.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    element.Focus();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        #endregion
    }
}