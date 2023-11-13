using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public class SetPanel : Panel
    {
        public Button previousSet;
        public Button nextSet;
        public Button playbackDrill;
        public Label setNumber;
        public Button saveImage;
        public Button makeVideo;

        public SetPanel()
        {
            AutoSize = true;

            setNumber = new Label();
            previousSet = new Button();
            nextSet = new Button();
            playbackDrill = new Button();
            saveImage = new Button();
            makeVideo = new Button();

            Controls.AddRange(new Control[] { setNumber, previousSet, nextSet, playbackDrill, saveImage, makeVideo });

            int uiX = 0, uiY = 0;

            setNumber.Text = "Default";
            setNumber.Location = new Point(uiX, uiY);
            setNumber.Width = 500;

            uiY += setNumber.Height;

            previousSet.Text = "Previous set";
            previousSet.Location = new Point(uiX, uiY);

            uiX += previousSet.Width;

            nextSet.Text = "Next set";
            nextSet.Location = new Point(uiX, uiY);

            uiX += nextSet.Width;

            playbackDrill.Text = "Play";
            playbackDrill.Location = new Point(uiX, uiY);

            uiX = 0;
            uiY += setNumber.Height;

            saveImage.Text = "Save image";
            saveImage.Location = new Point(uiX, uiY);

            uiX += saveImage.Width;

            makeVideo.Text = "Make video";
            makeVideo.Location = new Point(uiX, uiY);

            previousSet.Click += PreviousSet_Click;
            nextSet.Click += NextSet_Click;
            playbackDrill.Click += PlaybackDrill_Click;
            saveImage.Click += SaveImage_Click;
            makeVideo.Click += MakeVideo_Click;
        }

        private void PreviousSet_Click(object sender, EventArgs e)
        {
            Form1.instance.fieldPanel.DrawDots(Drill.instance.PreviousSet());
            setNumber.Text = "From " + Drill.instance.GetCurrentSetName() + " to " + Drill.instance.GetNextSetName();
        }
        private void NextSet_Click(object sender, EventArgs e)
        {
            Form1.instance.fieldPanel.DrawDots(Drill.instance.NextSet());
            setNumber.Text = "From " + Drill.instance.GetCurrentSetName() + " to " + Drill.instance.GetNextSetName();
        }
        private void PlaybackDrill_Click(object sender, EventArgs e)
        {
            //Drill.instance.playbackManager.InitVideo("C:/Users/powme/Downloads/ita.mp4", 30, "C:/Users/powme/Downloads/chhs_drill/2022intothinair/ita.mp3");
            Drill.instance.playbackManager.InitForPlayback();
        }
        private void SaveImage_Click(object sender, EventArgs e)
        {
            Form1.instance.fieldPanel.compositeImage.Save("C:/Users/powme/Downloads/" + Drill.instance.GetCurrentSetName() + ".png");
        }
        private void MakeVideo_Click(object sender, EventArgs e)
        {
            VideoExporter temp = new VideoExporter("C:/Users/powme/Downloads/pop.mp4", 30, "C:/Users/powme/Downloads/chhs_drill/2023pop/oldpop3.mp3");
            //VideoExporter temp = new VideoExporter("C:/Users/powme/Downloads/ita.mp4", 30);
        }
    }
}
