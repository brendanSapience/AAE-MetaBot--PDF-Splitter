using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    class ImageMagickUtils
    {
        double xRes = -1;
        double yRes = -1;
        StreamWriter w;
        //String PDFFilePath = "";
        MagickReadSettings imsettings = new MagickReadSettings();
        public ImageMagickUtils(double x, double y, StreamWriter w)
        {
            this.xRes = x;
            this.yRes = y;
            this.w = w;

            // Settings the density to 300 dpi will create an image with a better quality
            this.imsettings.Density = new Density(this.xRes, this.yRes);

        }

            public void ConvertPDFToPng(String PDFFilePath, String PNGFilePath)
            {
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    // Add all the pages of the pdf file to the collection
                    images.Read(PDFFilePath, this.imsettings);
                    w.WriteLine("DEBUG - PDF to PNG Conversion Starting.");
                //int page = 1;
                foreach (MagickImage image in images)
                    {
                    // Write page to file that contains the page number
                    w.WriteLine("DEBUG - FIle PDF Split - FIle Being Generated: " + PNGFilePath);
                    image.Write(PNGFilePath);
                    w.Flush();
                    }
                    w.Flush();
                }
        }

        
    }
}
