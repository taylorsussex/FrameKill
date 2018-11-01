using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Solvers;

namespace FrameKill
{
    public partial class Form1 : Form
    {
        private string file;
        private List<string> moveNames = new List<string>();
        private List<int> frames = new List<int>();

        public Form1()
        {
            InitializeComponent();
            openFileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                file = openFileDialog.FileName;
                try
                {
                    using (var reader = new StreamReader(file))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line.Split(',');

                            try
                            {
                                moveNames.Add(values[0]);
                                frames.Add(Convert.ToInt32(values[1]));
                            }
                            catch (System.IndexOutOfRangeException)
                            {
                                textBox3.Text = "ERROR - Both columns should contain the same amount of rows.";
                                return;
                            }
                            catch (System.FormatException)
                            {
                                textBox3.Text = "ERROR - The second column of the source file should contain only integers.";
                                return;
                            }
                        }
                    }

                    textBox1.Text = file;
                    textBox3.Text = openFileDialog.SafeFileName + " successfully loaded.";
                    button2.Enabled = true;
                }
                catch (IOException)
                {
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SimplexSolver solver = new SimplexSolver();
            int[] decisionVariables = new int[frames.Count];
            int totalFrames, desiredFrames;

            try
            {
                desiredFrames = Convert.ToInt32(textBox2.Text);
            }
            catch (System.FormatException)
            {
                textBox3.Text = "ERROR - The desired amount of frames should be an integer.";
                return;
            }

            solver.AddRow("Total frames", out totalFrames);

            switch (comboBox1.SelectedItem)
            {
                case "Lower Bound":
                    solver.SetBounds(totalFrames, desiredFrames, Rational.PositiveInfinity);
                    solver.AddGoal(totalFrames, 1, true);
                    break;
                case "Upper Bound":
                    solver.SetBounds(totalFrames, 0, desiredFrames);
                    solver.AddGoal(totalFrames, 1, false);
                    break;
                default:
                    return;
            }

            for (int i = 0; i < frames.Count; i++)
            {
                solver.AddVariable(moveNames[i], out decisionVariables[i]);
                solver.SetBounds(decisionVariables[i], 0, Rational.PositiveInfinity);
                solver.SetIntegrality(decisionVariables[i], true);
                solver.SetCoefficient(totalFrames, decisionVariables[i], frames[i]);
            }

            solver.Solve(new SimplexSolverParams());

            string output = "";

            for (int i = 0; i < frames.Count; i++)
            {
                Rational currentVariable = solver.GetValue(decisionVariables[i]);

                if (currentVariable != 0)
                {
                    output += currentVariable.ToString() + "x" + moveNames[i] + ", ";
                }
            }

            output += "kills " + solver.GetSolutionValue(totalFrames).AbsoluteValue.ToString() + " frames";
            textBox3.Text = output;
        }
    }
}
