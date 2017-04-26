using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MatchMaster
{
    public static class PdfPages
    {
        public static Document get()
        {
            Document d = new Document(PageSize.A4.Rotate(), 
                iTextSharp.text.Utilities.MillimetersToPoints(10),
                iTextSharp.text.Utilities.MillimetersToPoints(10),
                iTextSharp.text.Utilities.MillimetersToPoints(25),
                iTextSharp.text.Utilities.MillimetersToPoints(15));
            return d;
        }
    }

    public class PdfHeaderFooter : PdfPageEventHelper
    {
        PdfTemplate footer;

        string topic = string.Empty;
        string title = string.Empty;

        BaseFont footer_font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

        const int footer_font_size = 9;
        const float header_font_size = 14;

        iTextSharp.text.Font header_font = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, header_font_size, iTextSharp.text.Font.BOLD, iTextSharp.text.BaseColor.BLACK);

        public PdfHeaderFooter(string Topic, string Title)
        {
            this.topic = Topic;
            this.title = Title;
        }

        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

            footer = writer.DirectContent.CreateTemplate(
                iTextSharp.text.Utilities.MillimetersToPoints(20), 
                iTextSharp.text.Utilities.MillimetersToPoints(10));

            document.AddAuthor("MatchMaster");
            document.AddTitle(title);

        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            footer.BeginText();
            footer.SetFontAndSize(footer_font, footer_font_size);
            footer.SetTextMatrix(0, 0);
            footer.ShowText(writer.PageNumber.ToString());
            footer.EndText();
        }


        public override void OnStartPage(PdfWriter writer, Document document)
        {
            base.OnStartPage(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            var cb = writer.DirectContent;

            // ===================================

            PdfPTable h = new PdfPTable(2);
            h.TotalWidth = document.Right - document.Left;

            float[] pw = { 40f, 60f };
            h.SetWidths(pw);

            PdfPCell hc1 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_LEFT,
                Border = 0,
                Phrase = new Phrase(new Chunk(this.topic,header_font)),
                
            };

            PdfPCell hc2 = new PdfPCell()
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Border = 0,
                Phrase = new Phrase(new Chunk(this.title,header_font))
            };

            h.AddCell(hc1);
            h.AddCell(hc2);
            
            h.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - iTextSharp.text.Utilities.MillimetersToPoints(10), cb);

            // ===================================

            string text = $"Page {writer.PageNumber} of ";

            cb.BeginText();
            cb.SetFontAndSize(footer_font,footer_font_size);
            cb.SetTextMatrix(document.LeftMargin, iTextSharp.text.Utilities.MillimetersToPoints(10));
            cb.ShowText(text);
            cb.EndText();

            float l = footer_font.GetWidthPoint(text,footer_font_size);
            cb.AddTemplate(footer, document.LeftMargin + l, iTextSharp.text.Utilities.MillimetersToPoints(10));

            // Copyright

            text = "MatchMaster © 2017 by COREBYTE";
            l = footer_font.GetWidthPoint(text, footer_font_size);

            cb.BeginText();
            cb.SetFontAndSize(footer_font, footer_font_size);
            cb.SetTextMatrix(document.Right-l, iTextSharp.text.Utilities.MillimetersToPoints(10));
            cb.ShowText(text);
            cb.EndText();

        }

    }
}
