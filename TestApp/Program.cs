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

            String Res = pss.GetRangesFromKeywordAndPageCount(@"C:\dev\docs\", "90000081.pdf", "Certificate of Analysis");

            Console.Write("Output: "+Res);
            Console.ReadKey();


        }
    }
}
