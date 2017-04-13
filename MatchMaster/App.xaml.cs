using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

using System.Data.Entity;
using System.Windows.Controls;
using System.Windows.Input;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int ScreenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        public static int ScreenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;

        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));

            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.PreviewMouseLeftButtonDownEvent,
                new MouseButtonEventHandler(SelectivelyHandleMouseButton));


            base.OnStartup(e);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var t = sender as TextBox;

            if (t == null) return;
            if (t.IsReadOnly) return;

            t.SelectAll();
        }

        private void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var t = sender as TextBox;

            if (t == null) return;
            if (t.IsKeyboardFocusWithin) return;

            if (e.OriginalSource.GetType().Name == "TextBoxView")
            {
                e.Handled = true;
                t.Focus();
            }
        }


    }
}
