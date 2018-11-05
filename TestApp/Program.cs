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
            PdfUtils utils = new PdfUtils();
            
            // the dictionary contains the page numbers as Keys and the OCR Text as a value.
            Dictionary<String, String> resp = new Dictionary<String, String>();
            resp = utils.GetDictionaryFromPdf(@"C:\dev\docs\", "90000081.pdf");
            
            List<String> listOfPages = utils.FindPageWithString(resp,"Delivery Note");

            utils.FindPageWithHeaderAndFooter(resp, "Delivery Note","");

            Console.Write("Page List: " + String.Join(", ", listOfPages.ToArray()));
            

            Console.Write("Number of elements:"+resp.Count());
            Console.ReadKey();
        }
    }
}
