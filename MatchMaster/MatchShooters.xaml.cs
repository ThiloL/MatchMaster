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
    /// Interaction logic for MatchShooters.xaml
    /// </summary>
    public partial class MatchShooters : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();
        private Match _m;

        public MatchShooters(Match m)
        {
            InitializeComponent();
            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            _m = m;
            Refresh();
        }

        private void Refresh()
        {
            this.Title = "Set Match Shooters - " + _m.ToString();

            var q = from s in _ctx.Shooters orderby s.Surname, s.FirstName select s;
            ShootersGrid.ItemsSource = q.ToList();

            LblAllShooters.Content = $"All Shooters ({q.Count()}):";

            var ms = from s in _ctx.Matches.Include("MatchShooters").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchShooters orderby s.Surname, s.FirstName select s;
            MatchShootersGrid.ItemsSource = ms.ToList();

            LblMatchShooters.Content = $"Match Shooters ({ms.Count()}):";
        }

        private void BtnAddToMatch_Click(object sender, RoutedEventArgs e)
        {
            if (ShootersGrid.SelectedItems == null)
            {
                MessageBox.Show("Please select a Shooter first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach(Shooter s in ShootersGrid.SelectedItems)
            {
                if (!_ctx.Matches.Include("MatchShooters").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchShooters.Contains(s)) _ctx.Matches.Include("MatchShooters").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchShooters.Add(s);
                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnRemoveFromMatch_Click(object sender, RoutedEventArgs e)
        {
            if (MatchShootersGrid.SelectedItems == null)
            {
                MessageBox.Show("Please select a Match Shooter first.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<int> selected_match_shooters = new List<int>();
            foreach (Shooter s in MatchShootersGrid.SelectedItems) selected_match_shooters.Add(s.ShooterID);

            foreach (int shooter_id in selected_match_shooters)
            {
                var ms = _ctx.Shooters.FirstOrDefault(x => x.ShooterID == shooter_id);
                _ctx.Matches.Include("MatchShooters").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchShooters.Remove(ms);
                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
