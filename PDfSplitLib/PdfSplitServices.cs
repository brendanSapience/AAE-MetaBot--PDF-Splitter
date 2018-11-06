using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    public class PdfSplitServices
    {
        public String GetListOfPagesContainingString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            F1 f1 = new F1();
            return f1.GetListOfPagesContainingString(PDFFilePath, PDFFileName, StringToFind);
        }

        public String GetRangesFromKeywordAndPageCount(String PDFFilePath, String PDFFileName, String Keyword)
        {
            F1 f1 = new F1();
            return f1.GetRangesFromKeywordAndPageCount(PDFFilePath, PDFFileName, Keyword);
        }

            public String SidsSpecialFunction(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            F2 f2 = new F2();
            return f2.SidsSpecialFunction(PDFFilePath, PDFFileName, StringToFind);
        }

    }
}
