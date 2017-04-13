﻿using System;
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

using System.Data.Entity;

using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Core.Objects;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for Match.xaml
    /// </summary>
    public partial class MatchWindow : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();

        public MatchWindow()
        {
            InitializeComponent();
            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            Refresh();
        }

        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            Match m = new Match() { Title = "new Match", NumberOfPosses=1, NumberOfStages=1 };

            _ctx.Matches.Add(m);
            _ctx.SaveChanges();

            Refresh();
            matchDataGrid.SelectedItem = m;

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.MatchDetailsBg.UpdateSources();
            _ctx.SaveChanges();
        }

        private void Refresh()
        {
            if (_ctx.Matches == null) return;
            if (_ctx.Matches.Count().Equals(0)) return;

            var q = from p in _ctx.Matches.Include("MatchParticipations") orderby p.MatchID descending select p;
            matchDataGrid.ItemsSource = q.ToList();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (Details.DataContext == null) return;

            Match m = (Details.DataContext as Match);

            if (MessageBox.Show($"Do you really want to delete this match?\n\n{m.ToString()}","Confirmation",MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _ctx.Matches.Remove(_ctx.Matches.First(x => x.MatchID.Equals(m.MatchID)));
                _ctx.SaveChanges();
                Refresh();

            }

        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (Details.DataContext == null) return;

            Match m = (Details.DataContext as Match);
            Global.CurrentMatch = m;
            (Application.Current.MainWindow as MainWindow).SetTitle();

            this.Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
