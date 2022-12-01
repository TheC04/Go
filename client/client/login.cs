using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Go
{
    public partial class login : Form
    {
        string username;
        public login()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                username = textBox1.Text;
                Form m = new play(username);
                m.Visible = true;
                this.Visible = false;
            }
            else if (!char.IsLetterOrDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(8) && e.KeyChar != ' ')
            {
                MessageBox.Show("Non si possono usere caratteri speciali", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
        }

        private void start_Click(object sender, EventArgs e)
        {
            Console.WriteLine("aaa");
        }
    }
}
