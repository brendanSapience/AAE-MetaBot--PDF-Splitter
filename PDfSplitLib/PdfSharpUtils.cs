﻿using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    class PdfSharpUtils
    {

        StreamWriter w;
        public PdfSharpUtils(StreamWriter w)
        {
            this.w = w;
        }

        public void SplitAllPDFPages(String PathToFolderContainingPDF, String PDFFileName, String PathToOutputFolder,Boolean Debug)
        {

            // Get a fresh copy of the sample PDF file
            //string filename = PDFFileName;
            //File.Copy(Path.Combine(PathToFolderContainingPDF, filename),Path.Combine(Directory.GetCurrentDirectory(), filename), true);

            String CompleteFilePath = Path.Combine(PathToFolderContainingPDF, PDFFileName);
            if (Debug){ Console.WriteLine("\nDEBUG: File Being Processed: " + CompleteFilePath); }
            w.WriteLine("DEBUG - FIle PDF Split - File Being Processed: " + CompleteFilePath);
            // Open the file
            PdfDocument inputDocument = PdfReader.Open(Path.Combine(PathToFolderContainingPDF, PDFFileName), PdfDocumentOpenMode.Import);

            string name = Path.GetFileNameWithoutExtension(PDFFileName);
            for (int idx = 0; idx < inputDocument.PageCount; idx++)
            {
                // Create new document
                //Console.Write("Debug: " + idx);
                PdfDocument outputDocument = new PdfDocument();
                //outputDocument.Version = inputDocument.Version;
                //outputDocument.Info.Title =String.Format("Page {0} of {1}", idx + 1, inputDocument.Info.Title);
                //outputDocument.Info.Creator = inputDocument.Info.Creator;

                // Add the page and save it
                outputDocument.AddPage(inputDocument.Pages[idx]);
                String Str = String.Format("{1}_{0}_tempfile.pdf", name, idx + 1);
                if (Debug){ Console.WriteLine("\nDEBUG: Temp File Name Generated: " + Str); }
                w.WriteLine("DEBUG - FIle PDF Split - File Generated: " + Str);
                outputDocument.Save(PathToOutputFolder + Str);
                w.Flush();
            }
            w.Flush();

        }
    }
}
