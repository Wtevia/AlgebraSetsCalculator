using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public static AlgebraSetsCalculator setsCalc = new AlgebraSetsCalculator();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ActiveControl = panel1;
        }

        //This function makes all calculations
        private void button1_Click(object sender, EventArgs e)
        {
            label7.ResetText();
            label9.ResetText();
            setsCalc = new AlgebraSetsCalculator();
            setsCalc.setU.Add("null");
            string expression = "";

            //Enter elements in each set with ' ' between them
            try
            {
                //elements of set A
                expression = textBox2.Text;
                string[] strs = expression.Trim().Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    setsCalc.setA.Add(strs[i].Trim());
                }
                //elements of set B
                expression = textBox3.Text;
                strs = expression.Trim().Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    setsCalc.setB.Add(strs[i].Trim());
                }
                //elements of set C
                expression = textBox4.Text;
                strs = expression.Trim().Split(' ');
                for (int i = 0; i < strs.Length; i++)
                {
                    setsCalc.setC.Add(strs[i].Trim());
                }
                //elements of set U
                for (int i = 0; i < setsCalc.setA.Count; i++)
                {
                    if (!setsCalc.setU.Contains(setsCalc.setA[i]))
                        setsCalc.setU.Add(setsCalc.setA[i]);
                }
                for (int i = 0; i < setsCalc.setB.Count; i++)
                {
                    if (!setsCalc.setU.Contains(setsCalc.setB[i]))
                        setsCalc.setU.Add(setsCalc.setB[i]);
                }
                for (int i = 0; i < setsCalc.setC.Count; i++)
                {
                    if (!setsCalc.setU.Contains(setsCalc.setC[i]))
                        setsCalc.setU.Add(setsCalc.setC[i]);
                }
                label9.Text = string.Join(" ", setsCalc.setU);
                expression = textBox5.Text;

                //(A+B)/^C       
                string result = string.Join(" ", setsCalc.getSetByExpression(expression));
                label7.Text = result;
                if (label7.Text.Equals(""))
                    MessageBox.Show("Error. Try again.");
                panel1.Invalidate();
            } catch(Exception ex)
            {
                MessageBox.Show("Enter elements in all sets. Then enter Formula.\n"+ex.StackTrace, "Error: "+ex.Message);
            }
        }

        //This function draws Venn Diagram in panel1
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            int k = panel1.Width / 6;
            Rectangle recA = new Rectangle(new Point(panel1.Width/11, panel1.Height / 11), new Size(k * 3, k * 3));
            Rectangle recB = new Rectangle(new Point(recA.X + recA.Width / 3 * 2, recA.Y), new Size(k * 3, k * 3));
            Rectangle recC = new Rectangle(new Point(recA.X + recA.Width / 3, recA.Y + recA.Height / 12 * 7), new Size(k * 3, k * 3));
            Rectangle recU = new Rectangle(0, 0, panel1.Width, panel1.Height);
            Graphics g = e.Graphics;
            if (!label7.Text.Equals(""))
            {
                GraphicsPath pathA = new GraphicsPath();
                pathA.AddEllipse(recA);
                Region regA = new Region(pathA);
                GraphicsPath pathB = new GraphicsPath();
                pathB.AddEllipse(recB);
                Region regB = new Region(pathB);
                GraphicsPath pathC = new GraphicsPath();
                pathC.AddEllipse(recC);
                Region regC = new Region(pathC);
                GraphicsPath pathU = new GraphicsPath();
                pathU.AddRectangle(recU);
                Region regU = new Region(pathU);

                List<string> resSet = label7.Text.Trim().Split(' ').ToList();
                Region resultReg= new Region();
                resultReg.MakeEmpty();
                for (int i = 0; i < resSet.Count; i++)
                {

                    regA = new Region(pathA);
                    regB = new Region(pathB);
                    regC = new Region(pathC);
                    regU = new Region(pathU);

                    Region reg= new Region();
                    reg.MakeEmpty();
                    if (setsCalc.setA.Contains(resSet[i]))
                    {
                        if (setsCalc.setB.Contains(resSet[i]))
                        {
                            if (setsCalc.setC.Contains(resSet[i]))
                            {
                                reg = regA;
                                reg.Intersect(regB);
                                reg.Intersect(regC);
                            }
                            else
                            {
                                reg = regA;
                                reg.Intersect(regB);
                                reg.Exclude(regC);
                            }
                        }
                        else if (setsCalc.setC.Contains(resSet[i]))
                        {
                            reg = regA;
                            reg.Intersect(regC);
                            reg.Exclude(regB);
                        }
                        else
                        {
                            reg = regA;
                            reg.Exclude(regB);
                            reg.Exclude(regC);
                        }
                    }else if (setsCalc.setB.Contains(resSet[i]))
                    {
                        if (setsCalc.setC.Contains(resSet[i]))
                        {
                            reg = regB;
                            reg.Intersect(regC);
                            reg.Exclude(regA);
                        }
                        else
                        {
                            reg = regB;
                            reg.Exclude(regA);
                            reg.Exclude(regC);
                        }
                    }else if (setsCalc.setC.Contains(resSet[i]))
                    {
                        reg = regC;
                        reg.Exclude(regA);
                        reg.Exclude(regB);
                    }else
                    {
                        reg = regU;
                        reg.Exclude(regA);
                        reg.Exclude(regB);
                        reg.Exclude(regC);
                    }
                    resultReg.Union(reg);
                }
                for (int i = 0; i < setsCalc.setA.Count; i++)
                {
                    regA = new Region(pathA);
                    regB = new Region(pathB);
                    regC = new Region(pathC);
                    regU = new Region(pathU);

                    if (!resSet.Contains(setsCalc.setA[i]))
                        break;
                    if(i == setsCalc.setA.Count - 1)
                    {
                        regA.Exclude(regB);
                        regA.Exclude(regC);
                        resultReg.Union(regA);
                    }
                        
                }
                for (int i = 0; i < setsCalc.setB.Count; i++)
                {
                    regA = new Region(pathA);
                    regB = new Region(pathB);
                    regC = new Region(pathC);
                    regU = new Region(pathU);

                    if (!resSet.Contains(setsCalc.setB[i]))
                        break;
                    if (i == setsCalc.setB.Count - 1)
                    {
                        regB.Exclude(regA);
                        regB.Exclude(regC);
                        resultReg.Union(regB);
                    }
                }
                for (int i = 0; i < setsCalc.setC.Count; i++)
                {
                    regA = new Region(pathA);
                    regB = new Region(pathB);
                    regC = new Region(pathC);
                    regU = new Region(pathU);

                    if (!resSet.Contains(setsCalc.setC[i]))
                        break;
                    if (i == setsCalc.setC.Count - 1)
                    {
                        regC.Exclude(regA);
                        regC.Exclude(regB);
                        resultReg.Union(regC);
                    }
                }
                g.FillRegion(new SolidBrush(Color.Beige), resultReg);
            }
            g.DrawEllipse(new Pen(Color.Red), recA);
            g.DrawEllipse(new Pen(Color.Red), recB);
            g.DrawEllipse(new Pen(Color.Red), recC);
            g.DrawString("A", Font, new SolidBrush(Color.Black), recA.X + recA.Width / 2 - 5, recA.Y + recA.Height / 2 - 10);
            g.DrawString("B", Font, new SolidBrush(Color.Black), recB.X + recB.Width / 2 - 5, recB.Y + recB.Height / 2 - 10);
            g.DrawString("C", Font, new SolidBrush(Color.Black), recC.X + recC.Width / 2 - 5, recC.Y + recC.Height / 2);
            g.DrawString("U", Font, new SolidBrush(Color.Black), 10, 10);
        }

        //This function clears all sets and deletes all information
        private void button2_Click(object sender, EventArgs e)
        {
            setsCalc = new AlgebraSetsCalculator();
            setsCalc.setU.Add("null");
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            label7.ResetText();
            label9.ResetText();
            panel1.Invalidate();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            label7.ResetText();
            label9.ResetText();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            label7.ResetText();
            label9.ResetText();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            label7.ResetText();
            label9.ResetText();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            label7.ResetText();
            label9.ResetText();
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            panel1.Invalidate();
        }

        //This function resizes all element in Form1
        private void Form1_Resize(object sender, EventArgs e)
        {
            double kWidth = this.Width / 700.0;
            double kHeight = this.Height / 550.0;
            groupBox1.Location = new Point((int)(kWidth * 5), (int)(kHeight * 255));
            groupBox1.Size = new Size((int)(kWidth * 350), (int)(kHeight * 150));
            groupBox1.Font = new Font("Microsoft Sans Serif", (int)((kHeight+kWidth)/2*11) + 1);
            groupBox2.Location = new Point((int)(kWidth * 5), (int)(kHeight * 5)); //Location.X = (int)(kWidth * 5);
            groupBox2.Size = new Size((int)(kWidth * 350), (int)(kHeight * 250));
            groupBox2.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            groupBox3.Location = new Point((int)(kWidth * 350), (int)(kHeight * 5));
            groupBox3.Size = new Size((int)(kWidth * 300), (int)(kHeight * 400));
            groupBox3.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11 ) + 1);

            label4.Location = new Point((int)(kWidth * 10), (int)(kHeight * 410));
            label4.Size = new Size((int)(kWidth * 150), (int)(kHeight * 25));
            label4.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            label9.Location = new Point((int)(kWidth * 10), (int)(kHeight * 450));
            label9.Size = new Size((int)(kWidth * 200), (int)(kHeight * 25));
            label9.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);

            button1.Location = new Point((int)(kWidth * 250), (int)(kHeight * 410));
            button1.Size = new Size((int)(kWidth * 150), (int)(kHeight * 75));
            button1.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            button2.Location = new Point((int)(kWidth * 450), (int)(kHeight * 410));
            button2.Size = new Size((int)(kWidth * 150), (int)(kHeight * 75));
            button2.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11 ) + 1);
        }

        //This function resizes all element in groupBox1
        private void groupBox1_Resize(object sender, EventArgs e)
        {
            double kWidth = groupBox1.Width / 350.0;
            double kHeight = groupBox1.Height / 150.0;

            label5.Location = new Point((int)(kWidth * 10), (int)(kHeight * 25));
            label5.Size = new Size((int)(kWidth * 300), (int)(kHeight * 50));
            label5.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11)+1);

            textBox5.Location = new Point((int)(kWidth * 10), (int)(kHeight * 75));
            textBox5.Size = new Size((int)(kWidth * 300), (int)(kHeight * 50));
            textBox5.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11)+1);

        }

        //This function resizes all element in groupBox2
        private void groupBox2_Resize(object sender, EventArgs e)
        {
            double kWidth = groupBox2.Width / 350.0;
            double kHeight = groupBox2.Height / 250.0;

            label1.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            label1.Location = new Point((int)(kWidth * 10), (int)(kHeight * 30));
            label1.Size = new Size((int)(kWidth * 100), (int)(kHeight * 25));
            label2.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            label2.Location = new Point((int)(kWidth * 10), (int)(kHeight * 100));
            label2.Size = new Size((int)(kWidth * 100), (int)(kHeight * 25));
            label3.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            label3.Location = new Point((int)(kWidth * 10), (int)(kHeight * 170));
            label3.Size = new Size((int)(kWidth * 100), (int)(kHeight * 25));

            textBox2.Location = new Point((int)(kWidth * 10), (int)(kHeight * 60));
            textBox2.Size = new Size((int)(kWidth * 300), (int)(kHeight * 32));
            textBox2.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            textBox3.Location = new Point((int)(kWidth * 10), (int)(kHeight * 130));
            textBox3.Size = new Size((int)(kWidth * 300), (int)(kHeight * 32));
            textBox3.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
            textBox4.Location = new Point((int)(kWidth * 10), (int)(kHeight * 200));
            textBox4.Size = new Size((int)(kWidth * 300), (int)(kHeight * 32));
            textBox4.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);
        }


        //This function resizes all element in groupBox3
        private void groupBox3_Resize(object sender, EventArgs e)
        {
            double kWidth = groupBox3.Width / 300.0;
            double kHeight = groupBox3.Height / 400.0;

            label7.Location = new Point((int)(kWidth * 20), (int)(kHeight * 50));
            label7.Size = new Size((int)(kWidth * 200), (int)(kHeight * 25));
            label7.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);

            panel1.Location = new Point((int)0, (int)(kHeight * 100));
            panel1.Size = new Size((int)(kWidth * 300), (int)(kHeight * 300));
            panel1.Font = new Font("Microsoft Sans Serif", (int)((kHeight + kWidth) / 2 * 11) + 1);

        }
    }
}
