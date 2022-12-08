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
using System.Reflection;
using System.Runtime.InteropServices;

namespace Go
{
    public partial class play : Form
    {
        Thread t;
        bool turn, end = true, opponent = false;
        byte[] bytes = new byte[1024];
        Label[][] lb = new Label[9][];
        string color, op;
        Thread listener, waiter;
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
        Socket sok = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        public play(string u)
        {
            InitializeComponent();
            color = u;
            label1.Hide();
            CheckForIllegalCrossThreadCalls = false;
            if (color == "white")
            {
                turn = false;
                op = "black";
            }
            else
            {
                turn = true;
                op = "white";
            }
        }

        private delegate void MyDelegate();
        private void listen()
        {
            byte[] bytes = new byte[1024];
            try
            {
                bool connected = false;
                byte[] msg = Encoding.ASCII.GetBytes(color.ToLower() + "**");
                while (!connected)
                {
                    sok.Connect(remoteEP);
                    int bytesSent = sok.Send(msg);
                    if (sok.Connected)
                    {
                        connected = true;
                    }
                }
                while (Encoding.ASCII.GetString(bytes).IndexOf("**") == -1)
                {
                    sok.Receive(bytes);
                }
                this.Invoke((MyDelegate)delegate
                {
                    chform();
                });
                sok.Send(Encoding.ASCII.GetBytes("ready**"));
                Array.Clear(bytes, 0, bytes.Length);
                string op = "";
                while (true)
                {
                    checkwin();
                    if (!end)
                    {
                        while (op.IndexOf("**") == -1)
                        {
                            int bytesRec = sok.Receive(bytes);
                            op += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }
                        op = op.Split('*')[0];
                        if (op != "move")
                        {
                            recive(op);
                            op = "";
                        }
                        else
                        {
                            turn = true;
                        }
                    }
                    else
                    {
                        while (Encoding.ASCII.GetString(bytes).IndexOf("**") == -1)
                        {
                            sok.Receive(bytes);
                            op += Encoding.ASCII.GetString(bytes);
                        }
                        op = op.Split('*')[0];
                        if (op == "user")
                        {
                            MessageBox.Show("Hai vinto!");
                        }
                        else if (op == "draw!")
                        {
                            MessageBox.Show("Hai pareggiato");
                        }
                        else
                        {
                            MessageBox.Show("Hai perso...");
                        }
                        sok.Send(Encoding.ASCII.GetBytes("close**"));
                        sok.Shutdown(SocketShutdown.Both);
                        sok.Close();
                        Form f = new login();
                        f.Show();
                        this.Close();
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
                    label1.Text = "In attesa del server";
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
            this.Width = Resources.field.Width + 16;
            this.Height = Resources.field.Height + 39;
            this.MaximumSize = new Size(Resources.field.Width + 16, Resources.field.Height + 39);
            this.BackgroundImage = new Bitmap(Resources.field);
            this.BackgroundImageLayout = ImageLayout.None;
            button1.Visible = false;
            label1.Visible = false;
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
        void label1_Click(object sender, EventArgs e)
        {
            int i = 0;
            if (turn)
            {
                labelc((Label)sender);
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
        }
        private void labelc(Label sender)
        {
            int c = int.Parse(sender.Name.Split(';')[0]), r = int.Parse(sender.Name.Split(';')[1]);
            if(checksuicide(c, r))
            {
                if (sender.BackColor == Color.Transparent)
                {
                    sender.BackColor = Color.FromName(color);
                    sender.Enabled = false;
                    sok.Send(Encoding.ASCII.GetBytes(sender.Name.Split(';')[0] + sender.Name.Split(';')[1] + "**"));
                    turn = false;
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
                    if (lb[c + 1][r].BackColor==Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
                else if (r == 8)
                {
                    if (lb[c + 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (lb[c + 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
            }
            else if (c == 8)
            {
                if (r == 0)
                {
                    if (lb[c - 1][r].BackColor == Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
                else if (r == 8)
                {
                    if (lb[c - 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (lb[c - 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op))
                    {
                        ok = false;
                    }
                }
            }
            else
            {
                if (lb[c + 1][r].BackColor == Color.Transparent || lb[c - 1][r].BackColor == Color.Transparent)
                {
                    ok = true;
                }
                else
                {
                    if (lb[c][r - 1].BackColor == Color.Transparent || lb[c][r + 1].BackColor == Color.Transparent)
                    {
                        ok = true;
                    }
                    else
                    {
                        if (r == 0)
                        {
                            if (lb[c + 1][r].BackColor == Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op) && lb[c - 1][r].BackColor == Color.FromName(op))
                            {
                                ok = false;
                            }
                        }
                        else if (r == 8)
                        {
                            if (lb[c + 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op) && lb[c - 1][r].BackColor == Color.FromName(op))
                            {
                                ok = false;
                            }
                        }
                        else
                        {
                            if (lb[c + 1][r].BackColor == Color.FromName(op) && lb[c][r - 1].BackColor == Color.FromName(op) && lb[c][r + 1].BackColor == Color.FromName(op) && lb[c - 1][r].BackColor == Color.FromName(op))
                            {
                                ok = false;
                            }
                        }
                    }
                }
            }
            if (!ok)
            {
                turn = true;
            }
            return ok;
        }

        private void recive(string tab)
        {
            string name = "";
            if (tab == "end")
            {
                //sok.Send(Encoding.ASCII.GetBytes("ok**"));
                while (name.IndexOf("*") != -1)
                {
                    sok.Receive(bytes);
                    name += Encoding.ASCII.GetString(bytes);
                }
                name = name.Split('*')[0];
                if (name == "user")
                {
                    MessageBox.Show("Hai vinto!");
                }
                else if (name == "draw!")
                {
                    MessageBox.Show("Pareggio");
                }
                else
                {
                    MessageBox.Show("Hai perso...");
                }
                sok.Send(Encoding.ASCII.GetBytes("close**"));
                sok.Shutdown(SocketShutdown.Both);
                sok.Close();
                Form f = new login();
                f.Show();
                this.Close();
            }
            else
            {
                int k = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (tab.ToCharArray()[k] == '1')
                        {
                            lb[i][j].BackColor = Color.White;
                        }
                        else if(tab.ToCharArray()[k] == '2')
                        {
                            lb[i][j].BackColor = Color.Black;
                        }
                        else
                        {
                            lb[i][j].BackColor = Color.Transparent;
                        }
                        k++;
                    }
                }
            }
            turn = true;
            checkwin();
        }

        void checkwin()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (lb[i][j].BackColor == Color.Transparent)
                    {
                        end = false;
                    }
                }
            }
            if (end)
            {
                sok.Send(Encoding.ASCII.GetBytes("end**"));
            }
        }

        private void play_FormClosing(object sender, FormClosingEventArgs e)
        {
            t.Abort();
        }
    }
}
