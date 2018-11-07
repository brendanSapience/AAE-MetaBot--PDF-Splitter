using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    public class PdfSplitServices
    {
        // -------------------- Bren's ------------------ //

        // Following method not ready
        public String GetRangesFromRegexGroup(String PDFFilePath, String PDFFileName, String RegexWithRepeatedGroup)
        {
            F1 f1 = new F1();
            return f1.GetRangesFromRegexGroup(PDFFilePath, PDFFileName, RegexWithRepeatedGroup, true);
        }

        public String GetListOfPagesContainingString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            F1 f1 = new F1();
            return f1.GetListOfPagesContainingString(PDFFilePath, PDFFileName, StringToFind);
        }

        public String GetRangesFromKeywordAndPageCount(String PDFFilePath, String PDFFileName, String Keyword)
        {
            F1 f1 = new F1();
            return f1.GetRangesFromKeywordAndPageCount(PDFFilePath, PDFFileName, Keyword,false);
        }

        public String GetRangesFromKeywordAndPageCountWithRegex(String PDFFilePath, String PDFFileName, String Keyword,String RegexWithGroupsForPageCount)
        {
            F1 f1 = new F1();
            return f1.GetRangesFromKeywordAndPageCount(PDFFilePath, PDFFileName, Keyword, RegexWithGroupsForPageCount,false);
        }


        // ------------------ Sid's -------------------- // 
        public String SidsSpecialFunction(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            F2 f2 = new F2();
            return f2.SidsSpecialFunction(PDFFilePath, PDFFileName, StringToFind);
        }

    }
}
