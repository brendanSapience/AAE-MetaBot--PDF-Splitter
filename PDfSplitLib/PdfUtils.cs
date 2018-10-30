using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;


namespace PDfSplitLib
{
    public class PdfUtils
    {
        private String FilePath = "";
        private System.IO.StreamReader reader;

        public PdfUtils() { }

        public void SetFile(String PathToCsvFile)
        {
            this.FilePath = PathToCsvFile;

        }

        public void TestTesseract()
        {
            var testImagePath = @"C:\dev\docs\phototest.tif";
            try
            {
                using (var engine = new TesseractEngine(@"C:\dev\tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());

                            Console.WriteLine("Text (GetText): \r\n{0}", text);
                            Console.WriteLine("Text (iterator):");
                            using (var iter = page.GetIterator())
                            {
                                iter.Begin();

                                do
                                {
                                    do
                                    {
                                        do
                                        {
                                            do
                                            {
                                                if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                                                {
                                                    Console.WriteLine("<BLOCK>");
                                                }

                                                Console.Write(iter.GetText(PageIteratorLevel.Word));
                                                Console.Write(" ");

                                                if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                                                {
                                                    Console.WriteLine();
                                                }
                                            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                                            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                                            {
                                                Console.WriteLine();
                                            }
                                        } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
                                    } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
                                } while (iter.Next(PageIteratorLevel.Block));
                            }
                        }
                    }
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("Unexpected Error: " + e.Message);
                Console.WriteLine("Details: ");
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
        }
    }
}
