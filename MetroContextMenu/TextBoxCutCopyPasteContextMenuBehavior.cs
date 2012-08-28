using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

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
}