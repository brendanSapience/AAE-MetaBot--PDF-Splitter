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
        private Guid guid;

        public PdfUtils() {

            Guid g = Guid.NewGuid();
            this.guid = g;
        }

        public void SetFile(String PathToCsvFile)
        {
            this.FilePath = PathToCsvFile;

        }

        public List<String> FindPageWithString(Dictionary<String,String> InputDict, String StringToFind)
        {
            List<String> l = new List<String>();

            foreach (KeyValuePair<String, String> entry in InputDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;
                if (PageContent.Contains(StringToFind))
                {
                    l.Add(entry.Key);
                }

            }
            return l;
        }

        public String FindPageWithHeaderAndFooter(Dictionary<String, String> InputDict, String Header, String Footer)
        {
            List<String> Allheaders = new List<String>();
            Allheaders = FindPageWithString(InputDict, Header);

            List<String> AlLFooters = new List<String>();
            AlLFooters = FindPageWithString(InputDict, Footer);
            String output = "";
            if(Allheaders.Count() == AlLFooters.Count())
            {
                int i = 0;
                while(i < Allheaders.Count())
                {
                    output = output + "[";
                    String HeaderPage = Allheaders[i];
                    String FooterPage = AlLFooters[i];
                    output = output + HeaderPage + "-" + FooterPage + "]";


                    i++;


                }
            }
            return output;
        }

        public Dictionary<String,String> GetDictionaryFromPdf(String RootPath, String FileName)
        {
            // Split PDF into individual Pages using PDFSharp (PDFSharp in NuGet Package Manager)
            Dictionary<String, String> DocumentContent = new Dictionary<String, String>();

            //String RootPath = @"C:\dev\docs\";
            // "90000081.pdf"

            String TempPath = RootPath + guid+@"\";

            System.IO.Directory.CreateDirectory(TempPath);

            PdfSharpUtils psu = new PdfSharpUtils();
            psu.SplitAllPDFPages(RootPath, FileName, TempPath);

            // Now convert each pdf file into a png file
            // requires ImageMagick Wrapper for C# (Magick.NET Q16 Any CPU in NuGet Package manager)
            // because we do PDF conversions, the target system on which this is running requires install GhostScript

            ImageMagickUtils imu = new ImageMagickUtils(300, 300);

            string[] PdfFiles = Directory.GetFiles(TempPath, "*.pdf", SearchOption.TopDirectoryOnly);
            foreach(string filepath in PdfFiles)
            {
                imu.ConvertPDFToPng(filepath, filepath+".png");
            }

            // Now using Tesseract to OCR the single pdf page turned into a png file (Tesseract in NuGet Package Manager)
            // this involves downloading the Tesseract language data files locally (see tessdata below).

            TesseractUtils tu = new TesseractUtils(@"C:\dev\tessdata", "eng");

            string[] PngFiles = Directory.GetFiles(TempPath, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string filepath in PngFiles)
            {
                Console.Write("Processing File: " + filepath);

                String filename = Path.GetFileName(filepath);
                String[] tempArr = filename.Split('_');
                String PageNum = tempArr[0];
                Console.Write("Page Number: " + PageNum);
                

                TesseractOutput to = tu.OCRImageFile(filepath);
                //Console.Write(to.getText());
                DocumentContent.Add(PageNum, to.getText());
                //Console.ReadKey();
                
            }

            return DocumentContent;
            
        }
    }

}
