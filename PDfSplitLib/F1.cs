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

        public List<String> FindPageWithString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng",false);
            List<String> l = new List<String>();

            foreach (KeyValuePair<String, String> entry in myDict)
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

        public String GetListOfPagesContainingString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", false);
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

        public String GetRangesFromRegexGroup(String PDFFilePath, String PDFFileName, String RegexWithRepeatedGroup, Boolean Debug)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", true);

            List<String> ListOfRanges = new List<String>();


            foreach (KeyValuePair<String, String> entry in myDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;

                    //ListOfPages.Add(entry.Key);
                    var pattern = RegexWithRepeatedGroup;
                    RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
                    if (Debug)
                    {
                        Console.Write("\nDebug: Page " + entry.Key);
                        Console.Write("\n ======= Page Content ==========");
                        Console.Write("\n" + PageContent);
                        Console.Write("\n ======= End Of Page Content ==========");
                    }
                    // There should only be 1 Match (one instance of Page of.*)
                    foreach (Match match in Regex.Matches(PageContent, pattern, options))
                    {

                        if (Debug) { Console.Write("\nMatch Found."); }
                        // there should be 2 groups, group 0, the element expected on other pages
                        if (match.Groups.Count == 2)
                        {
                            String RepeatingPattern = match.Groups[1].ToString();
                            if (Debug)
                            {
                                Console.Write("\n 2 Group Matches Found.");
                                Console.Write("\n Repeating Pattern: " + RepeatingPattern);
                            }
                            

                            int PageNum = Int32.Parse(entry.Key);
                            Dictionary<String, String> MatchingDict = new Dictionary<String, String>();
                            foreach (KeyValuePair<String, String> tempentry in myDict)
                            {
                                int PageToAnalyze = Int32.Parse(tempentry.Key);
                                if (PageToAnalyze > PageNum)
                                {
                                    String PageContent1 = tempentry.Value;
                                    foreach (Match match1 in Regex.Matches(PageContent1, pattern, options))
                                    {
                                        if (PageContent1.Contains(RepeatingPattern))
                                        {
                                            MatchingDict.Add(tempentry.Key, tempentry.Value);
                                        }
                                    }
                                }
   
                            }

                            foreach (KeyValuePair<String, String> tempentry in MatchingDict)
                            {
                            ListOfRanges.Add(tempentry.Key);
                            }

                        }
                    }
                

            }
            var result = String.Join(";", ListOfRanges.ToArray());
            return result;


        }



        public String GetRangesFromKeywordAndPageCount(String PDFFilePath, String PDFFileName, String Keyword, Boolean Debug)
        {
            return GetRangesFromKeywordAndPageCount(PDFFilePath, PDFFileName, Keyword, @"page[ ]*([\d]+)[ ]*[o0][fli1][ ]*([\d]+)", Debug);
        }

        public String GetRangesFromKeywordAndPageCount(String PDFFilePath, String PDFFileName, String Keyword, String RegexWithGroupsForPageCount, Boolean Debug)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<String, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", false);

            List<String> ListOfRanges = new List<String>();

            foreach (KeyValuePair<String, String> entry in myDict)
            {
                //entry.Value entry.Key
                String PageContent = entry.Value;
                if (PageContent.Contains(Keyword))
                {
                    //ListOfPages.Add(entry.Key);
                    var pattern = RegexWithGroupsForPageCount;
                    RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
                    if (Debug)
                    {
                        Console.Write("\nDebug: Page " + entry.Key);
                        Console.Write("\n ======= Page Content ==========");
                        Console.Write("\n" + PageContent);
                        Console.Write("\n ======= End Of Page Content ==========");
                    }
                    // There should only be 1 Match (one instance of Page of.*)
                    foreach (Match match in Regex.Matches(PageContent, pattern, options))
                    {

                        if (Debug) { Console.Write("\nMatch Found."); }
                        // there should be 3 groups, group 0, the first page num and the last page num
                        if (match.Groups.Count == 3)
                        {
                            if (Debug)
                            { Console.Write("\n 3 Matches Found."); }
                            String CurrentPage = match.Groups[1].ToString();
                            String EndPage = match.Groups[2].ToString();

                            int CurrentPageNum = Int32.Parse(CurrentPage);
                            int EndPageNum = Int32.Parse(EndPage);
                            if (Debug)
                            { Console.Write("\n Debug: " + CurrentPageNum + ":" + EndPageNum); }
                            if (CurrentPageNum == 1)
                            {
                                
                                int EndIndex = Int32.Parse(entry.Key)+ EndPageNum-1;
                                ListOfRanges.Add(entry.Key + "-" + EndIndex);
                                if (Debug)
                                { Console.Write("Adding Range: " + entry.Key + "-" + EndIndex); }
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