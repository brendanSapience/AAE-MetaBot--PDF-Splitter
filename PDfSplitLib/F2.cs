using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    class F2
    {
        public F2() { }
        public String SidsSpecialFunction(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName);
            List<String> ListOfPages = new List<String>();
            foreach (KeyValuePair<String, String> entry in myDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;
                if (PageContent.Contains(StringToFind))
                {
                    ListOfPages.Add(entry.Key);
                }

            }
            var result = String.Join(",", ListOfPages.ToArray());
            return result;
        }
    }
}
