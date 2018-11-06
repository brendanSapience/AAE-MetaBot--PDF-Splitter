using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace PDfSplitLib
{
    class TesseractUtils
    {
        String TessDataPath = "";
        String TessLanguage = "";

        public TesseractUtils(String PathToTessDataFiles, String LanguageCode)
        {
            this.TessDataPath = PathToTessDataFiles; // should be a path to a folder, not an individual file
            this.TessLanguage = LanguageCode; // should be 3 characters
        }



        // Processes exactly one page, returns data on 1 single page
        public TesseractOutput OCRImageFile(String PathToPngFile)
        {

            try
            {
                var testImagePath = PathToPngFile;
                using (var engine = new TesseractEngine(this.TessDataPath, this.TessLanguage, EngineMode.Default))
                {
                    using (var img = Pix.LoadFromFile(testImagePath))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Console.WriteLine("\nDEBUG: Tesseract Mean Confidence: {0}", page.GetMeanConfidence());

                            TesseractOutput to = new TesseractOutput(page.GetMeanConfidence(), text);
                            return to;
                                
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Console.WriteLine("\nUnexpected Error: " + e.Message);
                Console.WriteLine("\nDetails: ");
                Console.WriteLine(e.ToString());
                Console.ReadKey();
                return null;
            }

        }
    }

    class TesseractOutput
    {
        //int PageNumber = -1; Page Number will be used as the dict entry
        float ConfidenceLevel = -1;
        String PageContent = "";

        public TesseractOutput(float ConfidenceLevel, String PageContent)
        {
            this.ConfidenceLevel = ConfidenceLevel;
            this.PageContent = PageContent;
        }

        public String getText() { return this.PageContent; }
   
    }

}
