using PDfSplitLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {

            PdfSplitServices pss = new PdfSplitServices();

            //String Result = pss.GetListOfPagesContainingString(@"C:\dev\docs\", "90000081.pdf", "Invoice");

            
            //String Res = pss.GetRangesFromRegexGroup(@"C:\dev\docs\", "90000081-short.pdf", @"Invoice[ ]*n.[ ]*(\d+)");

            //String Res = pss.GetRangesFromRegexGroup(@"C:\dev\docs\", "90000081-short.pdf", @"delivery[ ]*note(.*)");

            //String Res = pss.GetRangesFromRegexGroup(@"C:\dev\docs\", "90000081.pdf", @"Certificate of (Analysis)");

            pss.LoadPDFFile(@"C:\dev\docs\", "90000081.pdf");
            String Res = pss.GetRangesFromRegexGroup(@"Certificate of (Analysis)");

            //String Res = pss.GetRangesFromKeywordAndPageCount(@"C:\dev\docs\", "90000081.pdf", "Certificate of Analysis");

            Console.Write("\nOutput: "+Res);
            Console.ReadKey();


        }
    }
}
