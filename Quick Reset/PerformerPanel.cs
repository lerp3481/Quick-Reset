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
    public class PerformerPanel : Panel
    {
        public Label performerText;
        public TextBox performerInput;
        public Button identifyPerformer;
        public Button showPerformerLabels;
        public Button makeDotBook;

        public PerformerPanel()
        {
            AutoSize = true;

            performerText = new Label();
            performerInput = new TextBox();
            identifyPerformer = new Button();
            showPerformerLabels = new Button();
            makeDotBook = new Button();

            Controls.AddRange(new Control[] { performerText, performerInput, identifyPerformer, showPerformerLabels, makeDotBook });

            int uiX = 0, uiY = 0;

            showPerformerLabels.Text = "Toggle performer labels";
            showPerformerLabels.Width = 140;
            showPerformerLabels.Location = new Point(uiX, uiY);

            uiY += showPerformerLabels.Height;

            performerText.Text = "Performer: ";
            performerText.Location = new Point(uiX, uiY);

            uiX += performerText.Width;

            performerInput.Location = new Point(uiX, uiY);

            uiX = 0;
            uiY += performerText.Height;

            identifyPerformer.Text = "(Un)highlight performer";
            identifyPerformer.Width = 140;
            identifyPerformer.Location = new Point(uiX, uiY);

            uiX += identifyPerformer.Width;

            makeDotBook.Text = "Make dot book";
            makeDotBook.Width = 100;
            makeDotBook.Location = new Point(uiX, uiY);

            showPerformerLabels.Click += ShowPerformerLabels_Click;
            identifyPerformer.Click += IdentifyPerformer_Click;
            makeDotBook.Click += MakeDotBook_Click;
        }

        private void ShowPerformerLabels_Click(object sender, EventArgs e)
        {
            Drill.instance.showPerformerLabels = !Drill.instance.showPerformerLabels;
        }

        private void IdentifyPerformer_Click(object sender, EventArgs e)
        {
            Drill.instance.GetPerformerByLabel(performerInput.Text).ToggleHighlight();
        }

        private void MakeDotBook_Click(object sender, EventArgs e)
        {
            Performer p = Drill.instance.GetPerformerByLabel(performerInput.Text);
            /*
            Font textFont = new Font(FontFamily.GenericSansSerif, 48);
            Font textFont2 = new Font(FontFamily.GenericSansSerif, 16);
            Font textFont3 = new Font(FontFamily.GenericSansSerif, 30);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            StringFormat stringFormat2 = new StringFormat();
            stringFormat2.Alignment = StringAlignment.Near;
            stringFormat2.LineAlignment = StringAlignment.Center;
            StringFormat stringFormat3 = new StringFormat();
            stringFormat3.Alignment = StringAlignment.Near;
            stringFormat3.LineAlignment = StringAlignment.Near;

            for (int i = 0; i < Drill.instance.sets.Count; i++)
            {
                Set s = Drill.instance.sets[i];
                Dot dot = p.dots[i];
                Dot previousDot = i != 0 ? p.dots[i - 1] : new Dot();
                bool sameFlag = false;

                Image img = new Bitmap(600, 500);
                Graphics g = Graphics.FromImage(img);

                //Bg
                g.Clear(Color.FromArgb(100, 100, 100));

                //Chart # layout
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(20, 20, 150, 50));
                g.DrawString(s.set, textFont, new SolidBrush(Color.Black), new PointF(95, 45), stringFormat);

                //Counts layout
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(20, 80, 150, 50));
                g.DrawString(s.counts.ToString(), textFont, new SolidBrush(Color.Black), new PointF(95, 105), stringFormat);

                //Coordinate layout
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(180, 20, 400, 110));
                if (i != 0)
                {
                    if (previousDot.lefttoright == dot.lefttoright && previousDot.fronttoback == dot.fronttoback)
                    {
                        sameFlag = true;
                        g.DrawString("<SAME>", textFont2, new SolidBrush(Color.Black), new PointF(200, 45), stringFormat2);
                        g.DrawString("<SAME>", textFont2, new SolidBrush(Color.Black), new PointF(200, 105), stringFormat2);
                    }
                    else
                    {
                        g.DrawString(dot.LeftToRightToString(), textFont2, new SolidBrush(Color.Black), new PointF(200, 45), stringFormat2);
                        g.DrawString(dot.FrontToBackToString(), textFont2, new SolidBrush(Color.Black), new PointF(200, 105), stringFormat2);
                    }
                }
                else
                {
                    g.DrawString(dot.LeftToRightToString(), textFont2, new SolidBrush(Color.Black), new PointF(200, 45), stringFormat2);
                    g.DrawString(dot.FrontToBackToString(), textFont2, new SolidBrush(Color.Black), new PointF(200, 105), stringFormat2);
                }

                //Midset layout
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(310, 140, 270, 150));
                Dot midpoint = new Dot();
                if (sameFlag || i == 0)
                {
                    g.DrawString("----", textFont2, new SolidBrush(Color.Black), new PointF(330, 155), stringFormat2);
                    g.DrawString("----", textFont2, new SolidBrush(Color.Black), new PointF(330, 265), stringFormat2);
                }
                else
                {
                    midpoint.lefttoright = (previousDot.lefttoright + dot.lefttoright) / 2;
                    midpoint.fronttoback = (previousDot.fronttoback + dot.fronttoback) / 2;
                    g.DrawString(midpoint.LeftToRightToString(), textFont2, new SolidBrush(Color.Black), new PointF(330, 155), stringFormat2);
                    g.DrawString(midpoint.FrontToBackToString(), textFont2, new SolidBrush(Color.Black), new PointF(330, 265), stringFormat2);
                }

                //Graph layout
                float GRAPH_SCALE_FACTOR = 5.5f;
                int GRAPH_FIELD_LENGTH = (int)(25 * Field.STANDARD_STEP_SIZE);
                int GRAPH_FIELD_WIDTH = (int)(20 * Field.STANDARD_STEP_SIZE);
                g.FillRectangle(new SolidBrush(Color.White), new Rectangle(20, 140, (int)(GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR), (int)(GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR)));
                Image miniFieldImage = new Bitmap((int)(GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR), (int)(GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR));
                Graphics dotField = Graphics.FromImage(miniFieldImage);
                Pen linePen = new Pen(Color.Black);

                for (int x = 0; x <= GRAPH_FIELD_LENGTH; x++)
                {
                    //Vertical gridlines
                    linePen.Color = Color.FromArgb(192, 192, 192);
                    if ((float)(x + Field.STANDARD_STEP_SIZE * 2.5f) % (float)(Field.STANDARD_STEP_SIZE * 5f) == 0)
                    {
                        linePen.Color = Color.Black;
                    }
                    else if ((float)x % (float)(Field.STANDARD_STEP_SIZE * 2.5f) == 0)
                    {
                        linePen.Color = Color.FromArgb(127, 127, 255);
                    }
                    dotField.DrawLine(linePen, x * GRAPH_SCALE_FACTOR, 0, x * GRAPH_SCALE_FACTOR, GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR);
                }
                for (int y = 0; y <= GRAPH_FIELD_WIDTH; y++)
                {
                    //Horizontal gridlines
                    linePen.Color = Color.FromArgb(192, 192, 192);
                    if ((float)y % (float)(Field.STANDARD_STEP_SIZE * 2.5f) == 0)
                    {
                        linePen.Color = Color.FromArgb(127, 127, 255);
                    }
                    dotField.DrawLine(linePen, 0, y * GRAPH_SCALE_FACTOR, GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR);
                }

                //--Get dot to be somewhere in the middle of the graph
                float fieldX = (dot.lefttoright + (Field.STANDARD_STEP_SIZE * 2.5f) + (Field.FIELD_LENGTH / 2)) % GRAPH_FIELD_LENGTH;
                float fieldY = (dot.fronttoback + (Field.FIELD_WIDTH / 2)) % GRAPH_FIELD_WIDTH;
                float translationX = 0, translationY = 0;

                while (fieldX - (12.5 * Field.STANDARD_STEP_SIZE) > 2.5 * Field.STANDARD_STEP_SIZE)
                {
                    fieldX -= 5 * Field.STANDARD_STEP_SIZE;
                    translationX -= 5 * Field.STANDARD_STEP_SIZE;
                }
                while (fieldX - (12.5 * Field.STANDARD_STEP_SIZE) < -2.5 * Field.STANDARD_STEP_SIZE)
                {
                    fieldX += 5 * Field.STANDARD_STEP_SIZE;
                    translationX += 5 * Field.STANDARD_STEP_SIZE;
                }
                while (fieldY - (10 * Field.STANDARD_STEP_SIZE) > 1.25 * Field.STANDARD_STEP_SIZE)
                {
                    fieldY -= 2.5f * Field.STANDARD_STEP_SIZE;
                    translationY -= 2.5f * Field.STANDARD_STEP_SIZE;
                }
                while (fieldY - (10 * Field.STANDARD_STEP_SIZE) < -1.25 * Field.STANDARD_STEP_SIZE)
                {
                    fieldY += 2.5f * Field.STANDARD_STEP_SIZE;
                    translationY += 2.5f * Field.STANDARD_STEP_SIZE;
                }

                fieldY = GRAPH_FIELD_WIDTH - fieldY;

                //--Figure out where to put the previous dot
                float previousFieldX = 0, previousFieldY = 0;
                bool drawPreviousDot = false;

                if (i != 0 && !sameFlag)
                {
                    float deltaX = dot.lefttoright - previousDot.lefttoright;
                    float deltaY = dot.fronttoback - previousDot.fronttoback;

                    if (deltaX <= GRAPH_FIELD_LENGTH && deltaY <= GRAPH_FIELD_WIDTH)
                    {
                        drawPreviousDot = true;
                        previousFieldX = fieldX - deltaX;
                        previousFieldY = fieldY + deltaY;

                        while (previousFieldX > GRAPH_FIELD_LENGTH)
                        {
                            fieldX -= 5 * Field.STANDARD_STEP_SIZE;
                            previousFieldX -= 5 * Field.STANDARD_STEP_SIZE;
                        }
                        while (previousFieldX < 0)
                        {
                            fieldX += 5 * Field.STANDARD_STEP_SIZE;
                            previousFieldX += 5 * Field.STANDARD_STEP_SIZE;
                        }
                        while (previousFieldY > GRAPH_FIELD_WIDTH)
                        {
                            fieldY -= 2.5f * Field.STANDARD_STEP_SIZE;
                            previousFieldY -= 2.5f * Field.STANDARD_STEP_SIZE;
                        }
                        while (previousFieldY < 0)
                        {
                            fieldY += 2.5f * Field.STANDARD_STEP_SIZE;
                            previousFieldY += 2.5f * Field.STANDARD_STEP_SIZE;
                        }
                    }
                }

                //--Now bound it by the actual field
                float fromSide1 = dot.lefttoright + (Field.FIELD_LENGTH / 2);
                float fromSide2 = Field.FIELD_LENGTH - fromSide1;
                float fromFrontSideline = dot.fronttoback + (Field.FIELD_WIDTH / 2);
                float fromBackSideline = Field.FIELD_WIDTH - fromFrontSideline;

                float fromLeftBound = fieldX;
                float fromRightBound = GRAPH_FIELD_LENGTH - fromLeftBound;
                float fromTopBound = fieldY;
                float fromBottomBound = GRAPH_FIELD_WIDTH - fromTopBound;

                while (fromSide1 < fromLeftBound)
                {
                    fieldX -= 5 * Field.STANDARD_STEP_SIZE;
                    previousFieldX -= 5 * Field.STANDARD_STEP_SIZE;
                    fromLeftBound -= 5 * Field.STANDARD_STEP_SIZE;
                }
                while (fromSide2 < fromRightBound)
                {
                    fieldX += 5 * Field.STANDARD_STEP_SIZE;
                    previousFieldX += 5 * Field.STANDARD_STEP_SIZE;
                    fromRightBound -= 5 * Field.STANDARD_STEP_SIZE;
                }
                while (fromFrontSideline < fromBottomBound)
                {
                    fieldY += 2.5f * Field.STANDARD_STEP_SIZE;
                    previousFieldY += 2.5f * Field.STANDARD_STEP_SIZE;
                    fromBottomBound -= 2.5f * Field.STANDARD_STEP_SIZE;
                }
                while (fromFrontSideline < fromBottomBound)
                {
                    fieldY -= 2.5f * Field.STANDARD_STEP_SIZE;
                    previousFieldY -= 2.5f * Field.STANDARD_STEP_SIZE;
                    fromTopBound -= 2.5f * Field.STANDARD_STEP_SIZE;
                }

                if (fromSide1 < 2.5f * Field.STANDARD_STEP_SIZE)
                {
                    fieldX += 5 * Field.STANDARD_STEP_SIZE;
                    previousFieldX += 5 * Field.STANDARD_STEP_SIZE;
                }
                if (fromSide2 < 2.5f * Field.STANDARD_STEP_SIZE)
                {
                    fieldX -= 5 * Field.STANDARD_STEP_SIZE;
                    previousFieldX -= 5 * Field.STANDARD_STEP_SIZE;
                }

                //--Draw hashes, numbers, and sidelines
                FieldReference nearestRef = dot.GetNearestFrontToBackReference();
                float deviation = -dot.GetDeviationFromFrontToBackReference();

                float backSidelineY = 0, backNumberY = 0, backHashY = 0, frontHashY = 0, frontNumberY = 0, frontSidelineY = 0;

                switch (nearestRef)
                {
                    case FieldReference.BACK_SIDELINE:
                        backSidelineY = fieldY + deviation;
                        backHashY = backSidelineY + Field.SIDELINE_TO_HASH_DIST;
                        frontHashY = backHashY + Field.HASH_DIST;
                        frontSidelineY = frontHashY + Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.BOTTOM_BACK_NUMBERS:
                        backNumberY = fieldY + deviation + Field.STANDARD_STEP_SIZE;
                        backSidelineY = backNumberY - (8f * Field.STANDARD_STEP_SIZE);
                        backHashY = backSidelineY + Field.SIDELINE_TO_HASH_DIST;
                        frontHashY = backHashY + Field.HASH_DIST;
                        frontSidelineY = frontHashY + Field.SIDELINE_TO_HASH_DIST;
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.TOP_BACK_NUMBERS:
                        backNumberY = fieldY + deviation - Field.STANDARD_STEP_SIZE;
                        backSidelineY = backNumberY - (8f * Field.STANDARD_STEP_SIZE);
                        backHashY = backSidelineY + Field.SIDELINE_TO_HASH_DIST;
                        frontHashY = backHashY + Field.HASH_DIST;
                        frontSidelineY = frontHashY + Field.SIDELINE_TO_HASH_DIST;
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.BACK_HASH:
                        backHashY = fieldY + deviation;
                        backSidelineY = backHashY - Field.SIDELINE_TO_HASH_DIST;
                        frontHashY = backHashY + Field.HASH_DIST;
                        frontSidelineY = frontHashY + Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.FRONT_HASH:
                        frontHashY = fieldY + deviation;
                        backHashY = frontHashY - Field.HASH_DIST;
                        backSidelineY = backHashY - Field.SIDELINE_TO_HASH_DIST;
                        frontSidelineY = frontHashY + Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.TOP_FRONT_NUMBERS:
                        frontNumberY = fieldY + deviation + Field.STANDARD_STEP_SIZE;
                        frontSidelineY = frontNumberY + (8f * Field.STANDARD_STEP_SIZE);
                        frontHashY = frontSidelineY - Field.SIDELINE_TO_HASH_DIST;
                        backHashY = frontHashY - Field.HASH_DIST;
                        backSidelineY = backHashY - Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.BOTTOM_FRONT_NUMBERS:
                        frontNumberY = fieldY + deviation - Field.STANDARD_STEP_SIZE;
                        frontSidelineY = frontNumberY + (8f * Field.STANDARD_STEP_SIZE);
                        frontHashY = frontSidelineY - Field.SIDELINE_TO_HASH_DIST;
                        backHashY = frontHashY - Field.HASH_DIST;
                        backSidelineY = backHashY - Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        break;
                    case FieldReference.FRONT_SIDELINE:
                        frontSidelineY = fieldY + deviation;
                        frontHashY = frontSidelineY - Field.SIDELINE_TO_HASH_DIST;
                        backHashY = frontHashY - Field.HASH_DIST;
                        backSidelineY = backHashY - Field.SIDELINE_TO_HASH_DIST;
                        backNumberY = backSidelineY + (8f * Field.STANDARD_STEP_SIZE);
                        frontNumberY = frontSidelineY - (8f * Field.STANDARD_STEP_SIZE);
                        break;
                }

                int[] yds = new int[] { 0, 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5, 0 };
                int[] ydLabels = new int[] { -1, -1, -1, -1, -1 };
                int occr = 0, ydIdx = 0;

                for (int j = 0; j < yds.Length; j++)
                {
                    if (yds[j] == dot.GetNearestYardLine())
                    {
                        occr++;
                        if (yds[j] == 50)
                        {
                            ydIdx = j;
                            break;
                        }
                        if (dot.IsSide1())
                        {
                            ydIdx = j;
                            break;
                        }
                        else if (occr == 2)
                        {
                            ydIdx = j;
                            break;
                        }
                    }
                }

                ydLabels[0] = ydIdx >= 2 ? yds[ydIdx - 2] : -1;
                ydLabels[1] = ydIdx >= 1 ? yds[ydIdx - 1] : -1;
                ydLabels[2] = yds[ydIdx - 0];
                ydLabels[3] = yds[ydIdx + 1];
                ydLabels[4] = yds[ydIdx + 2];

                Font nTextFont = new Font(FontFamily.GenericSansSerif, 2f * Field.STANDARD_STEP_SIZE * GRAPH_SCALE_FACTOR);
                StringFormat nStringFormat = new StringFormat();
                nStringFormat.Alignment = StringAlignment.Center;
                nStringFormat.LineAlignment = StringAlignment.Center;

                linePen.Color = Color.Black;
                linePen.Width = 4;
                for (int _x = 0; _x <= GRAPH_FIELD_LENGTH * Field.STANDARD_STEP_SIZE; _x++)
                {
                    float x = (float)_x / (float)Field.STANDARD_STEP_SIZE;
                    for (int _y = 0; _y <= GRAPH_FIELD_WIDTH * Field.STANDARD_STEP_SIZE; _y++)
                    {
                        float y = (float)_y / (float)Field.STANDARD_STEP_SIZE;
                        if ((x + (2.5f * Field.STANDARD_STEP_SIZE)) % (5 * Field.STANDARD_STEP_SIZE) == 0)
                        {
                            if (y == frontHashY || y == backHashY)
                            {
                                dotField.DrawLine(linePen, (x - 1) * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR, (x + 1) * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR);
                            }
                            if (y == frontNumberY)
                            {
                                int whichYardIdx = ((int)(x + (2.5f * Field.STANDARD_STEP_SIZE)) / (int)(5 * Field.STANDARD_STEP_SIZE)) - 1;
                                dotField.DrawString(ydLabels[whichYardIdx].ToString(), nTextFont, new SolidBrush(Color.Black), new PointF(x * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR), nStringFormat);
                            }
                            if (y == backNumberY)
                            {
                                int whichYardIdx = ((int)(x + (2.5f * Field.STANDARD_STEP_SIZE)) / (int)(5 * Field.STANDARD_STEP_SIZE)) - 1;
                                dotField.TranslateTransform(x * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR);
                                dotField.RotateTransform(180f);
                                dotField.DrawString(ydLabels[whichYardIdx].ToString(), nTextFont, new SolidBrush(Color.Black), new PointF(0, 0), nStringFormat);
                                dotField.RotateTransform(0f);
                                dotField.ResetTransform();
                            }
                        }
                        if (y == frontSidelineY || y == backSidelineY)
                        {
                            dotField.DrawLine(linePen, 0, y * GRAPH_SCALE_FACTOR, GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR);
                        }

                        //--TODO: Draw endzones
                    }
                }

                //--Draw dot(s)
                if (drawPreviousDot)
                {
                    Pen pen = new Pen(Color.Blue, 0.5f * GRAPH_SCALE_FACTOR);
                    pen.StartCap = LineCap.RoundAnchor;
                    pen.EndCap = LineCap.ArrowAnchor;

                    dotField.DrawLine(pen, previousFieldX * GRAPH_SCALE_FACTOR, previousFieldY * GRAPH_SCALE_FACTOR, fieldX * GRAPH_SCALE_FACTOR, fieldY * GRAPH_SCALE_FACTOR);
                }
                float width = 1f * GRAPH_SCALE_FACTOR;
                float height = 1f * GRAPH_SCALE_FACTOR;

                dotField.FillEllipse(new SolidBrush(Color.Red), fieldX * GRAPH_SCALE_FACTOR - width / 2, fieldY * GRAPH_SCALE_FACTOR - height / 2, width, height);

                g.DrawImage(miniFieldImage, 20, 140);
                miniFieldImage.Dispose();
                dotField.Dispose();

                //Yard line number layouts
                for (int j = 0; j < 5; j++)
                {
                    g.FillRectangle(new SolidBrush(Color.White), new Rectangle((int)((4 * GRAPH_SCALE_FACTOR) + (j * 5 * Field.STANDARD_STEP_SIZE * GRAPH_SCALE_FACTOR)), (int)(150 + GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR), 40, 40));
                    g.DrawString(ydLabels[j].ToString(), textFont3, new SolidBrush(Color.Black), new PointF((4 * GRAPH_SCALE_FACTOR) + (j * 5 * Field.STANDARD_STEP_SIZE * GRAPH_SCALE_FACTOR), 150 + GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR));
                }

                //END
                img.Save("C:/Users/powme/Downloads/" + p.label + "/" + s.set + ".png");

                //throw new Exception("First, figure out where to draw the graphics");
                Form1.instance.fieldPanel.CreateGraphics().DrawImage(img, new Point(0, 0));
                img.Dispose();
                g.Dispose();
            }*/

            for (int i = 0; i < Drill.instance.sets.Count; i++)
            {
                using (Image img = DotBookMaker.MakeDotBook(p, i))
                {
                    img.Save("C:/Users/powme/Downloads/" + p.label + "/" + Drill.instance.sets[i].set + ".png");

                    Form1.instance.fieldPanel.CreateGraphics().DrawImage(img, new PointF(0, 0));
                }
            }
        }
    }
}
