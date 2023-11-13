using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Quick_Reset
{
    class PlaybackManager
    {
        public List<ProgramSettings> program;

        public Timer timer;

        public int displayCount; //Counts since the previous set
        public int absoluteCount; //Counts since the beginning of the show
        public int programStep; //What "ProgramSetting" we're currently on
        public float tempo; //The current tempo
        public long timeStart; //Timer
        public long timeDuration; //How many milliseconds a count is

        /*
        public string videoFileName = null;
        public VideoFileWriter vfWriter;*/
        public VideoExporter videoExporter = null;
        public long frameTimer = -1;

        public PlaybackManager()
        {
            program = new List<ProgramSettings>();
        }

        public void InitVideo(string filename, int frameRate, string audio = null)
        {
            videoExporter = new VideoExporter(filename, frameRate, audio);
        }

        public void InitForPlayback()
        {
            timer = new Timer();
            timer.Tick += Update;
            timer.Interval = 1;
            timer.Start();

            absoluteCount = CalculateAbsoluteCount(Drill.instance.sets[Drill.instance.currentSet].counts);

            programStep = 0;
            while (true)
            {
                try
                {
                    if (program[programStep + 1].absoluteStartCount < absoluteCount)
                    {
                        //We've already passed this step! Onto the next!
                        programStep++;
                    }
                    else
                    {
                        break;
                    }
                }
                catch
                {
                    break;
                }
            }

            displayCount = 1;
            HandleProgram();

            ResetTimer();
        }

        public void StopPlayback()
        {
            timer.Stop();
            if (videoExporter != null)
            {
                videoExporter.MakeVideo();
            }
        }

        public void ResetTimer()
        {
            timeStart = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            timeDuration = (long)(60000 / tempo);
        }

        public long GetTimeElapsed()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - timeStart;
        }

        public void HandleProgram()
        {
            tempo = program[programStep].tempo;

            //I don't know how to handle dynamic tempo changes legitmately, so here's a workaround
            if (program[programStep].tempoMode == ProgramSettings.TempoMode.DYNAMIC_START)
            {
                //Assuming that the next program has the DYNAMIC_END tempo mode
                tempo = (tempo + (float)program[programStep + 1].tempo) / 2.0f;
            }

            programStep++;
        }

        public void OnTick()
        {
            //When a new count comes
            displayCount++;
            absoluteCount++;

            if (displayCount > Drill.instance.sets[Drill.instance.currentSet + 1].counts)
            {
                //New set!
                Drill.instance.currentSet++;
                //TODO: Check whether or not to stop
                displayCount = 1;
            }

            try
            {
                if (program[programStep].absoluteStartCount == absoluteCount)
                {
                    HandleProgram();
                }
            }
            catch
            {
                //This just means that we've reached the last program
            }

            ResetTimer();
        }

        public void Update(object e, EventArgs h)
        {
            //--VIDEO STUFF
            if (videoExporter != null)
            {
                if (frameTimer == -1)
                {
                    //set frame timer
                    frameTimer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                
                if (1000f / (float)videoExporter.framerate <= DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameTimer)
                {
                    //time to dump a new frame!
                    videoExporter.DumpFrame();
                    frameTimer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }
                /*
                float msPerFrame = (float)timeDuration / ((float)tempo * (float)videoExporter.framerate / 60f);
                if (msPerFrame <= DateTimeOffset.Now.ToUnixTimeMilliseconds() - frameTimer)
                {
                    //time to dump a new frame!
                    videoExporter.DumpFrame();
                    frameTimer = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                }*/
            }
            //--
            if (GetTimeElapsed() >= timeDuration)
            {
                OnTick();
            }


            if (Drill.instance.GetNextSetName() == "NULL")
            {
                //Um let's stop
                StopPlayback();
                return;
            }

            Form1.instance.fieldPanel.AnimateDots((float)Drill.instance.playbackManager.GetTimeElapsed() / (float)Drill.instance.playbackManager.timeDuration);
            Form1.instance.setPanel.setNumber.Text = "From " + Drill.instance.GetCurrentSetName() + " to " + Drill.instance.GetNextSetName() + ": " + displayCount + " / " + Drill.instance.sets[Drill.instance.currentSet + 1].counts;
        }

        public int CalculateAbsoluteCount(int set)
        {
            if (set >= Drill.instance.sets.Count) return -1;
            int i = 0, ret = 0;
            while (i != set)
            {
                i++;
                ret += Drill.instance.sets[i].counts;
            }
            return ret + 1;
        }

        public float CalculateMilliseconds(int absCount) //absolute count
        {
            /*
            float ret = 0;

            //int absCount = CalculateAbsoluteCount(set) + count;
            int prg = 0;
            float thisTempo = program[0].tempo;
            //I don't know how to handle dynamic tempo changes legitmately, so here's a workaround
            if (program[0].tempoMode == ProgramSettings.TempoMode.DYNAMIC_START)
            {
                //Assuming that the next program has the DYNAMIC_END tempo mode
                thisTempo = (thisTempo + (float)program[1].tempo) / 2.0f;
            }
            if (program.Count > 1)
            {
                try
                {
                    int prevStartCount = 0;
                    while (program[prg + 1].absoluteStartCount < absCount)
                    {
                        ProgramSettings setting = program[prg];

                        thisTempo = setting.tempo;
                        //I don't know how to handle dynamic tempo changes legitmately, so here's a workaround
                        if (setting.tempoMode == ProgramSettings.TempoMode.DYNAMIC_START)
                        {
                            //Assuming that the next program has the DYNAMIC_END tempo mode
                            thisTempo = (thisTempo + (float)program[prg + 1].tempo) / 2.0f;
                        }

                        ret += (float)(setting.absoluteStartCount - prevStartCount) * 60000f / thisTempo;
                        prevStartCount = setting.absoluteStartCount;
                        prg++;
                    }
                }
                catch
                {
                    //We've reached the last program; no worries
                }
            }
            //Now, prg is the current program we're in. Now, where in the prg are we?
            while (prg >= 0)
            {
                absCount -= program[prg].absoluteStartCount;
                prg--;
            }

            ret += (float)absCount * 60000f / thisTempo;

            return ret;*/
            float ret = 0;
            int i = 0;
            for (i = 0; i < program.Count - 1; i++)
            {
                if (program[i].tempoMode == ProgramSettings.TempoMode.DYNAMIC_START)
                {
                    throw new NotImplementedException();
                }
                //In other words, if this isn't the last program
                if (program[i + 1].absoluteStartCount <= absCount)
                {
                    //Oh, we're in the next program
                    ret += (float)(program[i + 1].absoluteStartCount - program[i].absoluteStartCount) * 60000f / program[i].tempo;
                }
                else
                {
                    break;
                }
            }

            absCount -= program[i].absoluteStartCount;
            ret += (float)absCount * 60000f / program[i].tempo;

            return ret;
        }
    }
}
