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

namespace Go
{
    public partial class wait : Form
    {
        private string username, op;
        private bool opponent = false;
        private Thread listener, waiter;

        public wait(string u)
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
                IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 5000);
                Socket send = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    send.Connect(remoteEP);
                    byte[] msg = Encoding.ASCII.GetBytes(Sender + "**");
                    int bytesSent = send.Send(msg);
                    while (bytes.ToString().Length > 0)
                    {
                        int bytesRec = send.Receive(bytes);
                    }
                    if (bytes.ToString() == "no**")
                    {
                        MessageBox.Show("Non è stato trovato alcun avversario");
                        send.Shutdown(SocketShutdown.Both);
                        send.Close();
                        Form f = new login();
                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        op = bytes.ToString().Split('*')[0];
                        MessageBox.Show("Stai per sfidare " + op);
                        opponent = true;
                        send.Shutdown(SocketShutdown.Both);
                        send.Close();
                        Form f = new play(username, op);
                        f.Show();
                        this.Hide();
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
    }
}
