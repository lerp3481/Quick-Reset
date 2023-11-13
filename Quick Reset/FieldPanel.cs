using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public class FieldPanel : Panel
    {
        public Image fieldBG = null; //layer 0
        public Image currentImage = null; //layer 1
        public Image compositeImage = null;

        public int ImageWidthNoExcess = (int)(Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR);
        public int ImageHeightNoExcess = (int)(Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR);
        public int ImageWidthWithExcess = (int)(Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR) + (int)(Field.X_EXCESS * Field.DRAW_SCALE_FACTOR * 2);
        public int ImageHeightWithExcess = (int)(Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR) + (int)(Field.Y_EXCESS * Field.DRAW_SCALE_FACTOR * 2);

        public int Layer0X = (int)(Field.X_EXCESS * Field.DRAW_SCALE_FACTOR);
        public int Layer0Y = (int)(Field.Y_EXCESS * Field.DRAW_SCALE_FACTOR);

        public float MaximumDrawScaleFactor = 8;

        public bool RESET_IMAGE = false;

        public FieldPanel()
        {
            AutoScrollMinSize = new Size(ImageWidthWithExcess, ImageHeightWithExcess);
            Size = new Size((int)(Field.FIELD_LENGTH * MaximumDrawScaleFactor), (int)(Field.FIELD_WIDTH * MaximumDrawScaleFactor));
            AutoScroll = true;
            DoubleBuffered = true;
        }

        public void Recalculate()
        {
            ImageWidthNoExcess = (int)(Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR);
            ImageHeightNoExcess = (int)(Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR);
            ImageWidthWithExcess = (int)(Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR) + (int)(Field.X_EXCESS * Field.DRAW_SCALE_FACTOR * 2);
            ImageHeightWithExcess = (int)(Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR) + (int)(Field.Y_EXCESS * Field.DRAW_SCALE_FACTOR * 2);

            Layer0X = (int)(Field.X_EXCESS * Field.DRAW_SCALE_FACTOR);
            Layer0Y = (int)(Field.Y_EXCESS * Field.DRAW_SCALE_FACTOR);

            AutoScrollMinSize = new Size(ImageWidthWithExcess, ImageHeightWithExcess);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            /*
            DrawField(e);
            base.OnPaint(e);*/

            if (RESET_IMAGE)
            {
                if (fieldBG != null) fieldBG.Dispose();
                if (currentImage != null)
                {
                    currentImage.Dispose();
                    currentImage = null;
                }
                DrawFieldImage();
                RESET_IMAGE = false;
            }
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            if (RESET_IMAGE) g.Clear(BackColor);
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

            if (compositeImage != null) compositeImage.Dispose();
            compositeImage = new Bitmap(ImageWidthWithExcess, ImageHeightWithExcess);
            using (Graphics gr = Graphics.FromImage(compositeImage))
            {
                gr.Clear(BackColor);
                gr.DrawImage(fieldBG, Layer0X, Layer0Y);
                if (currentImage != null) gr.DrawImage(currentImage, 0, 0);
            }
            g.DrawImage(compositeImage, 0, 0);
        }

        public void ResetCurrentImage()
        {
            if (currentImage != null) currentImage.Dispose();
            currentImage = new Bitmap(ImageWidthWithExcess, ImageHeightWithExcess);
        }

        public void UpdateField(float? stepSize = null, float? scaleFactor = null)
        {
            /*
            //Reposition the UI (part 1)
            foreach (Control ctrl in Form1.instance.Controls)
            {
                if (!ctrl.Equals(this))
                {
                    ctrl.Left -= Form1.instance.uiX;
                    ctrl.Top -= Form1.instance.uiY;
                }
            }

            int ssSteps = (int)Form1.instance.fieldSettingsPanel.stepsizeSteps.Value;
            int ssDistance = (int)Form1.instance.fieldSettingsPanel.stepsizeDistance.Value;

            Field.STANDARD_STEP_SIZE = stepSize.HasValue ? stepSize.Value : (float)((float)ssSteps / (float)ssDistance);
            Field.DRAW_SCALE_FACTOR = scaleFactor.HasValue ? scaleFactor.Value : (float)Form1.instance.fieldSettingsPanel.drawScaleFactor.Value;
            Field.Recalculate();
            Form1.instance.ResetUIPositions();

            //Reposition the UI (part 2)
            foreach (Control ctrl in Form1.instance.Controls)
            {
                if (!ctrl.Equals(this))
                {
                    ctrl.Left += Form1.instance.uiX;
                    ctrl.Top += Form1.instance.uiY;
                }
            }

            DrawField(true);*/

            int ssSteps = (int)Form1.instance.fieldSettingsPanel.stepsizeSteps.Value;
            int ssDistance = (int)Form1.instance.fieldSettingsPanel.stepsizeDistance.Value;

            Field.STANDARD_STEP_SIZE = stepSize.HasValue ? stepSize.Value : (float)((float)ssSteps / (float)ssDistance);
            Field.DRAW_SCALE_FACTOR = scaleFactor.HasValue ? scaleFactor.Value : (float)Form1.instance.fieldSettingsPanel.drawScaleFactor.Value;
            
            Field.Recalculate(); //Recalc field consts
            Recalculate(); //Recalc mostly image consts

            RESET_IMAGE = true;
            Invalidate();
            Update();
        }

        public void DrawDots(int set)
        {
            ResetCurrentImage();
            Graphics g = Graphics.FromImage(currentImage);
            //DrawField();
            Brush brush = new SolidBrush(Color.Red);
            Brush highlighted = new SolidBrush(Color.Blue);

            Font textFont = new Font(FontFamily.GenericSansSerif, 10/*, FontStyle.Bold*/);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            for (int p = 0; p < Drill.instance.performers.Count; p++)
            {
                //For those whose drill begins later...
                if (Drill.instance.performers[p].firstActiveSet > set) continue;
                //For those whose drill ends earlier...
                if (Drill.instance.performers[p].lastActiveSet != -1 && Drill.instance.performers[p].lastActiveSet < set) continue;
                
                Dot d = Drill.instance.performers[p].dots[set];

                float x = (d.lefttoright + (Field.FIELD_LENGTH / 2)) * Field.DRAW_SCALE_FACTOR;
                float y = (-d.fronttoback + (Field.FIELD_WIDTH / 2)) * Field.DRAW_SCALE_FACTOR;

                //To account for field excess
                x += Layer0X;
                y += Layer0Y;

                float width = 1f * Field.DRAW_SCALE_FACTOR;
                float height = 1f * Field.DRAW_SCALE_FACTOR;

                float searchRadius = Field.STANDARD_STEP_SIZE;

                //Drill.instance.performers[p].isHighlighted = surroundingDots.Count != 0;

                g.FillEllipse(Drill.instance.performers[p].isHighlighted ? highlighted : brush, x - width / 2, y - height / 2, width, height);
                //g.FillRectangle(brush, x, y, 0.75f * Field.DRAW_SCALE_FACTOR, 0.75f * Field.DRAW_SCALE_FACTOR);

                if (Drill.instance.showPerformerLabels)
                {
                    //Figure out where to draw the text
                    List<Dot> surroundingDots = Drill.instance.GetSurroundingPerformers(Drill.instance.performers[p], searchRadius, set, 1, 0).Select(perf => perf.dots[set]).ToList();
                    if (surroundingDots.Count == 0)
                    {
                        //Default case: write text below the dot
                        y += searchRadius * 5;
                    }
                    g.DrawString(Drill.instance.performers[p].label, textFont, new SolidBrush(Color.Black), x, y, stringFormat);
                }
            }

            //img.Save("C:/Users/powme/Downloads/" + set + ".png");
            g.Dispose();
            Invalidate();
            Update();
        }

        public void AnimateDots(float progress)
        {
            ResetCurrentImage();
            Graphics g = Graphics.FromImage(currentImage);
            g.Clear(BackColor);
            g.DrawImage(fieldBG, Layer0X, Layer0Y);
            Brush brush = new SolidBrush(Color.Red);
            Brush highlighted = new SolidBrush(Color.Blue);

            Font textFont = new Font(FontFamily.GenericSansSerif, 10/*, FontStyle.Bold*/);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //float progress = (float)Drill.instance.playbackManager.GetTimeElapsed() / (float)Drill.instance.playbackManager.timeDuration;

            for (int p = 0; p < Drill.instance.performers.Count; p++)
            {
                //For those whose drill begins later...
                if (Drill.instance.performers[p].firstActiveSet > Drill.instance.currentSet) continue;
                //For those whose drill ends earlier...
                if (Drill.instance.performers[p].lastActiveSet != -1 && Drill.instance.performers[p].lastActiveSet <= Drill.instance.currentSet) continue;

                Dot current = Drill.instance.performers[p].dots[Drill.instance.currentSet];
                Dot next = Drill.instance.performers[p].dots[Drill.instance.currentSet + 1];

                float startLR = current.lefttoright + ((next.lefttoright - current.lefttoright) * ((float)(Drill.instance.playbackManager.displayCount - 1) / (float)Drill.instance.sets[Drill.instance.currentSet + 1].counts));
                float startFB = current.fronttoback + ((next.fronttoback - current.fronttoback) * ((float)(Drill.instance.playbackManager.displayCount - 1) / (float)Drill.instance.sets[Drill.instance.currentSet + 1].counts));
                float endLR = current.lefttoright + ((next.lefttoright - current.lefttoright) * ((float)Drill.instance.playbackManager.displayCount / (float)Drill.instance.sets[Drill.instance.currentSet + 1].counts));
                float endFB = current.fronttoback + ((next.fronttoback - current.fronttoback) * ((float)Drill.instance.playbackManager.displayCount / (float)Drill.instance.sets[Drill.instance.currentSet + 1].counts));

                Dot d = new Dot();
                d.lefttoright = startLR + (endLR - startLR) * progress;
                d.fronttoback = startFB + (endFB - startFB) * progress;

                float x = (d.lefttoright + (Field.FIELD_LENGTH / 2)) * Field.DRAW_SCALE_FACTOR;
                float y = (-d.fronttoback + (Field.FIELD_WIDTH / 2)) * Field.DRAW_SCALE_FACTOR;

                //To account for field excess
                x += Layer0X;
                y += Layer0Y;

                float width = 1f * Field.DRAW_SCALE_FACTOR;
                float height = 1f * Field.DRAW_SCALE_FACTOR;

                g.FillEllipse(Drill.instance.performers[p].isHighlighted ? highlighted : brush, x - width / 2, y - height / 2, width, height);

                if (Drill.instance.showPerformerLabels) g.DrawString(Drill.instance.performers[p].label, textFont, new SolidBrush(Color.Black), x, y, stringFormat);
            }

            //Graphics lol = CreateGraphics();
            //lol.DrawImage(img, 0, 0);

            g.Dispose();
            //img.Dispose();
            Invalidate();
            Update();
        }

        public void DrawFieldImage()
        {
            fieldBG = new Bitmap(ImageWidthNoExcess, ImageHeightNoExcess);
            Graphics g = Graphics.FromImage(fieldBG);
            g.Clear(BackColor);
            Pen pen = new Pen(Color.FromArgb(192, 192, 192));
            Brush brush = new SolidBrush(Color.White);
            Font textFont = new Font(FontFamily.GenericSansSerif, 2f * Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            //Make canvas
            g.FillRectangle(brush, 0, 0, Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR, Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR);
            brush = new SolidBrush(Color.Black);

            for (int x = 0; x <= Field.FIELD_LENGTH; x++)
            {
                //Vertical gridlines
                pen.Color = Color.FromArgb(192, 192, 192);
                if ((float)x % (float)(Field.STANDARD_STEP_SIZE * 5f) == 0)
                {
                    pen.Color = Color.Black;
                }
                else if ((float)x % (float)(Field.STANDARD_STEP_SIZE * 2.5f) == 0)
                {
                    pen.Color = Color.FromArgb(127, 127, 255);
                }
                g.DrawLine(pen, x * Field.DRAW_SCALE_FACTOR, 0 * Field.DRAW_SCALE_FACTOR, x * Field.DRAW_SCALE_FACTOR, Field.FIELD_WIDTH * Field.DRAW_SCALE_FACTOR);

                //Inserts
                for (var j = x * Field.DRAW_SCALE_FACTOR; j < (x + 1) * Field.DRAW_SCALE_FACTOR; j++)
                {
                    if ((float)j % (float)(Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR) == 0)
                    {
                        pen.Color = Color.Black;
                        pen.Width = 2;

                        float y = 0;
                        g.DrawLine(pen, j, (y - 1) * Field.DRAW_SCALE_FACTOR, j, (y + 1) * Field.DRAW_SCALE_FACTOR);
                        y = Field.FIELD_WIDTH;
                        g.DrawLine(pen, j, (y - 1) * Field.DRAW_SCALE_FACTOR, j, (y + 1) * Field.DRAW_SCALE_FACTOR);
                    }
                }
            }
            for (int y = 0; y <= Field.FIELD_WIDTH; y++)
            {
                //Horizontal gridlines
                pen.Color = Color.FromArgb(192, 192, 192);
                pen.Width = 1;
                if ((float)y % (float)(Field.STANDARD_STEP_SIZE * 2.5f) == 0)
                {
                    pen.Color = Color.FromArgb(127, 127, 255);
                }
                g.DrawLine(pen, 0 * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR, Field.FIELD_LENGTH * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR);
            }
            for (int _x = 0; _x < 21; _x++)
            {
                //Hashes
                float x = _x * 5 * Field.STANDARD_STEP_SIZE;

                pen.Color = Color.Black;
                pen.Width = 4;

                float y = 20 * Field.STANDARD_STEP_SIZE;
                g.DrawLine(pen, (x - 1) * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR, (x + 1) * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR);
                y = 32.5f * Field.STANDARD_STEP_SIZE;
                g.DrawLine(pen, (x - 1) * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR, (x + 1) * Field.DRAW_SCALE_FACTOR, y * Field.DRAW_SCALE_FACTOR);

                //Yard numbers
                int yardNumber = (-Math.Abs(_x - 10) + 10) * 5;
                /*
                SizeF sf = g.MeasureString(yardNumber.ToString(), textFont, Height, stringFormat);
                Bitmap bmp = new Bitmap((int)sf.Width, (int)sf.Height);
                using (Graphics bmpG = Graphics.FromImage(bmp))
                {
                    bmpG.DrawString(yardNumber.ToString(), textFont, brush, new PointF(0, 0), stringFormat);
                }
                bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
                g.DrawImage(bmp, x * Field.DRAW_SCALE_FACTOR, 8 * Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR);
                */

                g.TranslateTransform(x * Field.DRAW_SCALE_FACTOR, 8f * Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR);
                g.RotateTransform(180f);
                g.DrawString(yardNumber.ToString(), textFont, brush, new PointF(0, 0), stringFormat);
                g.RotateTransform(0f);
                g.ResetTransform();

                //g.DrawString(yardNumber.ToString(), textFont, brush, new PointF(x * Field.DRAW_SCALE_FACTOR, 8f * Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR), stringFormat);
                g.DrawString(yardNumber.ToString(), textFont, brush, new PointF(x * Field.DRAW_SCALE_FACTOR, (52.5f - 8f) * Field.STANDARD_STEP_SIZE * Field.DRAW_SCALE_FACTOR), stringFormat);
            }
        }
    }
}
