using ComparadorWebRequests.Logic.Comparison;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ComparadorWebRequests.Forms
{
    public partial class ResultForm : Form
    {
        public ResultForm(ComparisonResult result)
        {
            InitializeComponent();
            DisplayResult(result);
        }

        private void DisplayResult(ComparisonResult result)
        {
            txtDifferences.Clear();

            foreach (var line in result.Results)
            {
                string displayText = line.Status switch
                {
                    ComparisonResult.LineStatus.Equal => $"✔️ {line.LineLeft}",
                    ComparisonResult.LineStatus.Different => $"⚠️ {line.LineLeft} ⇄ {line.LineRight}",
                    ComparisonResult.LineStatus.MissingLeft => $"❌ (Robô apenas) {line.LineRight}",
                    ComparisonResult.LineStatus.MissingRight => $"❌ (Portal apenas) {line.LineLeft}",
                    _ => ""
                };

                // Definir cor
                switch (line.Status)
                {
                    case ComparisonResult.LineStatus.Equal:
                        txtDifferences.SelectionColor = Color.Black;
                        break;
                    case ComparisonResult.LineStatus.Different:
                        txtDifferences.SelectionColor = Color.Orange;
                        break;
                    case ComparisonResult.LineStatus.MissingLeft:
                    case ComparisonResult.LineStatus.MissingRight:
                        txtDifferences.SelectionColor = Color.Red;
                        break;
                }

                txtDifferences.AppendText(displayText + Environment.NewLine);
            }
        }
    }
}
