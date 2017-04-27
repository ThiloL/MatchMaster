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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;
using System.Data.SqlLocalDb;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();
        private TFH tfh = new TFH();

        //ShooterWindow w;
        //MatchWindow mw;
        //MatchShooters ms;
        //PrintStuff ps;
        //CategoryWindow cw;

        public MainWindow()
        {
            InitializeComponent();
            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;
            if (!CheckSqlServer()) HandleSqlProblem();
            LoadLastmatchIfRequired();
            SetTitle();
        }

        private void HandleSqlProblem()
        {
            SqlServerExpressInfo i = new SqlServerExpressInfo();
            i.ShowDialog();
            this.Shutdown();
        }

        private void LoadLastmatchIfRequired()
        {
            int last_match_id = -1;

            try
            {
                last_match_id = (int)Properties.Settings.Default.LastMatchId;
            }
            catch
            {
                return;
            }


            // Match exists?
            if (!_ctx.Matches.Any(x => x.MatchID == last_match_id)) return;

            Match last_match = _ctx.Matches.First(x => x.MatchID == last_match_id);

            // ask
            if (MessageBox.Show($"Do you want to select last Match\n\n{last_match.ToString()}?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) Global.CurrentMatch = last_match;
        }

        public void SetTitle()
        {
            if (Global.CurrentMatch == null)
            {
                this.Title = Global.Product;
                return;
            }
            this.BtnPrintMenu.IsEnabled = true;
            this.BtnSetPart.IsEnabled = true;
            this.Title = $"{Global.Product} - " + Global.CurrentMatch.ToString();
        }

        private bool CheckSqlServer()
        {
            try
            {
                ISqlLocalDbProvider provider = new SqlLocalDbProvider();
                ISqlLocalDbInstance instance = provider.GetOrCreateInstance(Global.Product);
                instance.Start();

            } catch (Exception e)
            {
                MessageBox.Show("LocalDB Instance could not be started:\n\n" + e.Message, $"{Global.Product}", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            
            // create Database and Files if MDF not exists
            if (!File.Exists(Global.DatabaseMdfPath())) DbCreator.Create(Global.Product, Global.DatabaseFolder());

            using (SqlConnection c = new SqlConnection(Properties.Settings.Default.SQLEXPRESS))
            {
                try
                {
                    c.Open();
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error opening Database:\n\n" + e.Message, $"{Global.Product}", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }


        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        /// <summary>
        /// Participants
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetPart_Click(object sender, RoutedEventArgs e)
        {
            if (Global.ms != null)
                if (Global.ms.IsLoaded)
                {
                    Global.ms.Focus();
                    return;
                }

            Global.ms = new MatchShooters(Global.CurrentMatch);
            Global.ms.Show();
        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Shutdown();
        }

        public void Shutdown()
        {
            tfh.Dispose();

            if (Global.CurrentMatch != null)
            {
                Properties.Settings.Default.LastMatchId = Global.CurrentMatch.MatchID;
                Properties.Settings.Default.Save();
            }

            Application.Current.Shutdown();
        }

        /// <summary>
        /// Print
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrintMenu_Click(object sender, RoutedEventArgs e)
        {
            if (Global.ps != null)
                if (Global.ps.IsLoaded)
                {
                    Global.ps.Focus();
                    return;
                }

            Global.ps = new PrintStuff(tfh);
            Global.ps.Show();
        }

        /// <summary>
        /// Categories
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            if (Global.cw != null)
                if (Global.cw.IsLoaded)
                {
                    Global.cw.Focus();
                    return;
                }

            Global.cw = new CategoryWindow();
            Global.cw.Show();
        }

        /// <summary>
        /// Matches
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnMatches_Click(object sender, RoutedEventArgs e)
        {
            if (Global.mw != null)
                if (Global.mw.IsLoaded)
                {
                    Global.mw.Focus();
                    return;
                }

            Global.mw = new MatchWindow();
            Global.mw.Show();
        }

        /// <summary>
        /// Shooters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnShooters_Click(object sender, RoutedEventArgs e)
        {
            if (Global.sw != null)
                if (Global.sw.IsLoaded)
                {
                    Global.sw.Focus();
                    return;
                }
            Global.sw = new ShooterWindow();
            Global.sw.Show();
        }
    }
}
