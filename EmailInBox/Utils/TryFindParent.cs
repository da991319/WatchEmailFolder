
using System.Windows;
using System.Windows.Media;

namespace EmailInBox.Utils
{
    public static class TryFindParent
    {
        public static T Search<T>(DependencyObject current) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(current);

            if (parent == null) return null;

            if (parent is T) return parent as T;
            return Search<T>(parent);
        }
    }
}
