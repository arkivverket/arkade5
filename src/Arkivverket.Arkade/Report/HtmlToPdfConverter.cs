using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;

namespace Arkivverket.Arkade.Report
{
    public class HtmlToPdfConverter
    {
        public static byte[] Convert(string html)
        {
            MemoryStream output = new MemoryStream();
            StringReader srHtml = new StringReader(html);

            Document document = new Document(PageSize.A4, 30, 30, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, output);

            document.Open();

            // Add Custom image tag processor to handle inlined images i html file
            DefaultTagProcessorFactory tagProcessors = (DefaultTagProcessorFactory) Tags.GetHtmlTagProcessorFactory();
            tagProcessors.RemoveProcessor(HTML.Tag.IMG);
            tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor());

            CssFilesImpl cssFiles = new CssFilesImpl();
            cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
            StyleAttrCSSResolver cssResolver = new StyleAttrCSSResolver(cssFiles);

            HtmlPipelineContext hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
            hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors);
            HtmlPipeline htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(document, writer));
            CssResolverPipeline pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
            XMLWorker worker = new XMLWorker(pipeline, true);
            XMLParser xmlParser = new XMLParser(true, worker, Encoding.UTF8);
            xmlParser.Parse(srHtml);

            document.Close();
            return output.ToArray();
        }
    }
}