using ImageMagick;
using Newtonsoft.Json;
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

        public Dictionary<int, String> GetDictionaryFromPdf(String RootPath, String FileName, String PathToTessData, String Language, Boolean Debug)
        {
            // Split PDF into individual Pages using PDFSharp (PDFSharp in NuGet Package Manager)
            Dictionary<int, String> DocumentContent = new Dictionary<int, String>();

            //String RootPath = @"C:\dev\docs\";
            // "90000081.pdf"
          
            // accounting for C:\mypath and C:\mypath\
            if(!RootPath.EndsWith(@"\") && !RootPath.EndsWith("/"))
            {
                RootPath = RootPath + "/";
            }

            String TempPath = RootPath + guid + @"\";
            String LogFile = TempPath + "run.log";

        
            System.IO.Directory.CreateDirectory(TempPath);

            StreamWriter w = File.AppendText(LogFile);

            PdfSharpUtils psu = new PdfSharpUtils(w);
            psu.SplitAllPDFPages(RootPath, FileName, TempPath, Debug);

            // Now convert each pdf file into a png file
            // requires ImageMagick Wrapper for C# (Magick.NET Q16 Any CPU in NuGet Package manager)
            // because we do PDF conversions, the target system on which this is running requires install GhostScript

            ImageMagickUtils imu = new ImageMagickUtils(300, 300,w);

            string[] PdfFiles = Directory.GetFiles(TempPath, "*.pdf", SearchOption.TopDirectoryOnly);
            w.WriteLine("DEBUG - Number of PDF Files Found: " + PdfFiles.Count());
            w.Flush();
            foreach (string filepath in PdfFiles)
            {
                imu.ConvertPDFToPng(filepath, filepath + ".png");
            }

            // Now using Tesseract to OCR the single pdf page turned into a png file (Tesseract in NuGet Package Manager)
            // this involves downloading the Tesseract language data files locally (see tessdata below).
            //ConfigItem item = LoadJson();

            TesseractUtils tu = new TesseractUtils(PathToTessData, Language,w);

            string[] PngFiles = Directory.GetFiles(TempPath, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string filepath in PngFiles)
            {
                if (Debug) {Console.WriteLine("\nDEBUG: Processing Temp Image File: " + filepath); }
                w.WriteLine("DEBUG: Processing Temp Image File: " + filepath);
                String filename = Path.GetFileName(filepath);
                String[] tempArr = filename.Split('_');
                String PageNum = tempArr[0];
                if (Debug) { Console.WriteLine("\nDEBUG: Current Page Number: " + PageNum); }
                w.WriteLine("DEBUG: Current Page Number: " + PageNum);

                TesseractOutput to = tu.OCRImageFile(filepath, Debug);
                //Console.WriteLine(to.getText());
                int PageNumAsInt = -1;
                try
                {
                    PageNumAsInt = Int32.Parse(PageNum);
                    if (to != null)
                    {
                        w.WriteLine("DEBUG: Adding Document to Dictionary: " + PageNumAsInt);
                        DocumentContent.Add(PageNumAsInt, to.getText());
                    }
                    else
                    {
                        w.WriteLine("DEBUG: ERROR, Could not add Document to Dictionary (Tesseract Output seems empty?): " + PageNumAsInt);
                    }
                }
                catch(Exception e)
                {
                    // do nothing and quit?
                    w.WriteLine("DEBUG: Error in Page Number Retrieved from File Name (it isnt a valid Integer?): " + PageNum);
                }


                //Console.ReadKey();
                w.Flush();
            }
            w.Close();
            // Delete Temp folder if not in Debug mode
            if (!Debug) { System.IO.Directory.Delete(TempPath,true); }

            return DocumentContent;
            
        }
    }

}
