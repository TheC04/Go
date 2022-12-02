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
        }

        private void white_Click(object sender, EventArgs e)
        {
            Form f = new play(white.Name);
            f.Show();
            this.Hide();
        }

        private void black_Click(object sender, EventArgs e)
        {
            Form f = new play(black.Name);
            f.Show();
            this.Hide();
        }
    }
}
