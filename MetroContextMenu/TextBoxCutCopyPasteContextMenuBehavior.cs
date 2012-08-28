using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using TriggerBase = System.Windows.Interactivity.TriggerBase;

namespace MetroContextMenu
{
    public class TextBoxCutCopyPasteContextMenuBehavior : Behavior<TextBox>
    {
        private readonly ContextMenu contextMenu;
        private readonly MenuItem copyMenuItem;
        private readonly MenuItem cutMenuItem;
        private readonly MenuItem pasteMenuItem;

        public TextBoxCutCopyPasteContextMenuBehavior()
        {
            contextMenu = new ContextMenu();
            //contextMenu.Style = (Style)Application.Current.Resources["MetroContextMenuStyle"];

            cutMenuItem = new MenuItem { Header = "Cut" };
            cutMenuItem.Click += CutClick;
            contextMenu.Items.Add(cutMenuItem);

            copyMenuItem = new MenuItem { Header = "Copy" };
            copyMenuItem.Click += CopyClick;
            contextMenu.Items.Add(copyMenuItem);

            pasteMenuItem = new MenuItem { Header = "Paste" };
            pasteMenuItem.Click += PasteClick;
            contextMenu.Items.Add(pasteMenuItem);
        }

        void PasteClick(object sender, RoutedEventArgs e)
        {
            AssociatedObject.SelectedText = Clipboard.GetText();
            contextMenu.IsOpen = false;
        }

        void CutClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(AssociatedObject.SelectedText);
            AssociatedObject.SelectedText = string.Empty;
            AssociatedObject.Focus();
            contextMenu.IsOpen = false;
        }

        void CopyClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(AssociatedObject.SelectedText);
            AssociatedObject.Focus();
            contextMenu.IsOpen = false;
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseRightButtonDown += AssociatedObject_MouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp += AssociatedObjectMouseRightButtonUp;
            AssociatedObject.SetValue(ContextMenuService.ContextMenuProperty, contextMenu);
            base.OnAttached();
        }

        void AssociatedObjectMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            pasteMenuItem.IsEnabled = Clipboard.ContainsText();

            if (string.IsNullOrEmpty(AssociatedObject.SelectedText))
            {
                cutMenuItem.IsEnabled = false;
                copyMenuItem.IsEnabled = false;
            }
            else
            {
                cutMenuItem.IsEnabled = true;
                copyMenuItem.IsEnabled = true;
            }

            contextMenu.IsOpen = true;
        }

        void AssociatedObject_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseRightButtonDown -= AssociatedObject_MouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp -= AssociatedObjectMouseRightButtonUp;
            base.OnDetaching();
        }
    }

    public class Behaviors : List<Behavior> { }
    public class Triggers : List<TriggerBase> { }
    public static class SupplementaryInteraction
    {
        public static Behaviors GetBehaviors(DependencyObject obj)
        {
            return (Behaviors)obj.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject obj, Behaviors value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors",
                                                typeof(Behaviors),
                                                typeof(SupplementaryInteraction),
                                                new PropertyMetadata(null, OnPropertyBehaviorsChanged));

        private static void OnPropertyBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behaviors = Interaction.GetBehaviors(d);
            foreach (var behavior in e.NewValue as Behaviors)
                behaviors.Add(behavior);
        }

        public static Triggers GetTriggers(DependencyObject obj)
        {
            return (Triggers)obj.GetValue(TriggersProperty);
        }

        public static void SetTriggers(DependencyObject obj, Triggers value)
        {
            obj.SetValue(TriggersProperty, value);
        }

        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("Triggers",
                                                typeof(Triggers),
                                                typeof(SupplementaryInteraction),
                                                new PropertyMetadata(null, OnPropertyTriggersChanged));

        private static void OnPropertyTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triggers = Interaction.GetTriggers(d);
            foreach (var trigger in e.NewValue as Triggers)
                triggers.Add(trigger);
        }
    }
}