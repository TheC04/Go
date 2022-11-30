using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Go
{
    public partial class play : Form
    {
        Thread t;
        string me;
        bool turn = false;
        byte[] bytes = new byte[1024];
        byte[] msg;
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
        Socket sok = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Label[][] lb = new Label[9][];

        public play(string username, string opponent)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
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
            if (sender.BackColor == Color.Transparent)
            {
                roundlabel(path, sender);
                sender.BackColor = Color.FromName(me);
                sender.Enabled = false;
                send(sender.Name);
            }
            else
            {
                MessageBox.Show("Non puoi mettere qui la tua pedina");
            }   
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
        }
        private void send(string msg)
        {
            sok.Send(Encoding.ASCII.GetBytes(msg));
            recive();
        }
        private void recive()
        {
            while (sok.Receive(bytes).ToString().Length != 81)
            {
                sok.Receive(bytes);
            }
            int i = 0, j = 0;
            while(i != 8 && j != 8){
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
            t = new Thread(getC);
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
                    while(me != "white" && me != "black")
                    {
                        int i = 0;
                        try
                        {
                            me = bytes.ToString();
                            msg = Encoding.ASCII.GetBytes("ok**");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString(), "Errore di comunicazione con il server");
                            msg = Encoding.ASCII.GetBytes("a**");
                            i++;
                        }
                        if (i == 10)
                        {
                            MessageBox.Show("Non è stato possibile comunicare con il server");
                            break;
                        }
                        sok.Send(msg);
                    }
                    sok.Receive(bytes);
                    if (bytes.ToString() == "yt**")
                    {
                        lock (this)
                        {
                            turn = true;
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
