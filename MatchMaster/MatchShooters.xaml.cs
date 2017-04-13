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
            MatchShootersGrid.PreviewMouseLeftButtonDown += MatchShootersGrid_PreviewMouseLeftButtonDown;


            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            _m = m;
            Refresh();
        }

        private void MatchShootersGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataGridCell c = sender as DataGridCell;

            if (c.IsEditing) return;

            if (!c.IsFocused) c.Focus();
            if (!c.IsSelected) c.IsSelected = true;
        }

        private void Refresh()
        {
            this.Title = "Set Match Participants - " + _m.ToString();

            var q = from s in _ctx.Shooters orderby s.Surname, s.FirstName select s;
            ShootersGrid.ItemsSource = q.ToList();

            LblAllShooters.Content = $"All Shooters ({q.Count()}):";

            //var ms = from s in _ctx.Matches.Include("MatchParticipations").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchParticipations orderby s.Shooter.Surname, s.Shooter.FirstName select s.Shooter;

            var mp = from s in _ctx.MatchParticipations.Where(x => x.MatchID == _m.MatchID)
                     orderby s.Shooter.Surname, s.Shooter.FirstName
                     select s;


            MatchShootersGrid.ItemsSource = mp.ToList();

            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(MatchShootersGrid.ItemsSource);
            cv.GroupDescriptions.Add(new PropertyGroupDescription("Posse"));
            
            LblMatchShooters.Content = $"Match Participants ({mp.Count()}):";
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
                MatchParticipation mp = new MatchParticipation()
                {
                    ShooterID = s.ShooterID,
                    MatchID = _m.MatchID
                };

                _ctx.MatchParticipations.Attach(mp);


                if (!_ctx.MatchParticipations.Any(x => x.Match.MatchID.Equals(_m.MatchID) && x.Shooter.ShooterID.Equals(s.ShooterID) ))
                    _ctx.MatchParticipations.Add(mp);


                //if (!_ctx.Matches.Include("MatchParticipations").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchParticipations.Contains(mp))
                //    _ctx.Matches.Include("MatchParticipations").FirstOrDefault(x => x.MatchID == _m.MatchID).MatchParticipations.Add(mp);

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

            List<int> selected_match_participations = new List<int>();
            foreach (MatchParticipation mp in MatchShootersGrid.SelectedItems) selected_match_participations.Add(mp.MatchParticipationId);

            foreach (int match_participation_id in selected_match_participations)
            {
                //var ms = _ctx.Shooters.FirstOrDefault(x => x.ShooterID == shooter_id);

                MatchParticipation mp = _ctx.MatchParticipations.Where(x => x.MatchParticipationId==match_participation_id ).First();

                _ctx.MatchParticipations.Remove(mp);
                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SwitchSpeedContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var s = MatchShootersGrid.SelectedItems;

            if ((s == null) || (s.Count.Equals(0))) return;

            foreach (MatchParticipation i in s)
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().IsSpeedTicket = !_ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().IsSpeedTicket;

            _ctx.SaveChanges();
            Refresh();
        }

        private void MatchShootersGrid_Drop(object sender, DragEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while (!(dep is DataGridCell) && (dep !=null) ) dep = VisualTreeHelper.GetParent(dep);

            if (dep == null) return;

            while((dep != null) && !(dep is DataGridRow)) dep = VisualTreeHelper.GetParent(dep);

            DataGridRow r = dep as DataGridRow;
        }

        private void MatchShootersGrid_DragEnter(object sender, DragEventArgs e)
        {
            if (MatchShootersGrid.SelectedItems.Count > 0) e.Effects = DragDropEffects.Move;
        }
    }
}
