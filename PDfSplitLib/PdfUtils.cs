using ImageMagick;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            // Split PDF into individual Pages using PDFSharp (PDFSharp in NuGet Package Manager)

            /*
            // Get a fresh copy of the sample PDF file
            const string filename = "90000081.pdf";
            File.Copy(Path.Combine("C:/dev/docs/",filename),
              Path.Combine(Directory.GetCurrentDirectory(), filename), true);

            // Open the file
            PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

            string name = Path.GetFileNameWithoutExtension(filename);
            for (int idx = 0; idx < inputDocument.PageCount; idx++)
            {
                // Create new document
                Console.Write("Debug: "+idx);
                PdfDocument outputDocument = new PdfDocument();
                outputDocument.Version = inputDocument.Version;
                outputDocument.Info.Title =
                  String.Format("Page {0} of {1}", idx + 1, inputDocument.Info.Title);
                outputDocument.Info.Creator = inputDocument.Info.Creator;

                // Add the page and save it
                outputDocument.AddPage(inputDocument.Pages[idx]);
                String Str = String.Format("{0} - Page {1}_tempfile.pdf", name, idx + 1);
                Console.Write(Str+"\n");
                outputDocument.Save("C:/dev/docs/"+Str);
            }
            */

            PdfSharpUtils psu = new PdfSharpUtils();
            psu.SplitAllPDFPages(@"C:\dev\docs\", "90000081.pdf", "C:/dev/docs/split");

            // Now convert each pdf file into a png file
            // requires ImageMagick Wrapper for C# (Magick.NET Q16 Any CPU in NuGet Package manager)
            // because we do PDF conversions, the target system on which this is running requires install GhostScript

            ImageMagickUtils imu = new ImageMagickUtils(300,300);
            imu.ConvertPDFToPng(@"C:\dev\docs\split\90000081 - Page 1_tempfile.pdf", @"C:\dev\docs\split\90000081 - Page 1_tempfile.png");

            // Now using Tesseract to OCR the single pdf page turned into a png file (Tesseract in NuGet Package Manager)
            // this involves downloading the Tesseract language data files locally (see tessdata below).

            TesseractUtils tu = new TesseractUtils(@"C:\dev\tessdata", "eng");
            TesseractOutput to = tu.OCRImageFile(@"C:\dev\docs\split\90000081 - Page 1_tempfile1.png");
            Console.Write(to.getText());
            Console.ReadKey();


            /*
            try
            {
                var testImagePath = @"C:\dev\docs\split\90000081 - Page 1_tempfile1.png";
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
            */
        }
    }

}
