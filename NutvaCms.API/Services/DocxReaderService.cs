using DocumentFormat.OpenXml.Packaging;
using System.Text;

namespace NutvaCms.API.Services
{
    public class DocxReaderService
    {
        public string ReadDocxText(string filePath)
        {
            var sb = new StringBuilder();
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                sb.Append(body.InnerText);
            }
            return sb.ToString();
        }
    }
}
