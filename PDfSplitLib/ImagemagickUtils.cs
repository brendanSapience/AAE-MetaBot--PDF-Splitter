using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    class ImageMagickUtils
    {
        double xRes = -1;
        double yRes = -1;
        //String PDFFilePath = "";
        MagickReadSettings imsettings = new MagickReadSettings();
        public ImageMagickUtils(double x, double y)
        {
            this.xRes = x;
            this.yRes = y;

            // Settings the density to 300 dpi will create an image with a better quality
            this.imsettings.Density = new Density(this.xRes, this.yRes);

        }

            public void ConvertPDFToPng(String PDFFilePath, String PNGFilePath)
            {
                using (MagickImageCollection images = new MagickImageCollection())
                {
                    // Add all the pages of the pdf file to the collection
                    images.Read(PDFFilePath, this.imsettings);

                    //int page = 1;
                    foreach (MagickImage image in images)
                    {
                        // Write page to file that contains the page number
                        image.Write(PNGFilePath);
                        // Writing to a specific format works the same as for a single image
                        //image.Format = MagickFormat.Ptif;
                        //image.Write(@"C:\dev\docs\split\90000081 - Page 1_tempfile" + page + ".tif");
                       // page++;
                    }
                }
        }

        
    }
}
