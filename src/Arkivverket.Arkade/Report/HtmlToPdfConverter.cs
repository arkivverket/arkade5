using System.IO;
using PdfSharp;
using PdfSharp.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;

namespace Arkivverket.Arkade.Report
{
    public class HtmlToPdfConverter
    {
        public static byte[] Convert(string html)
        {
            byte[] res;
            using (MemoryStream ms = new MemoryStream())
            {
                PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
    }
}