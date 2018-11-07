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


        /*
        public ConfigItem LoadJson()
        {
            using (StreamReader r = new StreamReader(@".\config.json"))
            {
                string json = r.ReadToEnd();
                ConfigItem item = JsonConvert.DeserializeObject<ConfigItem>(json);
                return item;
            }
        }

        public class ConfigItem
        {

            public string tessdata;

        }
        */
        public Dictionary<String, String> GetDictionaryFromPdf(String RootPath, String FileName, String PathToTessData, String Language, Boolean Debug)
        {
            // Split PDF into individual Pages using PDFSharp (PDFSharp in NuGet Package Manager)
            Dictionary<String, String> DocumentContent = new Dictionary<String, String>();

            //String RootPath = @"C:\dev\docs\";
            // "90000081.pdf"

            String TempPath = RootPath + guid + @"\";

            System.IO.Directory.CreateDirectory(TempPath);

            PdfSharpUtils psu = new PdfSharpUtils();
            psu.SplitAllPDFPages(RootPath, FileName, TempPath, Debug);

            // Now convert each pdf file into a png file
            // requires ImageMagick Wrapper for C# (Magick.NET Q16 Any CPU in NuGet Package manager)
            // because we do PDF conversions, the target system on which this is running requires install GhostScript

            ImageMagickUtils imu = new ImageMagickUtils(300, 300);

            string[] PdfFiles = Directory.GetFiles(TempPath, "*.pdf", SearchOption.TopDirectoryOnly);
            foreach (string filepath in PdfFiles)
            {
                imu.ConvertPDFToPng(filepath, filepath + ".png");
            }

            // Now using Tesseract to OCR the single pdf page turned into a png file (Tesseract in NuGet Package Manager)
            // this involves downloading the Tesseract language data files locally (see tessdata below).
            //ConfigItem item = LoadJson();

            TesseractUtils tu = new TesseractUtils(PathToTessData, Language);

            string[] PngFiles = Directory.GetFiles(TempPath, "*.png", SearchOption.TopDirectoryOnly);
            foreach (string filepath in PngFiles)
            {
                if (Debug) { Console.Write("\nDEBUG: Processing Temp Image File: " + filepath); }

                String filename = Path.GetFileName(filepath);
                String[] tempArr = filename.Split('_');
                String PageNum = tempArr[0];
                if (Debug) { Console.Write("\nDEBUG: Current Page Number: " + PageNum); }


                TesseractOutput to = tu.OCRImageFile(filepath, Debug);
                //Console.Write(to.getText());
                DocumentContent.Add(PageNum, to.getText());
                //Console.ReadKey();

            }

            // Delete Temp folder if not in Debug mode
            if (!Debug) { System.IO.Directory.Delete(TempPath,true); }

            return DocumentContent;
            
        }
    }

}
