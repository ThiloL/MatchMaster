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
            var q = from p in _ctx.Shooters.Include("MatchParticipations")
                    orderby p.Surname, p.FirstName select p;

                    //select new Shooter
                    //{
                    //    ShooterID = p.ShooterID,
                    //    FirstName = p.FirstName,
                    //    Surname = p.Surname,
                    //    Birthday = p.Birthday,
                    //    Nickname = p.Nickname,
                    //    WeaponClass = p.WeaponClass,
                    //    MatchParticipations = p.MatchParticipations
                    //};

            //var x = _ctx.Shooters
            //    .Include("ShootedMatches")
            //    .OrderBy(w => w.Surname)
            //    .OrderBy(y => y.FirstName)
            //    .OrderBy(z => z.ShootedMatches.OrderBy(p => p.Title))
            //    .ToList();

            ShootersGrid.ItemsSource = q.ToList();
        }
       

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Details.DataContext == null) return;

            Shooter s = (Details.DataContext as Shooter);

            if (MessageBox.Show($"Do you really want to delete this Shooter?\n\n{s.ToString()}", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _ctx.Shooters.Remove(_ctx.Shooters.First(x => x.ShooterID.Equals(s.ShooterID)));
                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Shooter s = new Shooter() { Surname = "new Shooter", FirstName = "new Shooter" };
            _ctx.Shooters.Add(s);
            _ctx.SaveChanges();
            Refresh();
            ShootersGrid.SelectedItem = s;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.ShooterDetailsBg.UpdateSources();
            _ctx.SaveChanges();
        }
    }
}
