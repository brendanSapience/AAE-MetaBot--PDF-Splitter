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
        public Dictionary<int, String> GlobalFileContentDict = new Dictionary<int, String>();
        public String PathToTesseractDataFiles = "";
        public String LanguageCode = "";
        public F1() { }

        public void LoadPdfFileContent(String RootPath, String FileName, String PathToTessData, String Language, Boolean Debug)
        {
            PdfUtils pu = new PdfUtils();
            this.PathToTesseractDataFiles = PathToTessData;
            this.LanguageCode = Language;
            Dictionary<int, String> myDict = pu.GetDictionaryFromPdf(RootPath, FileName, this.PathToTesseractDataFiles, this.LanguageCode, Debug);
            this.GlobalFileContentDict = myDict;

        }

        public List<int> FindPageWithString(String PDFFilePath, String PDFFileName, String StringToFind)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<int, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng",false);
            List<int> l = new List<int>();

            foreach (KeyValuePair<int, String> entry in myDict)
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
            Dictionary<int, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", false);
            List<int> ListOfPages = new List<int>();
            foreach (KeyValuePair<int, String> entry in myDict)
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

        public String GetRangesFromRegexGroup(String RegexWithRepeatedGroup, Boolean Debug)
        {

            Dictionary<int, String> GlobalPdfDictionary = this.GlobalFileContentDict;

            // Container for ALL ranges of single multipage documents. Ex: 1,2;3,4,5 => 2 documents (1,2) and (3,4,5)
            List<String> ListOfRanges = new List<String>();

            int DocumentSize = GlobalPdfDictionary.Count();
            int CurrentIndex = 1;
            if (Debug) { Console.WriteLine("\n Debug: Processing Index: " + CurrentIndex); }
            //Console.Write("Debug Idx vs docSize: " + CurrentIndex + ":" + DocumentSize);

            // Iterating over each page of the document in order
            while (CurrentIndex <= DocumentSize)
            {
                String PageContent = GlobalPdfDictionary[CurrentIndex];

                var pattern = RegexWithRepeatedGroup;
                RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline; //RightToLeft: from last part of doc to first
                if (Debug)
                {
                    Console.WriteLine("\nDebug: Page " + CurrentIndex);
                    Console.WriteLine("\n ======= Page Content ==========");
                    //Console.WriteLine("\n" + PageContent);
                    Console.WriteLine("\n ======= End Of Page Content ==========");
                }

                // We check the Regex (with Group) against the content of this page
                MatchCollection allMatchesOnpage = Regex.Matches(PageContent, pattern, options);
                Match match = GetFirstMatch(allMatchesOnpage);

                // If there is no match, we go to the next page
                if (match == null)
                {
                    if (Debug) { Console.WriteLine("\nNo Match Found, Skipping Current Page: " + CurrentIndex); }
                    CurrentIndex++;
                    continue;
                }

                // If there is a Match, we try to extract the Group
                if (Debug) { Console.WriteLine("\nMaster Match Found on Page: " + CurrentIndex); }
                // there should be 2 groups, group 0, the element expected on other pages
                if (match.Groups.Count == 2)
                {
                    // This contains the Group which will be matched on subsequent pages
                    String RepeatingPattern = match.Groups[1].ToString();

                    if (Debug)
                    {
                        Console.WriteLine("\n 2 Group Matches Found.");
                        Console.WriteLine("\n Repeating Pattern: " + RepeatingPattern);
                    }

                    // We can start a range here: the current Page matched the Regex
                    String DocRange = "";
                    int PageNumSTART = CurrentIndex;
                    DocRange = DocRange + PageNumSTART;

                    // We start the search for subsequent pages from the next page
                    int InnerLoopIdx = CurrentIndex + 1;
                    Boolean ExitLoop = false;

                    // We continue processing pages until the end of the doc OR until the exitloop is triggered
                    while (InnerLoopIdx <= DocumentSize && !ExitLoop)
                    {
                        // Taking a look at the next page (inner loop)
                        int PageToAnalyze = InnerLoopIdx;
                        if (Debug) { Console.WriteLine("\nAnalysis Minor Page: " + InnerLoopIdx); }
                        if (Debug) { Console.WriteLine("\nIF LOOP Analysis Minor Page: Checking if: " + PageToAnalyze + ">" + CurrentIndex); }

                        String PageContent1 = GlobalPdfDictionary[InnerLoopIdx];

                        // Trying to check for a match on the next page
                        MatchCollection TempMatchColl = Regex.Matches(PageContent1, pattern, options);
                        Match match1 = GetFirstMatch(TempMatchColl);

                        // If there is no match at all on the next page, we return to the main loop and start again on the same page
                        if (match1 == null || !PageContent1.Contains(RepeatingPattern))
                        {
                            if (Debug) { Console.WriteLine("\nNO Minor Match Found on Page: " + InnerLoopIdx); }
                            CurrentIndex++;
                            ExitLoop = true;
                            break;
                        }

                        // If there is a Match in minor Page AND repeating pattern found
                        if (PageContent1.Contains(RepeatingPattern))
                        {
                            // record PageNum1
                            if (Debug) { Console.WriteLine("\nMinor Match Found on Page: " + InnerLoopIdx); }

                            // we add the new page to existing Range.
                            DocRange = DocRange + "," + InnerLoopIdx;
                            // we go to the next page and continue the inner loop
                            InnerLoopIdx++;
                            CurrentIndex = InnerLoopIdx;
                            ExitLoop = false;
                            continue;
                        }

                    }
                    // Adding the range to the list of ranges
                    ListOfRanges.Add(DocRange);


                }
                else
                {
                    Console.WriteLine("\n Master Match found, but 2 Groups could not be found, skipping.");
                    CurrentIndex++;
                    continue;
                }

                //   CurrentIndex++;
            }
            var result = String.Join(";", ListOfRanges.ToArray());
            return result;


        }

        public String GetRangesFromRegexGroup(String PDFFilePath, String PDFFileName, String RegexWithRepeatedGroup, Boolean Debug)
        {
            PdfUtils pu = new PdfUtils();
            Dictionary<int, String> GlobalPdfDictionary = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", true);

            // Container for ALL ranges of single multipage documents. Ex: 1,2;3,4,5 => 2 documents (1,2) and (3,4,5)
            List<String> ListOfRanges = new List<String>();

            int DocumentSize = GlobalPdfDictionary.Count();
            int CurrentIndex = 1;
            if (Debug) { Console.WriteLine("\n Debug: Processing Index: " + CurrentIndex); }
            //Console.Write("Debug Idx vs docSize: " + CurrentIndex + ":" + DocumentSize);

            // Iterating over each page of the document in order
            while(CurrentIndex <= DocumentSize)
            {
                String PageContent = GlobalPdfDictionary[CurrentIndex];

                var pattern = RegexWithRepeatedGroup;
                RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline; //RightToLeft: from last part of doc to first
                if (Debug)
                {
                    Console.WriteLine("\nDebug: Page " + CurrentIndex);
                    Console.WriteLine("\n ======= Page Content ==========");
                    //Console.WriteLine("\n" + PageContent);
                    Console.WriteLine("\n ======= End Of Page Content ==========");
                }

                // We check the Regex (with Group) against the content of this page
                MatchCollection allMatchesOnpage = Regex.Matches(PageContent, pattern, options);
                Match match = GetFirstMatch(allMatchesOnpage);

                // If there is no match, we go to the next page
                if (match == null)
                {
                    if (Debug) { Console.WriteLine("\nNo Match Found, Skipping Current Page: " + CurrentIndex); }
                    CurrentIndex++;
                    continue;
                }

                // If there is a Match, we try to extract the Group
                if (Debug) { Console.WriteLine("\nMaster Match Found on Page: " + CurrentIndex); }
                // there should be 2 groups, group 0, the element expected on other pages
                if (match.Groups.Count == 2)
                {
                    // This contains the Group which will be matched on subsequent pages
                    String RepeatingPattern = match.Groups[1].ToString();

                    if (Debug)
                    {
                        Console.WriteLine("\n 2 Group Matches Found.");
                        Console.WriteLine("\n Repeating Pattern: " + RepeatingPattern);
                    }

                    // We can start a range here: the current Page matched the Regex
                    String DocRange = "";
                    int PageNumSTART = CurrentIndex;
                    DocRange = DocRange + PageNumSTART;

                    // We start the search for subsequent pages from the next page
                    int InnerLoopIdx = CurrentIndex+1;
                    Boolean ExitLoop = false;

                    // We continue processing pages until the end of the doc OR until the exitloop is triggered
                    while(InnerLoopIdx <= DocumentSize && !ExitLoop)
                    {
                        // Taking a look at the next page (inner loop)
                        int PageToAnalyze = InnerLoopIdx;
                        if (Debug) { Console.WriteLine("\nAnalysis Minor Page: " + InnerLoopIdx); }
                        if (Debug) { Console.WriteLine("\nIF LOOP Analysis Minor Page: Checking if: " + PageToAnalyze + ">" + CurrentIndex); }

                        String PageContent1 = GlobalPdfDictionary[InnerLoopIdx];

                       // Trying to check for a match on the next page
                        MatchCollection TempMatchColl = Regex.Matches(PageContent1, pattern, options);
                        Match match1 = GetFirstMatch(TempMatchColl);

                        // If there is no match at all on the next page, we return to the main loop and start again on the same page
                        if (match1 == null || !PageContent1.Contains(RepeatingPattern))
                        {
                            if (Debug) { Console.WriteLine("\nNO Minor Match Found on Page: " + InnerLoopIdx); }
                            CurrentIndex++;
                           ExitLoop = true;
                           break;
                        }

                        // If there is a Match in minor Page AND repeating pattern found
                        if (PageContent1.Contains(RepeatingPattern))
                        {
                            // record PageNum1
                            if (Debug) { Console.WriteLine("\nMinor Match Found on Page: " + InnerLoopIdx); }
                            
                            // we add the new page to existing Range.
                            DocRange = DocRange + "," + InnerLoopIdx;
                            // we go to the next page and continue the inner loop
                            InnerLoopIdx++;
                            CurrentIndex = InnerLoopIdx;
                            ExitLoop = false;
                            continue;
                        }

                    }
                    // Adding the range to the list of ranges
                    ListOfRanges.Add(DocRange);


                }
                else
                {
                    Console.WriteLine("\n Master Match found, but 2 Groups could not be found, skipping.");
                    CurrentIndex++;
                    continue;
                }

             //   CurrentIndex++;
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
            Dictionary<int, String> myDict = pu.GetDictionaryFromPdf(PDFFilePath, PDFFileName, @"C:\dev\tessdata", "eng", false);

            List<String> ListOfRanges = new List<String>();

            foreach (KeyValuePair<int, String> entry in myDict)
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
                        Console.WriteLine("\nDebug: Page " + entry.Key);
                        Console.WriteLine("\n ======= Page Content ==========");
                        Console.WriteLine("\n" + PageContent);
                        Console.WriteLine("\n ======= End Of Page Content ==========");
                    }
                    // There should only be 1 Match (one instance of Page of.*)
                    foreach (Match match in Regex.Matches(PageContent, pattern, options))
                    {

                        if (Debug) { Console.WriteLine("\nMatch Found."); }
                        // there should be 3 groups, group 0, the first page num and the last page num
                        if (match.Groups.Count == 3)
                        {
                            if (Debug)
                            { Console.WriteLine("\n 3 Matches Found."); }
                            String CurrentPage = match.Groups[1].ToString();
                            String EndPage = match.Groups[2].ToString();

                            int CurrentPageNum = Int32.Parse(CurrentPage);
                            int EndPageNum = Int32.Parse(EndPage);
                            if (Debug)
                            { Console.WriteLine("\n Debug: " + CurrentPageNum + ":" + EndPageNum); }
                            if (CurrentPageNum == 1)
                            {
                                
                                int EndIndex = entry.Key+ EndPageNum-1;
                                ListOfRanges.Add(entry.Key + "-" + EndIndex);
                                if (Debug)
                                { Console.WriteLine("Adding Range: " + entry.Key + "-" + EndIndex); }
                            }
                        } 
                    }
                }

            }
            var result = String.Join(",", ListOfRanges.ToArray());
            return result;

        }

        private Match GetFirstMatch(MatchCollection c)
        {
            List<Match> allmatches = new List<Match>();

            foreach(Match m in c)
            {
                allmatches.Add(m);
            }

            if (allmatches.Count == 0) { return null; }
            return allmatches[0];
        }

        private Match GetLastMatch(MatchCollection c)
        {
            List<Match> allmatches = new List<Match>();

            foreach (Match m in c)
            {
                allmatches.Add(m);
            }
            if (allmatches.Count == 0) { return null; }
            return allmatches[allmatches.Count()-1];
        }

    }


}