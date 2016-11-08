using System.IO;

namespace Arkivverket.Arkade.Report
{
    public class HtmlReport : IReport
    {
        public static readonly string REPORT_HTML = "report.html";
        public static readonly string CSS_DIR = "css";
        public static readonly string IMG_DIR = "img";
        public static readonly string BOOTSTRAP_CSS = CSS_DIR + "\\bootstrap.min.css";
        public static readonly string ARKADE_CSS = CSS_DIR + "\\arkade.css";
        public static readonly string ARKIVVERKET_IMG = IMG_DIR + "\\arkivverket.gif";

        private readonly string _html;

        public HtmlReport(string html)
        {
            _html = html;
        }

        public string GetHtml()
        {
            return _html;
        }

        public void Save(DirectoryInfo directory)
        {
            if (!directory.Exists)
            {
                directory.Create();
            }
            string htmlFile = Path.Combine(directory.FullName, REPORT_HTML);
            File.WriteAllText(htmlFile, _html);

            DirectoryInfo cssDir = new DirectoryInfo(Path.Combine(directory.FullName, CSS_DIR));
            if (!cssDir.Exists)
            {
                cssDir.Create();
            }
            File.Copy(BOOTSTRAP_CSS, Path.Combine(directory.FullName, BOOTSTRAP_CSS), true);
            File.Copy(ARKADE_CSS, Path.Combine(directory.FullName, ARKADE_CSS), true);

            DirectoryInfo imgDir = new DirectoryInfo(Path.Combine(directory.FullName, IMG_DIR));
            if (!imgDir.Exists)
            {
                imgDir.Create();
            }
            File.Copy(ARKIVVERKET_IMG, Path.Combine(directory.FullName, ARKIVVERKET_IMG), true);
        }
    }
}