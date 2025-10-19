using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace StorePOS.Client.Wpf.Behaviors
{
    /// <summary>
    /// Behavior that enables infinite scrolling for ScrollViewer controls.
    /// Triggers a command when the user scrolls near the bottom of the content.
    /// </summary>
    public class InfiniteScrollBehavior : Behavior<ScrollViewer>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(InfiniteScrollBehavior));

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(InfiniteScrollBehavior), 
                new PropertyMetadata(false));

        public static readonly DependencyProperty ThresholdProperty =
            DependencyProperty.Register(nameof(Threshold), typeof(double), typeof(InfiniteScrollBehavior), 
                new PropertyMetadata(100.0));

        /// <summary>
        /// Command to execute when scrolling near the bottom
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Indicates if data is currently being loaded (prevents multiple simultaneous loads)
        /// </summary>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// Distance from bottom (in pixels) at which to trigger loading
        /// </summary>
        public double Threshold
        {
            get => (double)GetValue(ThresholdProperty);
            set => SetValue(ThresholdProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.ScrollChanged += OnScrollChanged;
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.ScrollChanged -= OnScrollChanged;
            }
            base.OnDetaching();
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            if (scrollViewer == null || IsLoading || Command?.CanExecute(null) != true)
                return;

            // Calculate distance to bottom
            var distanceToBottom = scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset;

            // Trigger loading when within threshold distance of bottom
            if (distanceToBottom <= Threshold)
            {
                Command.Execute(null);
            }
        }
    }
}