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

        string col="black";

        public play(string username, string opponent)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            roundlabel(path);
        }

        private void roundlabel(System.Drawing.Drawing2D.GraphicsPath path)
        {
            //path.AddEllipse(0, 0, label1.Width, label81.Height);
            //this.label1.Region = new Region(path);
            Graphics graphics = this.CreateGraphics();
            Rectangle rectangle = new Rectangle(100, 100, 200, 200);
            graphics.DrawEllipse(Pens.Black, rectangle);
        }

        private void labelc(Label sender)
        {
            sender.BackColor = Color.FromName(col);
            sender.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            labelc((Label)sender);
        }
    }
}
