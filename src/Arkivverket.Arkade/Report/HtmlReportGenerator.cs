using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arkivverket.Arkade.Core;

namespace Arkivverket.Arkade.Report
{
    public class HtmlReportGenerator : IReportGenerator<HtmlReport>
    {
        public HtmlReportGenerator()
        {
        }

        public HtmlReport Generate(TestSession testSession)
        {
            StringBuilder html = new StringBuilder();

            html.Append(Header());


            return new HtmlReport(html.ToString());
        }

        private string Header()
        {
            return "";
        }
    }
}
