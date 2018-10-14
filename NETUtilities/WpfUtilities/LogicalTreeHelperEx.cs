using System.Windows;

namespace WpfUtilities
{
    public static class LogicalTreeHelperEx
    {
        public static T LogicalAncestor<T>(this DependencyObject element) where T : DependencyObject
        {
            if (element is T variable)
            {
                return variable;
            }

            var parent = LogicalTreeHelper.GetParent(element);
            return parent?.LogicalAncestor<T>();
        }
    }
}