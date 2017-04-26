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
    public partial class PrintStuff : MaMaWindow
    {
        private MatchMasterContext _ctx = new MatchMasterContext();
        private TFH tfh;

        public PrintStuff(TFH TempFileHandler) : base("Print")
        {
            InitializeComponent();

            this.tfh = TempFileHandler;
            FillDropdowns();
        }

        void FillDropdowns()
        {
            LstPosses.Items.Clear();
            LstPossesRS.Items.Clear();

            List<PosseListForDropdown> l = new List<PosseListForDropdown>();

            for( int i = 1; i <= Global.CurrentMatch.NumberOfPosses; i++)
            {
                var c = _ctx.MatchParticipations.Where(x => ( x.MatchID == Global.CurrentMatch.MatchID && x.Posse.Equals(i))).Count();
                l.Add(new PosseListForDropdown() { DisplayName = $"Posse #{i.ToString()} ({c})", PosseID = i });
            }

            LstPosses.ItemsSource = l.ToList();
            LstPosses.SelectedIndex = 0;

            LstPossesRS.ItemsSource = l.ToList();
            LstPossesRS.SelectedIndex = 0;

            // ========================

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
            var posse = (int)LstPosses.SelectedValue;

            var c = _ctx.MatchParticipations.Where(x => (x.MatchID == Global.CurrentMatch.MatchID && x.Posse == posse)).Count();

            if (c.Equals(0))
            {
                MessageBox.Show($"You cannot print an empty Posse List.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var f = PrintPosseListToFile(posse);
            if (f == null) return;

            Process.Start(f);
        }

        private void BtnPrintAllPosseList_Click(object sender, RoutedEventArgs e)
        {
            List<string> filenames = new List<string>();

            for (int i = 1; i <= Global.CurrentMatch.NumberOfPosses; i++) filenames.Add(PrintPosseListToFile(i));

            var merged_file = MergeFiles(filenames);
            if (merged_file == null) return;

            Process.Start(merged_file);
        }

        /// <summary>
        /// merge some pdf files into one
        /// </summary>
        /// <param name="filenames"></param>
        /// <returns></returns>
        private string MergeFiles(List<string> filenames)
        {
            if (filenames == null) return null;
            if (filenames.Count.Equals(0)) return null;

            bool merged = true;
            var temp = tfh.get("pdf");

            using (FileStream fs = new FileStream(temp, FileMode.Create))
            {
                Document d = new Document();
                PdfCopy c = new PdfCopy(d, fs);
                PdfReader r = null;

                try
                {
                    d.Open();
                    foreach(string f in filenames)
                    {
                        r = new PdfReader(f);
                        c.AddDocument(r);
                        r.Close();
                    }
                }
                catch
                {
                    merged = false;
                    if (r != null) r.Close();
                }
                finally
                {
                    if (d != null) d.Close();
                }

                if (merged) return temp;
                return null;
            }

            
        }

        private string PrintPosseListToFile(int p)
        {
            var fn = tfh.get("pdf");

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Document document = PdfPages.get();

                PdfWriter w = PdfWriter.GetInstance(document, new FileStream(fn, FileMode.Create));
                w.PageEvent = new PdfHeaderFooter($"Posse #{p}", Global.CurrentMatch.ToString());

                document.Open();

                Font fnt = new Font(Font.FontFamily.HELVETICA, 12f, Font.BOLD);

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
                        new PdfPCell(new Phrase(new Chunk(mp.Category,fnt)))
                    };

                    s.Rows.Add(new PdfPRow(lc));
                }


                document.Add(s);
                document.Close();

                return fn;
            }

            
        }

        private void BtnPrintPosseRatingSheet_Click(object sender, RoutedEventArgs e)
        {
            var posse = (int)LstPossesRS.SelectedValue;

            var c = _ctx.MatchParticipations.Where(x => (x.MatchID == Global.CurrentMatch.MatchID && x.Posse == posse)).Count();

            if (c.Equals(0))
            {
                MessageBox.Show($"You cannot print a Rating Sheet for an empty Posse.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var f = PrintRatingSheetToFile(posse);
            if (f == null) return;

            Process.Start(f);
        }

        /// <summary>
        /// Rating Sheet
        /// </summary>
        /// <param name="Posse"></param>
        /// <returns></returns>
        string PrintRatingSheetToFile(int p)
        {
            var fn = tfh.get("pdf");

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Document document = PdfPages.get();

                PdfWriter w = PdfWriter.GetInstance(document, new FileStream(fn, FileMode.Create));
                w.PageEvent = new PdfHeaderFooter($"Rating Sheet | Posse #{p} | Stage #___", Global.CurrentMatch.ToString());

                document.Open();

                Font fnt = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD);

                PdfPTable s = new PdfPTable(11);
                s.DefaultCell.Border = 0;
                s.HeaderRows = 1;

                s.WidthPercentage = 100;
                int[] pw2 = { 25, 15, 10, 10, 5, 5, 5 , 5, 5, 5, 25 };
                s.SetWidths(pw2);

                PdfPCell[] hc = new PdfPCell[]
                {
                    new PdfPCell(new Phrase(new Chunk("Surname,First Name\nNickname",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Category\nOutfit Warn.",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Time",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Error",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Ablauff.",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("MSV",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Bonus",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Spirit",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("S-DQ",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("M-DQ",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Signature",fnt))) { Border=0 },
                };

                foreach (var c in hc) c.PaddingBottom = 5;

                s.Rows.Add(new PdfPRow(hc));

                fnt = new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL);

                var mp_list = _ctx.MatchParticipations.Include("Shooter").Where(x => (x.MatchID == Global.CurrentMatch.MatchID && x.Posse == p)).ToList();

                foreach (MatchParticipation mp in mp_list)
                {
                    string n = mp.Shooter.Surname + "," + mp.Shooter.FirstName + Environment.NewLine + (string.IsNullOrEmpty(mp.Shooter.Nickname) ? " " : mp.Shooter.Nickname);

                    PdfPCell[] lc = new PdfPCell[]
                    {
                        new PdfPCell(new Phrase(new Chunk(n,fnt))),
                        new PdfPCell(new Phrase(new Chunk(mp.Category,fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt)))
                    };

                    s.Rows.Add(new PdfPRow(lc));
                }


                document.Add(s);
                document.Close();

                return fn;
            }
        }

        private void BtnPrintAllSpeedTicket_Click(object sender, RoutedEventArgs e)
        {
            var sp = from s in _ctx.MatchParticipations.Include("Shooter").Where(x => (x.MatchID == Global.CurrentMatch.MatchID) && (x.IsSpeedTicket))
                     orderby s.Shooter.Surname, s.Shooter.FirstName
                     select s;

            if (sp.Count().Equals(0))
            {
                MessageBox.Show($"There are no Speed Tickets in this Match.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<string> filenames = new List<string>();

            foreach(var mp in sp)
            {
                var fn = PrintSpeedTicket(mp);
                if (fn != null) filenames.Add(fn);
            }

            if (filenames.Count.Equals(0)) return;

            var result_file = MergeFiles(filenames);

            Process.Start(result_file);
        }

        /// <summary>
        /// Print one Speed Ticket Rating Sheet
        /// </summary>
        /// <param name="mp_id"></param>
        /// <returns></returns>
        string PrintSpeedTicket(MatchParticipation mp)
        {
            var fn = tfh.get("pdf");

            using (MemoryStream myMemoryStream = new MemoryStream())
            {
                Document document = PdfPages.get();

                PdfWriter w = PdfWriter.GetInstance(document, new FileStream(fn, FileMode.Create));
                w.PageEvent = new PdfHeaderFooter($"Speed Ticket Rating Sheet\n{mp.Shooter.ToString()}", Global.CurrentMatch.ToString());

                document.Open();

                Font fnt = new Font(Font.FontFamily.HELVETICA, 11f, Font.BOLD);

                PdfPTable s = new PdfPTable(10);
                s.DefaultCell.Border = 0;
                s.HeaderRows = 1;

                s.WidthPercentage = 100;
                int[] pw2 = {10, 10, 10, 5, 5, 5, 5, 5, 5, 25 };
                s.SetWidths(pw2);

                PdfPCell[] hc = new PdfPCell[]
                {
                    new PdfPCell(new Phrase(new Chunk("Stage #\n ",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Time",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Error",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Ablauff.",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("MSV",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Bonus",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Spirit",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("S-DQ",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("M-DQ",fnt))) { Border=0 },
                    new PdfPCell(new Phrase(new Chunk("Signature",fnt))) { Border=0 },
                };

                foreach (var c in hc) c.PaddingBottom = 5;

                s.Rows.Add(new PdfPRow(hc));

                fnt = new Font(Font.FontFamily.HELVETICA, 12f, Font.NORMAL);

                for (int i = 1; i <= Global.CurrentMatch.NumberOfStages; i++)
                {
                    PdfPCell[] lc = new PdfPCell[]
                    {
                        new PdfPCell(new Phrase(new Chunk(i.ToString(),fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt))),
                        new PdfPCell(new Phrase(new Chunk("",fnt)))
                    };

                    s.Rows.Add(new PdfPRow(lc));

                }

                document.Add(s);
                document.Close();

                return fn;
            }
        }
    }
}
