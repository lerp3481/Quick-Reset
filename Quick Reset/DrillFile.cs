using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class DrillFile
    {
        public static void Save(Stream outFile)
        {
            BinaryWriter bw = new BinaryWriter(outFile);
            List<string> labels = new List<string>();

            bw.Write(0x4C4C5244); //Magic (DRLL)

            //Field settings
            bw.Write(0x4C454946); //Magic (FIEL)
            bw.Write(Field.STANDARD_STEP_SIZE);
            bw.Write(Field.X_EXCESS);
            bw.Write(Field.Y_EXCESS);
            bw.Write(Field.DRAW_SCALE_FACTOR);

            //Sets
            bw.Write(0x53544553); //Magic (SETS)
            bw.Write(Drill.instance.sets.Count);
            for (int i = 0; i < Drill.instance.sets.Count; i++)
            {
                bw.Write(Drill.instance.sets[i].counts);

                labels.Add(Drill.instance.sets[i].set);
            }

            //Performers
            bw.Write(0x524D4650); //Magic (PFMR)
            bw.Write(Drill.instance.performers.Count);
            for (int i = 0; i < Drill.instance.performers.Count; i++)
            {
                bw.Write(Drill.instance.performers[i].firstActiveSet);
                bw.Write(Drill.instance.performers[i].lastActiveSet);

                int lastSet = Drill.instance.performers[i].lastActiveSet == -1 ? Drill.instance.sets.Count : Drill.instance.performers[i].lastActiveSet + 1;

                for (int j = 0; j < (lastSet - Drill.instance.performers[i].firstActiveSet); j++)
                //for (int j = Drill.instance.performers[i].firstActiveSet; j < lastSet; j++)
                {
                    bw.Write(Drill.instance.performers[i].dots[j].lefttoright);
                    bw.Write(Drill.instance.performers[i].dots[j].fronttoback);
                }

                labels.Add(Drill.instance.performers[i].label);
            }

            //Program settings
            bw.Write(0x4D475250); //Magic (PRGM)
            bw.Write(Drill.instance.playbackManager.program.Count);
            for (int i = 0; i < Drill.instance.playbackManager.program.Count; i++)
            {
                bw.Write(Drill.instance.playbackManager.program[i].absoluteStartCount);
                bw.Write(Drill.instance.playbackManager.program[i].tempo);
                bw.Write((int)Drill.instance.playbackManager.program[i].tempoMode);
            }

            //Strings
            bw.Write(0x4C42414C); //Magic (LABL)
            for (int i = 0; i < labels.Count; i++)
            {
                bw.Write(labels[i]);
            }
        }

        public static void Load(Stream inFile)
        {
            BinaryReader br = new BinaryReader(inFile);

            string DRLL = new string(br.ReadChars(4)); //Magic
            string FIEL = new string(br.ReadChars(4)); //Magic
            float STANDARD_STEP_SIZE = br.ReadSingle();
            Field.X_EXCESS = br.ReadInt32();
            Field.Y_EXCESS = br.ReadInt32();
            float DRAW_SCALE_FACTOR = br.ReadSingle();

            Form1.instance.fieldPanel.UpdateField(STANDARD_STEP_SIZE, DRAW_SCALE_FACTOR);

            string SETS = new string(br.ReadChars(4)); //Magic
            Drill.instance = new Drill();
            Drill.instance.sets = new List<Set>();
            int numSets = br.ReadInt32();
            for (int i = 0; i < numSets; i++)
            {
                Set currSet = new Set();
                currSet.privateID = i;
                currSet.counts = br.ReadInt32();
                Drill.instance.sets.Add(currSet);
            }

            string PFMR = new string(br.ReadChars(4)); //Magic
            Drill.instance.performers = new List<Performer>();
            int numPerformers = br.ReadInt32();
            for (int i = 0; i < numPerformers; i++)
            {
                Performer currPerformer = new Performer(null);
                currPerformer.firstActiveSet = br.ReadInt32();
                currPerformer.lastActiveSet = br.ReadInt32();
                currPerformer.dots = new List<Dot>();

                int lastSet = currPerformer.lastActiveSet == -1 ? Drill.instance.sets.Count : currPerformer.lastActiveSet + 1;

                //for (int j = 0; j < Drill.instance.sets.Count; j++)
                for (int j = 0; j < (lastSet - currPerformer.firstActiveSet); j++)
                {
                    Dot currDot = new Dot();
                    currDot.lefttoright = br.ReadSingle();
                    currDot.fronttoback = br.ReadSingle();
                    currPerformer.dots.Add(currDot);
                }
                currPerformer.dots.InsertRange(0, new Dot[currPerformer.firstActiveSet]);

                Drill.instance.performers.Add(currPerformer);
            }

            br.ReadInt32(); //Magic
            Drill.instance.playbackManager.program = new List<ProgramSettings>();
            int numPrograms = br.ReadInt32();
            for (int i = 0; i < numPrograms; i++)
            {
                ProgramSettings currPrg = new ProgramSettings();
                currPrg.absoluteStartCount = br.ReadInt32();
                currPrg.tempo = br.ReadInt32();
                currPrg.tempoMode = (ProgramSettings.TempoMode)br.ReadInt32();
                Drill.instance.playbackManager.program.Add(currPrg);
            }

            br.ReadInt32(); //Magic
            for (int i = 0; i < Drill.instance.sets.Count; i++)
            {
                Set currSet = Drill.instance.sets[i];
                currSet.set = br.ReadString();
                Drill.instance.sets[i] = currSet;
            }
            for (int i = 0; i < Drill.instance.performers.Count; i++)
            {
                Drill.instance.performers[i].label = br.ReadString();
            }
        }
    }
}
