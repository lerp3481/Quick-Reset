using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public class ProgramPanel : Panel
    {
        public Button addProgram;
        public Button removeProgram;
        public Button updateProgram;
        public ListBox programList;
        public ComboBox tempoMode;
        public NumericUpDown countInSet;
        public ComboBox set;
        public NumericUpDown tempo;
        public Label setLabel;
        public Label countLabel;
        public Label tempoModeLabel;
        public Label tempoLabel;

        public ProgramPanel()
        {
            AutoSize = true;

            addProgram = new Button();
            removeProgram = new Button();
            updateProgram = new Button();
            programList = new ListBox();
            tempoMode = new ComboBox();
            countInSet = new NumericUpDown();
            set = new ComboBox();
            tempo = new NumericUpDown();
            setLabel = new Label();
            countLabel = new Label();
            tempoModeLabel = new Label();
            tempoLabel = new Label();

            Controls.AddRange(new Control[] { addProgram, removeProgram, updateProgram, programList, tempoMode, countInSet, set, tempo, setLabel, countLabel, tempoModeLabel, tempoLabel });

            int uiX = 0, uiY = 0;

            programList.Location = new Point(uiX, uiY);
            programList.Size = new Size(100, 200); //idk

            uiY += programList.Height;

            addProgram.Text = "Add program";
            addProgram.Location = new Point(uiX, uiY);
            addProgram.AutoSize = true;

            uiX += addProgram.Width;

            removeProgram.Text = "Remove program";
            removeProgram.Location = new Point(uiX, uiY);
            removeProgram.AutoSize = true;

            uiX += removeProgram.Width;

            updateProgram.Text = "Update program";
            updateProgram.Location = new Point(uiX, uiY);
            updateProgram.AutoSize = true;

            uiX = programList.Width;
            uiY = 0;

            setLabel.Text = "Set:";
            setLabel.Location = new Point(uiX, uiY);

            uiX += setLabel.Width;

            set.Location = new Point(uiX, uiY);
            set.Width = 100;

            uiX = programList.Width;
            uiY += setLabel.Height;

            countLabel.Text = "Count:";
            countLabel.Location = new Point(uiX, uiY);

            uiX += countLabel.Width;

            countInSet.Value = 1;
            countInSet.Location = new Point(uiX, uiY);
            countInSet.Width = 50;

            uiX = programList.Width;
            uiY += countLabel.Height;

            tempoLabel.Text = "Tempo:";
            tempoLabel.Location = new Point(uiX, uiY);

            uiX += tempoLabel.Width;

            tempo.Value = 100;
            tempo.Maximum = 999;
            tempo.Location = new Point(uiX, uiY);
            tempo.Width = 50;

            uiX = programList.Width;
            uiY += tempoLabel.Height;

            tempoModeLabel.Text = "Tempo mode:";
            tempoModeLabel.Location = new Point(uiX, uiY);

            uiX += tempoModeLabel.Width;

            tempoMode.Items.AddRange(new string[] { "Static", "Dynamic start", "Dynamic end"});
            tempoMode.Location = new Point(uiX, uiY);

            addProgram.Click += AddProgram_Click;
            removeProgram.Click += RemoveProgram_Click;
            updateProgram.Click += UpdateProgram_Click;
            programList.SelectedIndexChanged += ProgramList_Update;
        }

        public ProgramSettings ParseSettings()
        {
            ProgramSettings ret = new ProgramSettings();

            Set thisSet = Drill.instance.GetSetByName(set.SelectedItem.ToString());
            ret.absoluteStartCount = Drill.instance.playbackManager.CalculateAbsoluteCount(thisSet.privateID) + ((int)countInSet.Value - 1);
            ret.tempoMode = (ProgramSettings.TempoMode)tempoMode.SelectedIndex;
            ret.tempo = (int)tempo.Value;

            return ret;
        }

        private void AddProgram_Click(object sender, EventArgs e)
        {
            Drill.instance.playbackManager.program.Add(ParseSettings());
            Drill.instance.playbackManager.program.OrderBy(x => x.absoluteStartCount);
            UpdateProgramList();
        }

        private void RemoveProgram_Click(object sender, EventArgs e)
        {
            Drill.instance.playbackManager.program.RemoveAt(programList.SelectedIndex);
            UpdateProgramList();
        }

        private void UpdateProgram_Click(object sender, EventArgs e)
        {
            Drill.instance.playbackManager.program[programList.SelectedIndex] = ParseSettings();
            Drill.instance.playbackManager.program.OrderBy(x => x.absoluteStartCount);
            UpdateProgramList();
        }

        private void ProgramList_Update(object sender, EventArgs e)
        {
            ProgramSettings setting = Drill.instance.playbackManager.program[programList.SelectedIndex];

            string[] settingArray = setting.ToString().Split(' ');
            Set thisSet = Drill.instance.GetSetByName(settingArray[1]);
            int countsIn = int.Parse(settingArray[3]);

            set.SelectedIndex = thisSet.privateID;
            countInSet.Value = countsIn;

            tempoMode.SelectedIndex = (int)setting.tempoMode;

            tempo.Value = setting.tempo;
        }

        public void Populate()
        {
            UpdateProgramList();
            set.Items.Clear();
            set.Items.AddRange(Drill.instance.sets.Select(x => x.set).ToArray());
        }

        public void UpdateProgramList()
        {
            programList.Items.Clear();
            programList.Items.AddRange(Drill.instance.playbackManager.program.Select(x => x.ToString()).ToArray());
        }
    }
}
