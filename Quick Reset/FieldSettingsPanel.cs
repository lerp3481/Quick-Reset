using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quick_Reset
{
    public class FieldSettingsPanel : Panel
    {
        public Label stepsizeText;
        public NumericUpDown stepsizeSteps;
        public Label stepsizeSeparator;
        public NumericUpDown stepsizeDistance;
        public Label drawScaleFactorText;
        public NumericUpDown drawScaleFactor;
        public Button updateField;

        public FieldSettingsPanel()
        {
            AutoSize = true;

            stepsizeText = new Label();
            stepsizeSteps = new NumericUpDown();
            stepsizeSeparator = new Label();
            stepsizeDistance = new NumericUpDown();
            drawScaleFactorText = new Label();
            drawScaleFactor = new NumericUpDown();
            updateField = new Button();

            Controls.AddRange(new Control[] { stepsizeText, stepsizeSteps, stepsizeSeparator, stepsizeDistance, drawScaleFactorText, drawScaleFactor, updateField });

            int uiX = 0, uiY = 0;

            stepsizeText.Text = "Step size (gridlines): ";
            stepsizeText.Location = new Point(uiX, uiY);

            uiX += stepsizeText.Width;

            stepsizeSteps.Value = 10;
            stepsizeSteps.Width = 40;
            stepsizeSteps.Location = new Point(uiX, uiY);

            uiX += stepsizeSteps.Width;

            stepsizeSeparator.Text = "to";
            stepsizeSeparator.Width = 20;
            stepsizeSeparator.Location = new Point(uiX, uiY);

            uiX += stepsizeSeparator.Width;

            stepsizeDistance.Value = 5;
            stepsizeDistance.Width = 40;
            stepsizeDistance.Location = new Point(uiX, uiY);

            uiX = 0;
            uiY += stepsizeText.Height;

            drawScaleFactorText.Text = "Field scale factor: ";
            drawScaleFactorText.Location = new Point(uiX, uiY);

            uiX += drawScaleFactorText.Width;

            drawScaleFactor.Value = 8;
            drawScaleFactor.Location = new Point(uiX, uiY);

            uiX = 0;
            uiY += drawScaleFactorText.Height;

            updateField.Text = "Update field";
            updateField.Location = new Point(uiX, uiY);

            updateField.Click += UpdateField_Click;
        }

        private void UpdateField_Click(object sender, EventArgs e)
        {
            Form1.instance.fieldPanel.UpdateField();
        }
    }
}
