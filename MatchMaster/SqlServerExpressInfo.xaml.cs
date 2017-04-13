using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for SqlServerExpressInfo.xaml
    /// </summary>
    public partial class SqlServerExpressInfo : Window
    {
        public SqlServerExpressInfo()
        {
            InitializeComponent();
            //this.MinHeight = App.ScreenHeight / 2;
            //this.Width = App.ScreenWidth / 2;
            //this.MinWidth = App.ScreenWidth / 3;
            this.Title = "MatchMaster";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
