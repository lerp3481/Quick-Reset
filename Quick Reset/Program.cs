using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public struct ProgramSettings
    {
        public enum TempoMode
        {
            STATIC, //instantaneous tempo change
            DYNAMIC_START, //start of accel or rit
            DYNAMIC_END //end of accel or rit
        }

        public int absoluteStartCount; //STARTS WITH 1!
        public int tempo;
        public TempoMode tempoMode;

        public override string ToString()
        {
            string ret = "";
            /*
            int i = absoluteStartCount - 1;
            int j = 0;
            Set currSet = Drill.instance.sets[j];
            Set nextSet;

            while (i > 0)
            {
                currSet = Drill.instance.sets[j];
                try
                {
                    nextSet = Drill.instance.sets[j + 1];
                }
                catch
                {
                    break;
                }

                if (nextSet.counts < i)
                {
                    i -= nextSet.counts;
                }
                else
                {
                    break;
                }
                j++;
            }*/
            /*
            int thisSet = 0;
            while (Drill.instance.playbackManager.CalculateAbsoluteCount(thisSet) < absoluteStartCount)
            {
                thisSet++;
            }
            thisSet--;
            int countsPrevious = Drill.instance.playbackManager.CalculateAbsoluteCount(thisSet);

            return "Set " + Drill.instance.sets[thisSet].set + " count " + (countsPrevious - absoluteStartCount + 1);*/
            int countsThisSet = 0, countsNextSet = 0;
            int set = -1;
            while (!(absoluteStartCount >= countsThisSet && absoluteStartCount < countsNextSet))
            {
                set++;
                countsThisSet = Drill.instance.playbackManager.CalculateAbsoluteCount(set);
                countsNextSet = Drill.instance.playbackManager.CalculateAbsoluteCount(set + 1);
            }

            return "Set " + Drill.instance.sets[set].set + " count " + (absoluteStartCount - countsThisSet + 1) + " (" + absoluteStartCount + ")";
        }
    }
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
