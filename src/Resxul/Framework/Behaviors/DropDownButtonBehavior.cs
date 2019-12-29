using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Resxul.Framework.Behaviors
{
    public class DropDownButtonBehavior : Behavior<Button>
    {
        private bool _isContextMenuOpen;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
        }

        void AssociatedObject_Click(object sender, RoutedEventArgs e)
        {
            Button source = sender as Button;
            if (source?.ContextMenu != null)
            {
                if (!_isContextMenuOpen)
                {
                    source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    source.ContextMenu.PlacementTarget = source;
                    source.ContextMenu.Placement = PlacementMode.Bottom;
                    source.ContextMenu.IsOpen = true;
                    _isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _isContextMenuOpen = false;
            var contextMenu = sender as ContextMenu;
            contextMenu?.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
        }
    }
}
