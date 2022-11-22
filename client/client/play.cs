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
            path.AddEllipse(0, 0, label1.Width, label1.Height);
            path.AddEllipse(0, 0, label1.Width, label2.Height);
            path.AddEllipse(0, 0, label1.Width, label3.Height);
            path.AddEllipse(0, 0, label1.Width, label4.Height);
            path.AddEllipse(0, 0, label1.Width, label5.Height);
            path.AddEllipse(0, 0, label1.Width, label6.Height);
            path.AddEllipse(0, 0, label1.Width, label7.Height);
            path.AddEllipse(0, 0, label1.Width, label8.Height);
            path.AddEllipse(0, 0, label1.Width, label9.Height);
            path.AddEllipse(0, 0, label1.Width, label10.Height);
            path.AddEllipse(0, 0, label1.Width, label11.Height);
            path.AddEllipse(0, 0, label1.Width, label12.Height);
            path.AddEllipse(0, 0, label1.Width, label13.Height);
            path.AddEllipse(0, 0, label1.Width, label14.Height);
            path.AddEllipse(0, 0, label1.Width, label15.Height);
            path.AddEllipse(0, 0, label1.Width, label16.Height);
            path.AddEllipse(0, 0, label1.Width, label17.Height);
            path.AddEllipse(0, 0, label1.Width, label18.Height);
            path.AddEllipse(0, 0, label1.Width, label19.Height);
            path.AddEllipse(0, 0, label1.Width, label20.Height);
            path.AddEllipse(0, 0, label1.Width, label21.Height);
            path.AddEllipse(0, 0, label1.Width, label22.Height);
            path.AddEllipse(0, 0, label1.Width, label23.Height);
            path.AddEllipse(0, 0, label1.Width, label24.Height);
            path.AddEllipse(0, 0, label1.Width, label25.Height);
            path.AddEllipse(0, 0, label1.Width, label26.Height);
            path.AddEllipse(0, 0, label1.Width, label27.Height);
            path.AddEllipse(0, 0, label1.Width, label28.Height);
            path.AddEllipse(0, 0, label1.Width, label29.Height);
            path.AddEllipse(0, 0, label1.Width, label30.Height);
            path.AddEllipse(0, 0, label1.Width, label31.Height);
            path.AddEllipse(0, 0, label1.Width, label32.Height);
            path.AddEllipse(0, 0, label1.Width, label33.Height);
            path.AddEllipse(0, 0, label1.Width, label34.Height);
            path.AddEllipse(0, 0, label1.Width, label35.Height);
            path.AddEllipse(0, 0, label1.Width, label36.Height);
            path.AddEllipse(0, 0, label1.Width, label37.Height);
            path.AddEllipse(0, 0, label1.Width, label38.Height);
            path.AddEllipse(0, 0, label1.Width, label39.Height);
            path.AddEllipse(0, 0, label1.Width, label40.Height);
            path.AddEllipse(0, 0, label1.Width, label41.Height);
            path.AddEllipse(0, 0, label1.Width, label42.Height);
            path.AddEllipse(0, 0, label1.Width, label43.Height);
            path.AddEllipse(0, 0, label1.Width, label44.Height);
            path.AddEllipse(0, 0, label1.Width, label45.Height);
            path.AddEllipse(0, 0, label1.Width, label46.Height);
            path.AddEllipse(0, 0, label1.Width, label47.Height);
            path.AddEllipse(0, 0, label1.Width, label48.Height);
            path.AddEllipse(0, 0, label1.Width, label49.Height);
            path.AddEllipse(0, 0, label1.Width, label50.Height);
            path.AddEllipse(0, 0, label1.Width, label51.Height);
            path.AddEllipse(0, 0, label1.Width, label52.Height);
            path.AddEllipse(0, 0, label1.Width, label53.Height);
            path.AddEllipse(0, 0, label1.Width, label54.Height);
            path.AddEllipse(0, 0, label1.Width, label55.Height);
            path.AddEllipse(0, 0, label1.Width, label56.Height);
            path.AddEllipse(0, 0, label1.Width, label57.Height);
            path.AddEllipse(0, 0, label1.Width, label58.Height);
            path.AddEllipse(0, 0, label1.Width, label59.Height);
            path.AddEllipse(0, 0, label1.Width, label60.Height);
            path.AddEllipse(0, 0, label1.Width, label61.Height);
            path.AddEllipse(0, 0, label1.Width, label62.Height);
            path.AddEllipse(0, 0, label1.Width, label63.Height);
            path.AddEllipse(0, 0, label1.Width, label64.Height);
            path.AddEllipse(0, 0, label1.Width, label65.Height);
            path.AddEllipse(0, 0, label1.Width, label66.Height);
            path.AddEllipse(0, 0, label1.Width, label67.Height);
            path.AddEllipse(0, 0, label1.Width, label68.Height);
            path.AddEllipse(0, 0, label1.Width, label69.Height);
            path.AddEllipse(0, 0, label1.Width, label70.Height);
            path.AddEllipse(0, 0, label1.Width, label71.Height);
            path.AddEllipse(0, 0, label1.Width, label72.Height);
            path.AddEllipse(0, 0, label1.Width, label73.Height);
            path.AddEllipse(0, 0, label1.Width, label74.Height);
            path.AddEllipse(0, 0, label1.Width, label75.Height);
            path.AddEllipse(0, 0, label1.Width, label76.Height);
            path.AddEllipse(0, 0, label1.Width, label77.Height);
            path.AddEllipse(0, 0, label1.Width, label78.Height);
            path.AddEllipse(0, 0, label1.Width, label79.Height);
            path.AddEllipse(0, 0, label1.Width, label80.Height);
            path.AddEllipse(0, 0, label1.Width, label81.Height);
            this.label1.Region = new Region(path);
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

        //https://stackoverflow.com/questions/11502338/how-to-display-message-box-without-any-buttons-in-c-sharp
    }
}
