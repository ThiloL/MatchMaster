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

using GongSolutions.Wpf.DragDrop;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for MatchShooters.xaml
    /// </summary>
    public partial class MatchShooters : Window, IDropTarget
    {
        private MatchMasterContext _ctx = new MatchMasterContext();
        private Match _m;

        private Control FocusedGrid = null;

        //private List<Shooter> selected_shooters = new List<Shooter>();
        //private List<MatchParticipation> selected_matchparticipations = new List<MatchParticipation>();

        public MatchShooters(Match m)
        {
            InitializeComponent();

            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(MatchShootersGrid, this);
            GongSolutions.Wpf.DragDrop.DragDrop.SetDropHandler(SpeedTicketGrid, this);

            MatchShootersGrid.GotFocus += (sender, e) => { FocusedGrid = (Control)sender; };
            SpeedTicketGrid.GotFocus += (sender, e) => { FocusedGrid = (Control)sender; };

            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            _m = m;
            Refresh();
        }

        private void Refresh()
        {
            this.Title = "Set Match Participants - " + _m.ToString();

            // === Shooter List ===

            var q = from s in _ctx.Shooters orderby s.Surname, s.FirstName select s;
            ShootersGrid.ItemsSource = q.ToList();

            LblAllShooters.Content = $"All Shooters ({q.Count()}):";

            // === Regular Participants List ===

            var mp = from s in _ctx.MatchParticipations.Where(x => (x.MatchID == _m.MatchID) && !(x.IsSpeedTicket))
                     orderby s.Shooter.Surname, s.Shooter.FirstName
                     select s;

            var pgd = new PropertyGroupDescription("Posse");

            List<int> all_posse_id = new List<int>();
            for (int i = 1; i <= Global.CurrentMatch.NumberOfPosses; i++) pgd.GroupNames.Add(i);

            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(mp.ToList());
            cv.GroupDescriptions.Add(pgd);

            MatchShootersGrid.ItemsSource = cv;
            
            LblMatchShooters.Content = $"Match Participants ({mp.Count()}):";

            // === Speed Ticket Participants List ===

            var sp = from s in _ctx.MatchParticipations.Where(x => (x.MatchID == _m.MatchID) && (x.IsSpeedTicket) )
                     orderby s.Shooter.Surname, s.Shooter.FirstName
                     select s;

            SpeedTicketGrid.ItemsSource = sp.ToList();

            LblSpeedTickets.Content = $"Speed Ticket ({sp.Count()}):";
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
                    MatchID = _m.MatchID,
                    Category = s.Category
                };

                _ctx.MatchParticipations.Attach(mp);


                if (!_ctx.MatchParticipations.Any(x => x.Match.MatchID.Equals(_m.MatchID) && x.Shooter.ShooterID.Equals(s.ShooterID) ))
                    _ctx.MatchParticipations.Add(mp);

                _ctx.SaveChanges();
                Refresh();
            }
        }

        private void BtnRemoveFromMatch_Click(object sender, RoutedEventArgs e)
        {
            if (FocusedGrid == null) return;
            if (!(FocusedGrid is DataGrid)) return;

            var selected_items = ((DataGrid)FocusedGrid).SelectedItems;

            if (selected_items == null) return;
            if (selected_items.Count.Equals(0)) return;

            List<int> selected_match_participations_ids = new List<int>();
            foreach (MatchParticipation mp in selected_items) selected_match_participations_ids.Add(mp.MatchParticipationId);

            foreach (int match_participation_id in selected_match_participations_ids)
            {
                MatchParticipation mp = _ctx.MatchParticipations.Where(x => x.MatchParticipationId == match_participation_id).First();

                _ctx.MatchParticipations.Remove(mp);
                _ctx.SaveChanges();
                Refresh();
            }

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// make speed ticket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddSpeedTicket_Click(object sender, RoutedEventArgs e)
        {
            var s = MatchShootersGrid.SelectedItems;

            if ((s == null) || (s.Count.Equals(0))) return;

            foreach (MatchParticipation i in s)
            {
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().IsSpeedTicket = true;
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().Posse = 0;
            }

            _ctx.SaveChanges();
            Refresh();
        }

        /// <summary>
        /// disable speed ticket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveSpeedTicket_Click(object sender, RoutedEventArgs e)
        {
            var s = SpeedTicketGrid.SelectedItems;

            if ((s == null) || (s.Count.Equals(0))) return;

            foreach (MatchParticipation i in s)
            {
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().IsSpeedTicket = false;
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == i.MatchParticipationId).First().Posse = 0;
            }

            _ctx.SaveChanges();
            Refresh();
        }

        private void SwitchMatchDQ_Click(object sender, RoutedEventArgs e)
        {
            if (FocusedGrid == null) return;
            if (!(FocusedGrid is DataGrid)) return;

            var selected_items = ((DataGrid)FocusedGrid).SelectedItems;

            if (selected_items == null) return;
            if (selected_items.Count.Equals(0)) return;


            foreach (MatchParticipation mp in selected_items)
            {
                _ctx.MatchParticipations.Where(x => x.MatchParticipationId == mp.MatchParticipationId).First().IsMatchDQ = !_ctx.MatchParticipations.Where(x => x.MatchParticipationId == mp.MatchParticipationId).First().IsMatchDQ;
            }

            _ctx.SaveChanges();
            Refresh();

        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
            dropInfo.Effects = DragDropEffects.Link;

            dropInfo.NotHandled = true;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.VisualTarget is DataGrid)) return;

            if (dropInfo.TargetGroup == null)
            {
                HandleEx(dropInfo);
                return;
            }

            int target_posse_id;
            string target_group_name = dropInfo.TargetGroup.Name.ToString();

            if (Int32.TryParse(target_group_name, out target_posse_id))
            {
                var shooters = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<Shooter>().ToList();
                var match_participants = DefaultDropHandler.ExtractData(dropInfo.Data).OfType<MatchParticipation>().ToList();

                if ((shooters != null) && (shooters.Count > 0))
                {
                    foreach (Shooter s in shooters)
                    {
                        MatchParticipation mp = new MatchParticipation()
                        {
                            ShooterID = s.ShooterID,
                            MatchID = _m.MatchID,
                            Posse = (int)target_posse_id
                        };

                        _ctx.MatchParticipations.Attach(mp);

                        if (!_ctx.MatchParticipations.Any(x => x.Match.MatchID.Equals(_m.MatchID) && x.Shooter.ShooterID.Equals(s.ShooterID))) _ctx.MatchParticipations.Add(mp);

                        _ctx.SaveChanges();
                    }

                }
                else if ((match_participants != null) && (match_participants.Count > 0))
                {
                    foreach (MatchParticipation mp in match_participants)
                    {
                        _ctx.MatchParticipations.Where(x => x.MatchParticipationId.Equals(mp.MatchParticipationId)).First().Posse = (int)target_posse_id;
                    }

                    _ctx.SaveChanges();

                }

                Refresh();
            }

        }

        void HandleEx(IDropInfo di)
        {
            DataGrid t = di.VisualTarget as DataGrid;

            var shooters = DefaultDropHandler.ExtractData(di.Data).OfType<Shooter>().ToList();
            if ((shooters != null) && (shooters.Count > 0))
            {
                foreach (Shooter s in shooters)
                {
                    MatchParticipation mp = new MatchParticipation()
                    {
                        ShooterID = s.ShooterID,
                        MatchID = _m.MatchID,
                        Posse = 0
                    };

                    if (t.Name.Equals("SpeedTicketGrid")) mp.IsSpeedTicket = true;

                    _ctx.MatchParticipations.Attach(mp);

                    if (!_ctx.MatchParticipations.Any(x => x.Match.MatchID.Equals(_m.MatchID) && x.Shooter.ShooterID.Equals(s.ShooterID))) _ctx.MatchParticipations.Add(mp);

                    _ctx.SaveChanges();
                }
                Refresh();
                return;
            }
        }

    }
}
