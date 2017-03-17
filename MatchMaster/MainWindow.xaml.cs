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

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            SetTitle();
        }

        public void SetTitle()
        {
            if (Global.CurrentMatch == null)
            {
                this.Title = "MatchMaster";
                return;
            }

            this.Title = "MatchMaster - " + Global.CurrentMatch.ToString();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MatchWindow mw = new MatchWindow();
            mw.Show();
        }

        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
    }
}
