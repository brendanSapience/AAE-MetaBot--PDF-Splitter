using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PDfSplitLib
{
    class F1
    {
        public F1() { }
        public String GetListOfPagesContainingString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName);
            List<String> ListOfPages = new List<String>();
            foreach (KeyValuePair<String, String> entry in myDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;
                if (PageContent.Contains(StringToFind))
                {
                    ListOfPages.Add(entry.Key);
                }

            }
            var result = String.Join(",", ListOfPages.ToArray());
            return result;
        }

        public String GetRangesFromKeywordAndPageCount(String PDFFilePath, String PDFFileName, String Keyword)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName);

            List<String> ListOfRanges = new List<String>();

            foreach (KeyValuePair<String, String> entry in myDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;
                if (PageContent.Contains(Keyword))
                {
                    //ListOfPages.Add(entry.Key);
                    var pattern = @"page[ ]*([\d]+)[ ]*of[ ]*([\d]+)";
                    RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
                    Console.Write("\nDebug: Page " + entry.Key);
                    // There should only be 1 Match (one instance of Page of.*)
                    foreach (Match match in Regex.Matches(PageContent, pattern, options))
                    {

                        Console.Write("\nMatch Found.");
                        // there should be 3 groups, group 0, the first page num and the last page num
                        if (match.Groups.Count == 3)
                        {
                            Console.Write("\n 3 Matches Found.");
                            String CurrentPage = match.Groups[1].ToString();
                            String EndPage = match.Groups[2].ToString();

                            int CurrentPageNum = Int32.Parse(CurrentPage);
                            int EndPageNum = Int32.Parse(EndPage);
                            Console.Write("\n Debug: "+ CurrentPageNum+":"+ EndPageNum);
                            if (CurrentPageNum == 1)
                            {
                                
                                int EndIndex = Int32.Parse(entry.Key)+ EndPageNum-1;
                                ListOfRanges.Add(entry.Key + "-" + EndIndex);
                                Console.Write("Adding Range: " + entry.Key + "-" + EndIndex);
                            }
                        } 
                    }
                }

            }
            var result = String.Join(",", ListOfRanges.ToArray());
            return result;

        }
    }
}