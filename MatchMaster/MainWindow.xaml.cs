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

//using PdfSharp;
//using PdfSharp.Pdf;
//using PdfSharp.Drawing;

//using MigraDoc;

using System.Diagnostics;
//using MigraDoc.DocumentObjectModel;
//using MigraDoc.Rendering;
//using MigraDoc.DocumentObjectModel.Tables;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.SqlClient;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();

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
            var last_match_id = Properties.Settings.Default.LastMatchId;

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
                this.Title = "MatchMaster";
                return;
            }
            this.BtnPrintMenu.IsEnabled = true;
            this.BtnSetPart.IsEnabled = true;
            this.Title = "MatchMaster - " + Global.CurrentMatch.ToString();
        }

        private bool CheckSqlServer()
        {
            using (SqlConnection c = new SqlConnection(Properties.Settings.Default.SQLEXPRESS))
            {
                try
                {
                    c.Open();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }


        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MatchWindow mw = new MatchWindow();
            mw.Show();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Shutdown();
        }

        private void MnuPdf_Click(object sender, RoutedEventArgs e)
        {
            const string filename = "HelloWorld.pdf";

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4);
                PdfWriter w = PdfWriter.GetInstance(document, new FileStream(filename,FileMode.Create));
                document.Open();

                PdfPTable t = new PdfPTable(1);
                t.HeaderRows = 1;
                t.WidthPercentage = 100;
                for (int i = 0; i < 300; i++)
                {

                    t.AddCell($"Zelle {i}!!!");

                }

                document.Add(new iTextSharp.text.Paragraph("Hallo !!!"));
                document.Add(t);
                document.Close();


            }

            Process.Start(filename);











        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShooterWindow w = new ShooterWindow();
            w.Show();
        }

        private void BtnSetPart_Click(object sender, RoutedEventArgs e)
        {
            MatchShooters ms = new MatchShooters(Global.CurrentMatch);
            ms.Show();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Shutdown();
        }

        public void Shutdown()
        {
            if (Global.CurrentMatch != null)
            {
                Properties.Settings.Default.LastMatchId = Global.CurrentMatch.MatchID;
                Properties.Settings.Default.Save();
            }

            Application.Current.Shutdown();
        }

        private void BtnPrintMenu_Click(object sender, RoutedEventArgs e)
        {
            PrintStuff w = new PrintStuff();
            w.Show();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            CategoryWindow cw = new CategoryWindow();
            cw.Show();
        }
    }
}
