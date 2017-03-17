using System;
using System.Collections.Generic;
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
    /// Interaction logic for Shooter.xaml
    /// </summary>
    public partial class ShooterWindow : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();

        public ShooterWindow()
        {
            InitializeComponent();
            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            Refresh();
        }

        private void Refresh()
        {
            var q = from p in _ctx.Shooters orderby p.Surname, p.FirstName select p;
            ShootersGrid.ItemsSource = q.ToList();
        }

        private void ShootersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
