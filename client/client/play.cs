using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using client.Properties;

namespace Go
{
    public partial class play : Form
    {
        Thread t;
        string me;
        bool turn = false;
        byte[] bytes = new byte[1024];
        Label[][] lb = new Label[9][];
        string username, op;
        bool opponent = false;
        Thread listener, waiter;
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
        Socket sok = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        public play(string u)
        {
            InitializeComponent();
            username = u;
            label1.Hide();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void listen()
        {
            string Sender = username;
            byte[] bytes = new byte[1024];
            try
            {
                sok.Connect(remoteEP);
                byte[] msg = Encoding.ASCII.GetBytes(Sender + "**");
                int bytesSent = sok.Send(msg);
                if (Encoding.ASCII.GetString(bytes).Split('*')[0] != "ok")
                {
                    sok.Receive(bytes);
                }
                while (bytes.ToString().IndexOf("**") == -1)
                {
                    op = bytes.ToString().Split('*')[0];
                    MessageBox.Show("Stai per sfidare " + op);
                    opponent = true;
                }
                sok.Send(Encoding.ASCII.GetBytes("ok**"));
                while (bytes.ToString().IndexOf("**") == -1)
                {
                    op = bytes.ToString().Split('*')[0];
                    MessageBox.Show("Sarai il " + op);
                    me = op;
                }
                chform();
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception en)
            {
                Console.WriteLine("Unexpected exception : {0}", en.ToString());
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Annulla")
            {
                MessageBox.Show("Ricerca avversario annullata");
                Form f = new login();
                f.Show();
                this.Hide();
            }
            else
            {
                button1.Text = "Annulla";
                label1.Show();
                listener = new Thread(new ThreadStart(listen));
                waiter = new Thread(new ThreadStart(waiting));
                listener.Start();
                waiter.Start();
            }
        }
        private void waiting()
        {
            int c = 0;
            while (!opponent)
            {
                if (c == 3)
                {
                    c = 0;
                    label1.Text = "In attesa di un avversario";
                }
                else
                {
                    c++;
                    label1.Text += ".";
                }
                Thread.Sleep(750);
            }
        }

        void chform()
        {
            this.Width = client.Properties.Resources.field.Width + 5;
            this.Height = client.Properties.Resources.field.Height + 13;
            this.BackgroundImage = new Bitmap(Resources.field);
            this.BackgroundImageLayout = ImageLayout.None;
            button1.Visible = false;
            label1.Visible = false;
            t = new Thread(getC);
            create();
        }
        private void create()
        {
            for (int i = 0; i < 9; i++)
            {
                lb[i] = new Label[9];
            }
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    lb[i][j] = new Label();
                    lb[i][j].Name = Convert.ToString((i) + ";" + (j));
                    lb[i][j].Click += new EventHandler(label1_Click);
                    lb[i][j].TextAlign = ContentAlignment.MiddleCenter;
                    lb[i][j].Width = 20;
                    lb[i][j].Height = 20;
                    lb[i][j].BackColor = Color.Transparent;
                    if (i == 0)
                    {
                        lb[i][j].Left = 1;
                    }
                    else if (i > 0)
                    {
                        lb[i][j].Left = lb[i - 1][j].Left + 51;
                    }
                    if (j == 0)
                    {
                        lb[i][j].Top = 1;
                    }
                    else if (j > 0)
                    {

                        lb[i][j].Top = lb[i][j - 1].Top + 51;
                    }
                    this.Controls.Add(lb[i][j]);
                }
            }
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
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int c = int.Parse(sender.Name.Split(';')[0]), r = int.Parse(sender.Name.Split(';')[1]);
            if(checksuicide(c, r))
            {
                if (sender.BackColor == Color.Transparent)
                {
                    roundlabel(path, sender);
                    sender.BackColor = Color.FromName(me);
                    sender.Enabled = false;
                    sok.Send(Encoding.ASCII.GetBytes(sender.Name));
                }
                else
                {
                    MessageBox.Show("Non puoi mettere qui la tua pedina");
                }
            }
            else
            {
                MessageBox.Show("Non puoi mettere una pedina dove verrebbe mangiata");
            }
            
        }
        bool checksuicide(int c, int r)
        {
            bool ok = true;
            if (c == 0)
            {
                if (r == 0)
                {
                    if (lb[c + 1][r].BackColor!=Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else if (r == 8)
                {
                    if (lb[c + 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (lb[c + 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
            }
            else if (c == 8)
            {
                if (r == 0)
                {
                    if (lb[c - 1][r].BackColor != Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else if (r == 8)
                {
                    if (lb[c - 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (lb[c - 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
            }
            else
            {
                if (r == 0)
                {
                    if (lb[c + 1][r].BackColor != Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me) && lb[c - 1][r].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else if (r == 8)
                {
                    if (lb[c + 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me) && lb[c - 1][r].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (lb[c + 1][r].BackColor != Color.FromName(me) && lb[c][r - 1].BackColor != Color.FromName(me) && lb[c][r + 1].BackColor != Color.FromName(me) && lb[c - 1][r].BackColor != Color.FromName(me))
                    {
                        ok = false;
                    }
                }
            }
            return ok;
        }
        private void label1_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (turn)
            {
                labelc((Label)sender);
                turn = false;
            }
            else
            {
                i++;
            }
            if (i == 3)
            {
                MessageBox.Show("Fai giocare anche il tuo avversario");
            }
            else if (i == 6)
            {
                MessageBox.Show("Non tocca a te, smettila");
            }
            else if (i == 9)
            {
                MessageBox.Show("Prova a cliccare di nuovo, ti sfido, due volte, ti sfido");
            }
            else if (i == 10)
            {
                sok.Shutdown(SocketShutdown.Both);
                sok.Close();
                this.Close();
            }
            Thread t = new Thread(new ThreadStart(recive));
        }
        private void recive()
        {
            string tab = "", name = "";
            while (tab.Length != 81)
            {
                tab += sok.Receive(bytes).ToString().Split('*')[0];
                if (tab == "end")
                {
                    while (name.IndexOf("*") != -1)
                    {
                        name += sok.Receive(bytes).ToString().Split('*')[0];
                        if (name == username)
                        {
                            sok.Shutdown(SocketShutdown.Both);
                            sok.Close();
                            MessageBox.Show("Hai vinto!");
                            Form f = new login();
                            f.Show();
                            this.Close();
                        }
                        else if (name == "draw!")
                        {
                            sok.Shutdown(SocketShutdown.Both);
                            sok.Close();
                            MessageBox.Show("Pareggio");
                            Form f = new login();
                            f.Show();
                            this.Close();
                        }
                        else
                        {
                            sok.Shutdown(SocketShutdown.Both);
                            sok.Close();
                            MessageBox.Show("Hai perso...");
                            Form f = new login();
                            f.Show();
                            this.Close();
                        }
                    }
                }
            }
            int i = 0, j = 0;
            while (i != 8 && j != 8)
            {
                {
                    if (bytes.ToString().ElementAt(i) == '0')
                    {
                        lb[j][i].BackColor = Color.Transparent;
                    }
                    else if (bytes.ToString().ElementAt(i) == '1')
                    {
                        lb[j][i].BackColor = Color.White;
                    }
                    else
                    {
                        lb[j][i].BackColor = Color.Black;
                    }
                    if (i == 8)
                    {
                        i = 0;
                        j++;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        private void play_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void getC()
        {
            try
            {
                try
                {
                    sok.Connect(remoteEP);
                    while (bytes.ToString().Length > 0)
                    {
                        int bytesRec = sok.Receive(bytes);
                    }
                    while (me != "white" && me != "black")
                    {
                        try
                        {
                            me = bytes.ToString().Split('*')[0];
                            if (me == "white")
                            {
                                turn = true;
                            }
                            else
                            {
                                turn = false;
                            }
                            sok.Send(Encoding.ASCII.GetBytes("ok**"));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Errore di comunicazione con il server");
                        }
                    }
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception en)
                {
                    Console.WriteLine("Unexpected exception : {0}", en.ToString());
                }

            }
            catch (Exception en)
            {
                Console.WriteLine(en.ToString());
            }
        }

        private void play_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
        }
    }
}
