using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Go
{
    public partial class play : Form
    {

        string me, op;

        public play(string username, string opponent)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
        }

        private void roundlabel(System.Drawing.Drawing2D.GraphicsPath path, Label l)
        {
            path.AddEllipse(0, 0, l.Width, l.Height);
            l.Region = new Region(path);
            Graphics graphics = this.CreateGraphics();
            Rectangle rectangle = new Rectangle(100, 100, 200, 200);
            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        private void labelc(Label sender)
        {
            sender.BackColor = Color.FromName(me);
            sender.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            labelc((Label)sender);
        }

        private void create()
        {
            Label[][] lb = new Label[9][];
            for (int i = 0; i < 9; i++)
            {
                lb[i] = new Label[9];
            }
            int posLeft = 70;
            int posHeight = 70;


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {

                    lb[i][j] = new Label();
                    lb[i][j].Name = Convert.ToString((i) + "-" + (j));
                    lb[i][j].Click += new EventHandler(labelc);


                    if (j == 0)
                    {
                        lb[i][j].Left = posLeft;
                        lb[i][j].Height = posHeight * (i + 1);
                    }

                    if (j > 0)
                    {
                        lb[i][j].Left = lb[i][j - 1].Right;
                        lb[i][j].Height = lb[i][j - 1].Height;
                    }

                    this.Controls.Add(lb[i][j]);

                }


            }
        }
    }
}
