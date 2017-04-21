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

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Diagnostics;

namespace MatchMaster
{
    /// <summary>
    /// Interaction logic for PrintStuff.xaml
    /// </summary>
    public partial class PrintStuff : Window
    {
        private MatchMasterContext _ctx = new MatchMasterContext();

        public PrintStuff()
        {
            InitializeComponent();

            this.MinHeight = App.ScreenHeight / 2;
            this.Width = App.ScreenWidth / 2;
            this.MinWidth = App.ScreenWidth / 3;

            FillDropdowns();
        }

        void FillDropdowns()
        {
            LstPosses.Items.Clear();

            List<PosseListForDropdown> l = new List<PosseListForDropdown>();

            for( int i = 1; i <= Global.CurrentMatch.NumberOfPosses; i++)
            {
                var c = _ctx.MatchParticipations.Where(x => ( x.MatchID == Global.CurrentMatch.MatchID && x.Posse.Equals(i))).Count();
                l.Add(new PosseListForDropdown() { DisplayName = $"Posse #{i.ToString()} ({c})", PosseID = i });
            }

            LstPosses.ItemsSource = l.ToList();
            LstPosses.SelectedIndex = 0;
        }

        private class PosseListForDropdown
        {
            public string DisplayName { get; set; }
            public int PosseID { get; set; }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnPrintPosseList_Click(object sender, RoutedEventArgs e)
        {
            PrintPosseList((int)LstPosses.SelectedValue);
        }



        private void PrintPosseList(int p)
        {
            var c = _ctx.MatchParticipations.Where(x => (x.MatchID == Global.CurrentMatch.MatchID && x.Posse == p)).Count();

            if (c.Equals(0))
            {
                MessageBox.Show($"You cannot print an empty Posse List.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
            }

            const string filename = "PosseList.pdf";

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                PdfWriter w = PdfWriter.GetInstance(document, new FileStream(filename, FileMode.Create));
                document.Open();

                Font fnt = new Font(Font.FontFamily.HELVETICA, 18f, Font.BOLD);

                PdfPTable h = new PdfPTable(2);
                h.DefaultCell.Border = 0;

                h.WidthPercentage = 100;
                int[] pw1 = { 30, 70 };
                h.SetWidths(pw1);

                PdfPCell[] phc = new PdfPCell[]
                {
                    new PdfPCell(new Phrase(new Chunk($"List of Posse #{p.ToString()}",fnt))) { Border = 0, PaddingBottom = 10 },
                    new PdfPCell(new Phrase(new Chunk(Global.CurrentMatch.ToString(),fnt))) { HorizontalAlignment = Element.ALIGN_RIGHT, Border=0, PaddingBottom = 10 }
                };

                h.Rows.Add(new PdfPRow(phc));

                document.Add(h);

                fnt = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD);

                PdfPTable s = new PdfPTable(6);
                s.DefaultCell.Border = 0;
                s.HeaderRows = 1;

                s.WidthPercentage = 100;
                int[] pw2 = { 10, 10, 20, 20, 20, 20 };
                s.SetWidths(pw2);

                PdfPCell[] hc = new PdfPCell[]
                {
                    new PdfPCell(new Phrase(new Chunk("RO-CH",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("RO-DEP",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Surname",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("First Name",fnt))){ Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Nick name",fnt))){ Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Weapon class",fnt))){ Border=0 }
                };

                s.Rows.Add(new PdfPRow(hc));

                fnt = new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL);

                var mp_list = _ctx.MatchParticipations.Include("Shooter").Where(x => (x.MatchID == Global.CurrentMatch.MatchID && x.Posse==p)).ToList();

                foreach(MatchParticipation mp in mp_list)
                {
                    PdfPCell[] lc = new PdfPCell[]
                    {
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk(mp.Shooter.Surname,fnt))),
                        new PdfPCell(new Phrase(new Chunk(mp.Shooter.FirstName,fnt))),
                        new PdfPCell(new Phrase(new Chunk(mp.Shooter.Nickname,fnt))),
                        new PdfPCell(new Phrase(new Chunk("Weapon class",fnt)))
                    };

                    s.Rows.Add(new PdfPRow(lc));
                }


                document.Add(s);




                document.Close();
            }

            Process.Start(filename);
        }
    }
}
