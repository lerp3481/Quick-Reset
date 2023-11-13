using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class VideoExporter
    {
        public string filename;
        public int framerate;
        public string audiofile;

        public string frameFolder;
        public int numFrames = 0;

        public int setHeight = 100; //height of set name and stuff in pixels
        public Font setFont = new Font(FontFamily.GenericSansSerif, 48);

        public int minimumCount = 2;

        public VideoExporter(string f, int frate, string audio = null)
        {
            filename = f;
            framerate = frate;
            audiofile = audio;

            frameFolder = Path.GetFileNameWithoutExtension(f) + "/";

            Directory.CreateDirectory(frameFolder);

            SimulateTime();
            MakeVideo();
        }

        public void SimulateTime()
        {
            while (Drill.instance.GetNextSetName() != "NULL")
            {
                /*
                //Step 0. Check if it's possible to continue
                if (Drill.instance.GetNextSetName() == "NULL")
                {
                    //Let's stop
                    return;
                }*/

                //Step 1. Convert frames to time
                float millisecondsElapsed = 1000f * (float)numFrames / (float)framerate;

                //Step 2. Figure out which count this is
                while (Drill.instance.playbackManager.CalculateMilliseconds(minimumCount) < millisecondsElapsed)
                {
                    minimumCount++;
                }

                //Step 3. Figure out when in the count this is
                float prevCountMs = Drill.instance.playbackManager.CalculateMilliseconds(minimumCount - 1);
                float nextCountMs = Drill.instance.playbackManager.CalculateMilliseconds(minimumCount);

                float prg = (millisecondsElapsed - prevCountMs) / (nextCountMs - prevCountMs);

                //Step 4. Figure out which set this is
                Drill.instance.currentSet = 0;
                int scount = Drill.instance.playbackManager.CalculateAbsoluteCount(Drill.instance.currentSet + 1);
                while (scount < minimumCount)
                {
                    if (scount == -1)
                    {
                        //Let's stop
                        return;
                    }
                    Drill.instance.currentSet++;
                    scount = Drill.instance.playbackManager.CalculateAbsoluteCount(Drill.instance.currentSet + 1);
                }

                //Step 5. Figure out which count of the set this is
                int countInSet = minimumCount - Drill.instance.playbackManager.CalculateAbsoluteCount(Drill.instance.currentSet);

                //Step 6. Simulate time!
                Drill.instance.playbackManager.displayCount = countInSet;
                Form1.instance.fieldPanel.AnimateDots(prg);
                Form1.instance.setPanel.setNumber.Text = "From " + Drill.instance.GetCurrentSetName() + " to " + Drill.instance.GetNextSetName() + ": " + countInSet + " / " + Drill.instance.sets[Drill.instance.currentSet + 1].counts;
                Debug.WriteLine(Form1.instance.setPanel.setNumber.Text + " [" + minimumCount + " " + prevCountMs + "-" + nextCountMs + "]");
                DumpFrame();
            }
            //SimulateTime();
        }

        public void DumpFrame()
        {
            using (Image frame = new Bitmap(Form1.instance.fieldPanel.ImageWidthWithExcess, Form1.instance.fieldPanel.ImageHeightWithExcess + setHeight))
            {
                using (Graphics g = Graphics.FromImage(frame))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.Clear(Color.Black);
                    g.DrawImage(Form1.instance.fieldPanel.compositeImage, 0, 0);
                    g.DrawString(Form1.instance.setPanel.setNumber.Text, setFont, new SolidBrush(Color.White), 0, Form1.instance.fieldPanel.ImageHeightWithExcess);

                    frame.Save(frameFolder + numFrames.ToString("D5") + ".png");
                    numFrames++;
                }
            }
        }

        public void MakeVideo()
        {
            ProcessStartInfo p = new ProcessStartInfo();

            p.FileName = "ffmpeg.exe";

            p.Arguments = "-r " + framerate + " -i " + frameFolder + "%05d.png ";
            if (audiofile != null)
            {
                p.Arguments += "-i " + audiofile + " ";
            }
            p.Arguments += "-pix_fmt yuv420p " + filename;

            using (Process proc = Process.Start(p))
            {
                proc.WaitForExit();
            }

            Directory.Delete(frameFolder, true);
        }
    }
}
