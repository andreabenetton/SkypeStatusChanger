using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace SkypeStatusChanger.Extensions
{
    internal class HyperlinkExtensions
    {
        public static bool GetIsExternal(DependencyObject obj)
        {
            return (bool)obj.GetValue(IS_EXTERNAL_PROPERTY);
        }

        public static void SetIsExternal(DependencyObject obj, bool value)
        {
            obj.SetValue(IS_EXTERNAL_PROPERTY, value);
        }

        /// <summary>
        /// If setted to "True" hyperlink became live, i.e. opens page in the browser.
        /// </summary>
        public static readonly DependencyProperty IS_EXTERNAL_PROPERTY =
            DependencyProperty.RegisterAttached("IsExternal", typeof(bool), typeof(HyperlinkExtensions), new PropertyMetadata(IsExternalChanged));

        private static void IsExternalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var hyperlink = d as Hyperlink;
            if (hyperlink == null) return;

            if ((bool) e.NewValue)
                hyperlink.RequestNavigate += HyperlinkRequestNavigate;
            else
                hyperlink.RequestNavigate -= HyperlinkRequestNavigate;
        }

        private static void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}
