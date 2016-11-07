namespace Arkivverket.Arkade.Report
{
    public class HtmlReport : IReport
    {
        private readonly string _html;

        public HtmlReport(string html)
        {
            _html = html;
        }

        public string GetHtml()
        {
            return _html;
        }
    }
}