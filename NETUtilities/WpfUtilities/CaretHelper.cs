using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfUtilities
{
    /// <summary>
    /// Provides 
    /// </summary>
    public static class CaretHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Position
        {
            public int X;
            public int Y;

            public Position(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(ref Position p);

        /// <summary>
        /// Get the caret position relative to screen.
        /// </summary>
        public static Point GetCaretPosition()
        {
            Position position = new Position();
            GetCaretPos(ref position);

            if (Keyboard.FocusedElement is DependencyObject dependencyObject)
            {
                var popupRoot = dependencyObject.LogicalAncestor<Popup>()?.Child;
                var container = popupRoot ?? Window.GetWindow(dependencyObject);

                if (container != null)
                {
                    return container.PointToScreen(new Point(position.X, position.Y));
                }
            }

            return new Point(position.X, position.Y);
        }
    }
}