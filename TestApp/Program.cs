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
            utils.SetFile("MyPdfFile");
            utils.TestTesseract();

        }
    }
}
