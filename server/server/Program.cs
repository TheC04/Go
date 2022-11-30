using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection.Emit;

namespace server
{
    internal class Program
    {

        public class tp
        {
            public Thread[] tpool = new Thread[2];
        }
        public class user
        {
            public string username;
            public string color;
        }
        public class slot
        {
            public int t, b, l, r, val;
            public slot()
            {
                t = b = l = r = val = 0;
            }
        }

        public class global
        {
            int i = 0;
            private static string data = null, status;
            user[] u = new user[2];
            private bool on = true, match = false, turn;
            byte[] bytes = new Byte[1024];
            tp threads = new tp();
            Socket sok;
            Socket handler;
            SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
            slot[][] f = new slot[9][];

            public global()
            {
                createm();
            }

            public void connect()
            {
                try
                {
                    if (i < 2)
                    {
                        IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
                        sok = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        sok.Bind(localEndPoint);
                        sok.Listen(10);
                        handler = sok.Accept();
                        threads.tpool[i] = new Thread(new ThreadStart(setup));
                        threads.tpool[i].Start();
                        i++;
                    }
                    else
                    {
                        Console.WriteLine("Another client tried to connect, but there are no space left");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            private void setup()
            {
                data = null;
                while (true)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('*')[0];
                    if (data.IndexOf("**") > -1)
                    {
                        u[i].username = data;
                        Console.WriteLine(u[i]);
                        break;
                    }
                }
                handler.Send(Encoding.ASCII.GetBytes("ok**"));
                //current thread per i colori
                if (semaphore.CurrentCount == 1)
                {
                    Console.WriteLine("Thread  " + Thread.CurrentThread.ToString() + " arrived");
                    color();
                    Console.WriteLine("Color chosen");
                    semaphore.Release();
                    while (bytes.ToString() != "ok**")
                    {
                        if (Thread.CurrentThread == threads.tpool[0])
                        {
                            handler.Send(Encoding.ASCII.GetBytes(u[0].color));
                        }
                        else
                        {
                            handler.Send(Encoding.ASCII.GetBytes(u[1].color));
                        }
                        handler.Receive(bytes);
                    }
                }
                else
                {
                    Console.WriteLine("Thread " + Thread.CurrentThread.ToString() + " is waiting for an opponent");
                    semaphore.Wait();
                    while (bytes.ToString() != "ok**")
                    {
                        if (Thread.CurrentThread == threads.tpool[0])
                        {
                            handler.Send(Encoding.ASCII.GetBytes(u[0].color));
                        }
                        else
                        {
                            handler.Send(Encoding.ASCII.GetBytes(u[1].color));
                        }
                        handler.Receive(bytes);
                    }
                }
                startmatch();
                //byte[] msg = Encoding.ASCII.GetBytes();
                //handler.Send(msg);
                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();
            }
            public void color()
            {
                bool ok = false;
                while (!ok)
                {
                    Random r = new Random();
                    if (r.Next(0, 100) < 50)
                    {
                        u[1].color = "white**";
                        u[0].color = "black**";
                        ok = true;
                    }
                    else if (r.Next(0, 100) > 50)
                    {
                        u[1].color = "black**";
                        u[0].color = "white**";
                        ok = true;
                    }
                }
            }

            public void startmatch()
            {
                byte[] bytes = new Byte[1024];
                if (u[0].color == "white**")
                {
                    turn = true;
                }
                else
                {
                    turn = false;
                }
                while (match)
                {
                    if (Thread.CurrentThread == threads.tpool[0])
                    {
                        if (!turn)
                        {
                            semaphore.Wait();
                        }
                        Console.WriteLine("T1 turn");
                        handler.Send(Encoding.ASCII.GetBytes("yt**"));
                        handler.Receive(bytes);
                        Console.WriteLine("Move recived from T1: " + bytes.ToString());
                        refresh(bytes.ToString(), u[0].color);
                        semaphore.Release();
                    }
                    else if (Thread.CurrentThread == threads.tpool[1])
                    {
                        if (turn)
                        {
                            semaphore.Wait();
                        }
                        Console.WriteLine("T2 turn");
                        handler.Send(Encoding.ASCII.GetBytes("yt**"));
                        handler.Receive(bytes);
                        Console.WriteLine("Move recived from T2: " + bytes.ToString());
                        refresh(bytes.ToString(), u[1].color);
                        semaphore.Release();
                    }
                    handler.Send(Encoding.ASCII.GetBytes(status));
                    turn = !turn;
                }
            }
            private void refresh(string move, string sender)
            {
                int c, r, u;
                c = int.Parse(move.Split(';')[0]);
                r = int.Parse(move.Split(';')[1]);
                if (sender == "white**")
                {
                    u = 1;
                }
                else
                {
                    u = 2;
                }
                f[c][r].val = u;
                if (c > 0 )
                {
                    f[c - 1][r].r = u;
                }
                if (r > 0)
                {
                    f[c][r - 1].b = u;
                }
                if (c < 9)
                {
                    f[c + 1][r].l = u;
                }
                if (r < 9)
                {
                    f[c][r + 1].t = u;
                }
                for(int i = 0; i < 9; i++)
                {
                    for(int j = 0; j < 9; j++)
                    {
                        if (f[i][j].t!=0 && f[i][j].b != 0 && f[i][j].l != 0 && f[i][j].r != 0)
                        {
                            if (f[i][j].val != f[i][j].t && f[i][j].val != f[i][j].b && f[i][j].val != f[i][j].l && f[i][j].val != f[i][j].r)
                            {
                                f[i][j].val = 0;
                            }
                        }
                        status += f[i][j].ToString();
                    }
                }
                //metto pedina e elimino eventuali mangiate
            }

            void createm()
            {
                for (int i = 0; i < 9; i++)
                {
                    f[i] = new slot[9];
                }
                for(int i = 0; i < 9; i++)
                {
                    for(int j = 0; j < 9; j++)
                    {
                        f[i][j] = new slot();
                        if (i == 0)
                        {
                            f[i][j].l = -1;
                        }
                        if (i == 8)
                        {
                            f[i][j].r = -1;
                        }
                        if (j == 0)
                        {
                            f[i][j].t = -1;
                        }
                        if (j == 8)
                        {
                            f[i][j].b = -1;
                        }
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            tp threads = new tp();
            global g = new global();
            g.connect();
        }
    }
}
