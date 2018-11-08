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
            Dictionary<int, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", false);
            
            return "";
        }
    }
}
