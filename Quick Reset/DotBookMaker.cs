using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quick_Reset
{
    class DotBookMaker
    {
        //Layout settings (location is in reference to top-left corner of dot book)
        public static Size DOTBOOK_SIZE = new Size(600, 500);

        public static Size SETLYT_SIZE = new Size(150, 50);
        public static Point SETLYT_LOC = new Point(20, 20);

        public static Size CNTLYT_SIZE = new Size(150, 50);
        public static Point CNTLYT_LOC = new Point(20, 80);

        public static Size COORDLYT_SIZE = new Size(400, 110);
        public static Point COORDLYT_LOC = new Point(180, 20);

        public static Size MIDLYT_SIZE = new Size(270, 150);
        public static Point MIDLYT_LOC = new Point(310, 140);

        public static float GRAPH_SCALE_FACTOR = 5.5f;
        public static int GRAPH_FIELD_LENGTH = (int)(25 * Field.STANDARD_STEP_SIZE);
        public static int GRAPH_FIELD_WIDTH = (int)(20 * Field.STANDARD_STEP_SIZE);

        public static Size FIELDLYT_SIZE = new Size((int)(GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR), (int)(GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR));
        public static Point FIELDLYT_LOC = new Point(20, 140);

        public static Size YDLINES_SIZE = new Size(40, 40);
        public static int YDLINES_Y = 150; //Distance between the bottom of the field layout and the numbers

        //Text settings
        public static Font LargeFont = new Font(FontFamily.GenericSansSerif, 48); //Used for set number and counts
        public static Font SmallFont = new Font(FontFamily.GenericSansSerif, 16); //Used for coordinates

        public static StringFormat CenteredText = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };
        public static StringFormat LeftText = new StringFormat()
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };

        public static Point LargeTextOffset = new Point(75, 25);
        public static Point SmallTextOffset = new Point(0, 25);

        public static Image MakeDotBook(Performer p, int set)
        {
            Set s = Drill.instance.sets[set];
            Dot dot = p.dots[set];
            Dot previousDot = set != 0 ? p.dots[set - 1] : new Dot();

            Image img = new Bitmap(DOTBOOK_SIZE.Width, DOTBOOK_SIZE.Height);
            Graphics g = Graphics.FromImage(img);

            p.isHighlighted = true;

            MakeLayouts(g, dot, previousDot, s);

            g.Dispose();
            return img;
        }

        public static void MakeLayouts(Graphics g, Dot d, Dot pd, Set s)
        {
            //Background
            g.Clear(Color.FromArgb(100, 100, 100));

            //Set layout
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(SETLYT_LOC, SETLYT_SIZE));
            g.DrawString(s.set, LargeFont, new SolidBrush(Color.Black), SETLYT_LOC.X + LargeTextOffset.X, SETLYT_LOC.Y + LargeTextOffset.Y, CenteredText);

            //Counts layout
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(CNTLYT_LOC, CNTLYT_SIZE));
            g.DrawString(s.counts.ToString(), LargeFont, new SolidBrush(Color.Black), CNTLYT_LOC.X + LargeTextOffset.X, CNTLYT_LOC.Y + LargeTextOffset.Y, CenteredText);

            //Coordinate layout
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(COORDLYT_LOC, COORDLYT_SIZE));
            g.DrawString(s.privateID != 0 && d.IsSame(pd) ? "<SAME>" : d.LeftToRightToString(), SmallFont, new SolidBrush(Color.Black), COORDLYT_LOC.X + SmallTextOffset.X, COORDLYT_LOC.Y + SmallTextOffset.Y, LeftText);
            g.DrawString(s.privateID != 0 && d.IsSame(pd) ? "<SAME>" : d.FrontToBackToString(), SmallFont, new SolidBrush(Color.Black), COORDLYT_LOC.X + SmallTextOffset.X, COORDLYT_LOC.Y + SmallTextOffset.Y + SmallFont.Height, LeftText);

            //Midset layout
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(MIDLYT_LOC, MIDLYT_SIZE));
            g.DrawString(s.privateID == 0 || d.IsSame(pd) ? "----" : d.MakeMidpointDot(pd).LeftToRightToString(), SmallFont, new SolidBrush(Color.Black), MIDLYT_LOC.X + SmallTextOffset.X, MIDLYT_LOC.Y + SmallTextOffset.Y, LeftText);
            g.DrawString(s.privateID == 0 || d.IsSame(pd) ? "----" : d.MakeMidpointDot(pd).FrontToBackToString(), SmallFont, new SolidBrush(Color.Black), MIDLYT_LOC.X + SmallTextOffset.X, MIDLYT_LOC.Y + SmallTextOffset.Y + SmallFont.Height, LeftText);

            //Graph layout
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(FIELDLYT_LOC, FIELDLYT_SIZE));
            using (Image miniFieldImage = DrawMiniField(d, pd, s, s.privateID != 0 && !d.IsSame(pd)))
            {
                g.DrawImage(miniFieldImage, FIELDLYT_LOC);
            }
        }

        public static Image DrawMiniField(Dot d, Dot pd, Set s, bool drawPreviousDot)
        {
            Image img = new Bitmap(FIELDLYT_SIZE.Width, FIELDLYT_SIZE.Height);
            Graphics g = Graphics.FromImage(img);

            //Draw the gridlines
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
                g.DrawLine(linePen, x * GRAPH_SCALE_FACTOR, 0, x * GRAPH_SCALE_FACTOR, GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR);
            }
            for (int y = 0; y <= GRAPH_FIELD_WIDTH; y++)
            {
                //Horizontal gridlines
                linePen.Color = Color.FromArgb(192, 192, 192);
                if ((float)y % (float)(Field.STANDARD_STEP_SIZE * 2.5f) == 0)
                {
                    linePen.Color = Color.FromArgb(127, 127, 255);
                }
                g.DrawLine(linePen, 0, y * GRAPH_SCALE_FACTOR, GRAPH_FIELD_LENGTH * GRAPH_SCALE_FACTOR, y * GRAPH_SCALE_FACTOR);
            }

            //Fit this dot into the graph
            /*
            float fieldX = (d.lefttoright + (Field.STANDARD_STEP_SIZE * 2.5f)) % GRAPH_FIELD_LENGTH;
            float fieldY = d.fronttoback % GRAPH_FIELD_WIDTH;

            float deltaX = d.lefttoright - pd.lefttoright;
            float deltaY = d.fronttoback - pd.fronttoback;

            float previousFieldX = fieldX - deltaX;
            float previousFieldY = fieldY + deltaY;*/

            //See if we can also fit

            //------------------OLD CODE STARTS NOW
            Dot dot = d;
            Dot previousDot = pd;
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

            if (drawPreviousDot)
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
            Graphics dotField = g;
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

            //g.DrawImage(miniFieldImage, 20, 140);
            //miniFieldImage.Dispose();
            dotField.Dispose();

            //Yard line number layouts
            Font textFont3 = new Font(FontFamily.GenericSansSerif, 30);
            for (int j = 0; j < 5; j++)
            {
                //g.FillRectangle(new SolidBrush(Color.White), new Rectangle((int)((4 * GRAPH_SCALE_FACTOR) + (j * 5 * Field.STANDARD_STEP_SIZE * GRAPH_SCALE_FACTOR)), (int)(150 + GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR), 40, 40));
                //g.DrawString(ydLabels[j].ToString(), textFont3, new SolidBrush(Color.Black), new PointF((4 * GRAPH_SCALE_FACTOR) + (j * 5 * Field.STANDARD_STEP_SIZE * GRAPH_SCALE_FACTOR), 150 + GRAPH_FIELD_WIDTH * GRAPH_SCALE_FACTOR));
            }

            g.Dispose();
            return img;
        }
    }
}
