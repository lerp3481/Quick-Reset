using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public partial class Form1 : Form
    {
        public FieldPanel fieldPanel;
        public FieldSettingsPanel fieldSettingsPanel;
        public SetPanel setPanel;
        public PerformerPanel performerPanel;
        public ProgramPanel programPanel;

        public static Form1 instance = null;

        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            Shown += SetupUI;

            instance = this;
        }

        private void SetupUI(object sender, EventArgs e)
        {
            fieldPanel = new FieldPanel();
            fieldSettingsPanel = new FieldSettingsPanel();
            setPanel = new SetPanel();
            performerPanel = new PerformerPanel();
            programPanel = new ProgramPanel();

            Controls.AddRange(new Control[] { fieldPanel, fieldSettingsPanel, setPanel, performerPanel, programPanel });

            int uiX = fieldPanel.Width;
            int uiY = fieldPanel.Height + 24;

            fieldPanel.Location = new Point(0, 24);
            fieldSettingsPanel.Location = new Point(uiX, 24);
            setPanel.Location = new Point(uiX, 24 + fieldSettingsPanel.Height);
            performerPanel.Location = new Point(uiX, 24 + fieldSettingsPanel.Height + setPanel.Height);
            programPanel.Location = new Point(uiX, 24 + fieldSettingsPanel.Height + setPanel.Height + performerPanel.Height);

            fieldPanel.RESET_IMAGE = true;
            fieldPanel.Invalidate();
            fieldPanel.Update();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void openPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PdfParser parser = new PdfParser(ofd.FileName);
                Drill.instance = parser.ConvertToDrill();

                OnLoadDrill();
            }
        }

        private void appendFromPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                PdfParser parser = new PdfParser(ofd.FileName);
                Drill newDrill = parser.ConvertToDrill();

                //Sometimes the end of part 1 can be repeated in the beginning of the part 2 for example, and that messes with our program
                //...or sometimes, the drill writer adds an extra empty set for no reason...
                bool lastSetDuplicate = newDrill.sets[0].counts == 0;
                //lastSetDuplicate = Drill.instance.sets.Any(x => x.set == newDrill.sets[0].set) || (!Drill.instance.sets.Any(x => x.set == newDrill.sets[0].set) && newDrill.sets[0].counts == 0);

                //Add sets
                int oldNumSets = Drill.instance.sets.Count;
                newDrill.sets = newDrill.sets.Skip(lastSetDuplicate ? 1 : 0).ToList();
                for (int i = 0; i < newDrill.sets.Count; i++)
                {
                    //Sigh, why is this so needlessly complicated
                    Set s = newDrill.sets[i];
                    s.privateID += oldNumSets - 1;
                    newDrill.sets[i] = s;
                }
                Drill.instance.sets.AddRange(newDrill.sets);
                //Add dots (and performers if necessary)
                for (int i = 0; i < newDrill.performers.Count; i++)
                {
                    if (Drill.instance.performers.Any(x => x.label == newDrill.performers[i].label))
                    {
                        //Performer is in both the previous part and the new part!
                        Drill.instance.GetPerformerByLabel(newDrill.performers[i].label).dots.AddRange(newDrill.performers[i].dots.Skip(lastSetDuplicate ? 1 : 0));
                    }
                    else
                    {
                        //New performer!
                        Performer p = newDrill.performers[i];
                        if (lastSetDuplicate) p.dots = p.dots.Skip(1).ToList();
                        p.dots.InsertRange(0, new Dot[oldNumSets]);
                        p.firstActiveSet = oldNumSets;

                        Drill.instance.performers.Add(p);
                    }
                }
                //Now let's check if any performers have been removed
                for (int i = 0; i < Drill.instance.performers.Count; i++)
                {
                    if (!newDrill.performers.Any(x => x.label == Drill.instance.performers[i].label))
                    {
                        //Missing performer!
                        if (Drill.instance.performers[i].lastActiveSet == -1) Drill.instance.performers[i].lastActiveSet = oldNumSets - 1;
                    }
                }

                OnLoadDrill();
            }
        }

        private void openDrillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Stream loadFile = new FileStream(ofd.FileName, FileMode.Open);
                DrillFile.Load(loadFile);
                loadFile.Close();

                OnLoadDrill();
            }
        }

        private void saveDrillToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Stream saveFile = new FileStream(sfd.FileName, FileMode.Create);
                DrillFile.Save(saveFile);
                saveFile.Close();
            }
        }

        private void OnLoadDrill()
        {
            //Some things we call whenever drill is loaded
            Field.X_EXCESS = (int)Math.Ceiling(Drill.instance.GetSuggestedXExcess());
            Field.Y_EXCESS = (int)Math.Ceiling(Drill.instance.GetSuggestedYExcess());
            fieldPanel.UpdateField();
            fieldPanel.DrawDots(0);
            programPanel.Populate();

            File.WriteAllLines("C:/Users/powme/Downloads/res.txt", Drill.instance.RankPerformersByDifficulty());

            //VideoExporter temp = new VideoExporter("C:/Users/powme/Downloads/ita.mp4", 30, "C:/Users/powme/Downloads/chhs_drill/2022intothinair/ita.mp3");
            //VideoExporter temp = new VideoExporter("C:/Users/powme/Downloads/spiritshow.mp4", 30, "C:/Users/powme/Downloads/chhs_drill/2020spiritshow/spiritshow.mp3");
        }
    }
}
