using iText;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class PdfParser
    {
        PdfDocument pdf;

        public PdfParser(string filename)
        {
            pdf = new PdfDocument(new PdfReader(filename));
        }

        public Drill ConvertToDrill()
        {
            Drill ret = new Drill();

            //The origin is at the lower left
            //Page quadrants are 306x396
            Rectangle topLeftCard = new Rectangle(0, 396, 306, 396);
            Rectangle topRightCard = new Rectangle(306, 396, 306, 396);
            Rectangle bottomLeftCard = new Rectangle(0, 0, 306, 396);
            Rectangle bottomRightCard = new Rectangle(306, 0, 306, 396);

            Performer marcher = null;
            bool addedMarcher = false;
            for (int i = 1; i < pdf.GetNumberOfPages() + 1; i++)
            {
                PdfPage page = pdf.GetPage(i);

                string topLeftText = PdfTextExtractor.GetTextFromPage(page, new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(topLeftCard)));
                string topRightText = PdfTextExtractor.GetTextFromPage(page, new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(topRightCard)));
                string bottomLeftText = PdfTextExtractor.GetTextFromPage(page, new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(bottomLeftCard)));
                string bottomRightText = PdfTextExtractor.GetTextFromPage(page, new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(bottomRightCard)));

                string nextPageText = null;
                try
                {
                    nextPageText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(i + 1), new FilteredTextEventListener(new LocationTextExtractionStrategy(), new TextRegionEventFilter(topLeftCard)));
                }
                catch
                {
                    //This was the last page! No worries
                }

                string[] tlArray = topLeftText.Split(new string[] { "\n" }, StringSplitOptions.None);
                string[] trArray = topRightText.Split(new string[] { "\n" }, StringSplitOptions.None);
                string[] blArray = bottomLeftText.Split(new string[] { "\n" }, StringSplitOptions.None);
                string[] brArray = bottomRightText.Split(new string[] { "\n" }, StringSplitOptions.None);
                string[] nextPageArray = null;
                string nextPageStuff = null;
                if (nextPageText != null)
                {
                    nextPageArray = nextPageText.Split(new string[] { "\n" }, StringSplitOptions.None);
                    nextPageStuff = nextPageArray[2]; //Skip past the first part of performer info
                }

                string[][] array = new string[][] { tlArray, trArray, blArray, brArray, nextPageArray };

                for (int c = 0; c < 4; c++)
                {
                    if (array[c][0].Length == 0)
                    {
                        continue;
                    }
                    for (int j = 0; j < array[c].Length; j++)
                    {
                        string line = array[c][j];

                        if (line.StartsWith("Performer: "))
                        {
                            //New performer!
                            int labelStart = line.IndexOf("Label: ") + 7;
                            string label = "";
                            while (true)
                            {
                                char a = line[labelStart];
                                if (a == ' ') break;
                                label += a;
                                labelStart++;
                            }

                            if (!addedMarcher && marcher != null)
                            {
                                if (marcher.label != label)
                                {
                                    ret.performers.Add(marcher);
                                    addedMarcher = true;
                                    marcher = new Performer(label);
                                }
                            }
                            if (marcher == null)
                            {
                                marcher = new Performer(label);
                            }
                            Debug.WriteLine("Performer " + marcher.label);
                            addedMarcher = false;
                        }
                        else if (line.StartsWith("Set Measure Counts "))
                        {
                            //Useless line
                            continue;
                        }
                        else if (line.StartsWith("Printed: "))
                        {
                            //Only adds if it's the end of the PDF
                            if (array[c + 1] == null || array[c + 1][0].Length == 0)
                            {
                                ret.performers.Add(marcher);
                                addedMarcher = true;
                            }
                        }
                        else
                        {
                            if (!char.IsDigit(line[0]))
                            {
                                //This can only happen when this is a continuation from another drill sheet
                                continue;
                            }
                            //Drill data!
                            //Sometimes the coordinate goes over the line so a new line is made...
                            string newLine = "";
                            try
                            {
                                newLine = array[c][j + 1];
                            }
                            catch
                            {
                                try
                                {
                                    string next = array[c + 1][1];
                                    if (char.IsDigit(next[0]) || next.StartsWith("Set"))
                                    {
                                        newLine = "";
                                    }
                                    else
                                    {
                                        newLine = next;
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                            if (char.IsDigit(newLine[0]) || newLine.StartsWith("Printed: "))
                            {
                                bool b = newLine.StartsWith("Printed: ");
                                newLine = "";

                                //Let's check if there's more text on the next sheet
                                if (b)
                                {
                                    string next = null;
                                    try
                                    {
                                        if (array[c + 1] != null)
                                        {
                                            next = array[c + 1][1];
                                            if (char.IsDigit(next[0]) || next.StartsWith("Set"))
                                            {
                                                newLine = "";
                                            }
                                            else
                                            {
                                                newLine = next;
                                            }

                                            //But wait! Is the next line a duplicate?
                                            if (line.Split(' ')[0] == next.Split(' ')[0])
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        newLine = "";
                                    }
                                }
                            }
                            else
                            {
                                //Great, now don't try to parse the next line
                                j++;
                            }

                            marcher.dots.Add(GetDrill(line, newLine, ret));
                        }
                    }
                }
            }

            //Mr. Seneca didn't define set 34A in gold rush...
            if (ret.sets.Last().set == "34")
            {
                //Add the set
                Set set34A = ret.sets.Last();
                set34A.privateID++;
                set34A.set = "34A";
                set34A.counts = 15;

                ret.sets.Add(set34A);

                //Let the performers know...
                for (int i = 0; i < ret.performers.Count; i++)
                {
                    Dot lastDot = ret.performers[i].dots.Last();
                    ret.performers[i].dots.Add(lastDot);
                }
            }

            return ret;
        }

        public Dot GetDrill(string text, string remainderText, Drill drill)
        {
            Dot ret = new Dot();

            // Mr. Seneca moment - POP! part 3, performer G3
            // Deal with "BEHIND -5" front-to-backs
            if (text.Contains("BEHIND -5"))
            {
                string[] textParts = text.Split(' ');
                float fbOffset = float.Parse(textParts[textParts.Length - 5]);

                float newOffset = 10 - fbOffset;
                textParts[textParts.Length - 5] = newOffset.ToString();
                text = string.Join(" ", textParts);
                text = text.Replace("  BEHIND -5", " IN FRONT FRONT SIDELINE");
            }
            // text = text.Replace("3.0  BEHIND -5", "7.0 IN FRONT FRONT SIDELINE");

            string setName = text.Split(' ')[0];
            Debug.WriteLine(setName);

            int lrIdx = text.IndexOf("SIDE 1: On 50 YARD LINE");
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("SIDE 2: On 50 YARD LINE");
            }
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("On 50 YARD LINE");
            }
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("SIDE 1: On 50");
            }
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("SIDE 2: On 50");
            }
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("On 50");
            }
            if (lrIdx == -1)
            {
                lrIdx = text.IndexOf("SIDE ");
            }

            List<char> cnts = new List<char>();

            for (int i = lrIdx - 2; i > 0; i--)
            {
                if (char.IsNumber(text[i]))
                {
                    cnts.Add(text[i]);
                }
                else
                {
                    break;
                }
            }
            cnts.Reverse();

            int setCounts = int.Parse(new string(cnts.ToArray()));
            if (!drill.DoesSetExist(setName))
            {
                drill.AddDot(setName, setCounts);
            }

            int lrEndIdx = text.IndexOf("YARD LINE") + 9;
            if (lrEndIdx == 8)
            {
                //-1 + 9 = 8; there is no "YARD LINE"

                //Next approach:
                //Split the text through ' ' and check if there's two numbers back to back
                List<string> textParts = text.ToLower().Split(' ').ToList();
                int lastLRIndex = -1;
                bool hasSeenWordsYet = false;
                for (int i = 0; i < textParts.Count - 1; i++)
                {
                    if (textParts[i].Length == 0) continue;
                    if (!hasSeenWordsYet)
                    {
                        hasSeenWordsYet = char.IsLetter(textParts[i][0]);
                        continue;
                    }
                    float num1, num2;
                    if (float.TryParse(textParts[i], out num1))
                    {
                        int nextPartInt = 1;
                        while (textParts[i + nextPartInt].Length == 0)
                        {
                            nextPartInt++;
                        }
                        if (float.TryParse(textParts[i + nextPartInt], out num2) || textParts[i + nextPartInt] == "on")
                        {
                            //Two numbers back to back!
                            //...or number + "On"
                            lastLRIndex = i + 1;
                        }
                    }
                }

                textParts.RemoveRange(lastLRIndex, textParts.Count - lastLRIndex);
                lrEndIdx = string.Join(" ", textParts.ToArray()).Length;
            }
            string lrStr = text.Substring(lrIdx, lrEndIdx - lrIdx);
            ret.lefttoright = Dot.LeftToRightFromString(lrStr.ToLower());

            char meer = text[lrEndIdx];
            while (meer == ' ')
            {
                lrEndIdx++;
                meer = text[lrEndIdx];
            }
            string fbStr = text.Substring(lrEndIdx) + remainderText;
            ret.fronttoback = Dot.FrontToBackFromString(fbStr.ToLower());

            return ret;
        }
    }
}
