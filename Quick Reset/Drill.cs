using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class Drill
    {
        public List<Set> sets;
        public List<Performer> performers;
        public PlaybackManager playbackManager;

        public int currentSet;
        public bool showPerformerLabels;

        public static Drill instance;

        public Drill()
        {
            sets = new List<Set>();
            performers = new List<Performer>();
            playbackManager = new PlaybackManager();
        }

        public float GetSuggestedXExcess()
        {
            float ret = 0;
            foreach (Performer p in performers)
            {
                foreach (Dot d in p.dots)
                {
                    if (Math.Abs(d.lefttoright) > Field.FIELD_LENGTH / 2)
                    {
                        //This dot goes beyond the field
                        float distanceOff = Math.Abs(d.lefttoright) - Field.FIELD_LENGTH / 2;
                        if (distanceOff > ret)
                        {
                            ret = distanceOff;
                        }
                    }
                }
            }
            return ret;
        }
        public float GetSuggestedYExcess()
        {
            float ret = 0;
            foreach (Performer p in performers)
            {
                foreach (Dot d in p.dots)
                {
                    if (Math.Abs(d.fronttoback) > Field.FIELD_WIDTH / 2)
                    {
                        //This dot goes beyond the field
                        float distanceOff = Math.Abs(d.fronttoback) - Field.FIELD_WIDTH / 2;
                        if (distanceOff > ret)
                        {
                            ret = distanceOff;
                        }
                    }
                }
            }
            return ret;
        }
        public Performer[] GetSurroundingPerformers(Performer p, float radius, int set, int count, float progress)
        {
            //p is who to check around
            //radius is how many steps around p to check
            //set is which set we're in
            //count is what count of the set we're starting in (displaycount (starts at 1))
            //progress is how far we're in to the next count
            //returns any performers that are within that search radius
            List<Performer> ret = new List<Performer>();

            Dot pDot = p.GetInBetweenDot(set, count, progress);

            for (int i = 0; i < performers.Count; i++)
            {
                if (p.label == performers[i].label)
                {
                    continue;
                }

                Performer t = performers[i];

                Dot tDot = t.GetInBetweenDot(set, count, progress);

                float dx = tDot.lefttoright - pDot.lefttoright;
                float dy = tDot.fronttoback - pDot.fronttoback;

                float distance = (float)Math.Sqrt((float)(dx * dx + dy * dy));

                if (distance <= radius)
                {
                    ret.Add(p);
                }
            }

            return ret.ToArray();
        }
        public Performer GetPerformerByLabel(string label)
        {
            return performers.First(x => x.label == label);
        }

        public Set GetSetByName(string setName)
        {
            return sets.First(x => x.set == setName);
        }

        public bool DoesSetExist(string setname)
        {
            return sets.Any(x => x.set == setname);
        }

        public void AddDot(string setname, int counts)
        {
            sets.Add(new Set(setname, counts, sets.Count()));
        }

        public int NextSet()
        {
            if (currentSet + 1 < sets.Count)
            {
                currentSet++;
            }
            return currentSet;
        }

        public int PreviousSet()
        {
            if (currentSet != 0)
            {
                currentSet--;
            }
            return currentSet;
        }

        public string GetCurrentSetName()
        {
            return sets[currentSet].set;
        }

        public string GetNextSetName()
        {
            try
            {
                return sets[currentSet + 1].set;
            }
            catch
            {
                return "NULL";
            }
        }

        public string[] RankPerformersByDifficulty()
        {
            List<string> ret = new List<string>();
            Dictionary<string, float> pScore = new Dictionary<string, float>();
            Dictionary<Set, List<float>> sScore = new Dictionary<Set, List<float>>();

            //First line (all sets + total)
            string firstLine = ",";
            //Omitting the last set because score can only be calculated if there's a next set
            for (int i = 0; i < sets.Count - 1; i++)
            {
                firstLine += sets[i].set + ",";
                sScore.Add(sets[i], new List<float>());
            }
            firstLine += "TOTAL";
            ret.Add(firstLine);

            foreach (Performer p in performers)
            {
                string pLine = p.label + ",";
                float totalScore = 0.0f;

                for (int i = 0; i < p.dots.Count - 1; i++)
                {
                    Dot thisDot = p.dots[i];
                    Dot nextDot = p.dots[i + 1];

                    float dx = nextDot.lefttoright - thisDot.lefttoright;
                    float dy = nextDot.fronttoback - thisDot.fronttoback;

                    float distance = (float)Math.Sqrt((dx * dx) + (dy * dy));

                    //float score = distance / (float)(sets[i + 1].counts);
                    float score = distance / Field.STANDARD_STEP_SIZE;
                    totalScore += score;
                    pLine += score + ",";
                    sScore[sets[i]].Add(score);
                }

                if (p.lastActiveSet != -1 && p.lastActiveSet < sets.Count)
                {
                    pLine = pLine.PadRight(pLine.Length + sets.Count - p.lastActiveSet - 1, ',');
                }

                pLine += totalScore;
                pScore.Add(pLine, totalScore);
                //ret.Add(pLine);
            }

            var sortedDict = from entry in pScore orderby entry.Value descending select entry;

            ret.AddRange(sortedDict.Select(x => x.Key));

            //Last line
            string lastLine = "MEDIAN,";
            //Omitting the last set because score can only be calculated if there's a next set
            for (int i = 0; i < sets.Count - 1; i++)
            {
                //Sort the values
                List<float> sortedVals = sScore[sets[i]].OrderBy(x => x).ToList();

                //Find median
                int numCount = sortedVals.Count();
                int halfIdx = sortedVals.Count() / 2;
                float median;
                if ((numCount % 2) == 0)
                {
                    median = ((sortedVals.ElementAt(halfIdx) + sortedVals.ElementAt(halfIdx - 1))) / 2f;
                }
                else
                {
                    median = sortedVals.ElementAt(halfIdx);
                }

                lastLine += median + ",";
            }
            //Find median of total scores
            List<float> scores = sortedDict.Reverse().Select(x => x.Value).ToList();
            int nCount = scores.Count();
            int hIdx = scores.Count() / 2;
            float m;
            if ((nCount % 2) == 0) m = ((scores.ElementAt(hIdx) + scores.ElementAt(hIdx - 1))) / 2f;
            else m = scores.ElementAt(hIdx);

            lastLine += m;
            ret.Add(lastLine);

            return ret.ToArray();
        }
    }
}
