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

        public class user
        {
            public string username;
            public string color;

            public user()
            {
                username = "";
                color = "";
            }
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
            string status, winner;
            user[] u = new user[2];
            bool end = false, turn;
            byte[] bytes = new Byte[1024];
            Socket sok;
            Socket handler;
            slot[][] f = new slot[9][];

            public global()
            {
                createm();
                for(int i = 0; i < 2 ; i++)
                {
                    u[i] = new user();
                }
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
                        lock ((object)i)
                        {
                            Thread t = new Thread(new ThreadStart(setup));
                            t.Name = "T" + i.ToString();
                            t.Start();
                            Console.WriteLine(t.Name + " started");
                            i++;
                        }
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
                bool ok = false;
                string data = "";
                while (data.IndexOf("**") == -1)
                {
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('*')[0];
                    lock ((object)i)
                    {
                        u[i].username = data;
                        Console.WriteLine(u[i].username);
                    }
                }
                handler.Send(Encoding.ASCII.GetBytes("ok**"));
                if (i == 1)
                {
                    Console.WriteLine("Thread  " + Thread.CurrentThread.Name + " arrived");
                    if (Thread.CurrentThread.Name == "T0")
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[1].username));
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[0].username));
                    }
                    while (data.IndexOf("**") == -1)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('*')[0];
                        if (data == "ok")
                        {
                            break;
                        }
                    }
                    color();
                    Console.WriteLine("Color chosen");
                    ok = true;
                    if (Thread.CurrentThread.Name == "T0")
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[0].color));
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[1].color));
                    }
                    while (data.IndexOf("**") == -1)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('*')[0];
                        if (data == "ok")
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Thread " + Thread.CurrentThread.Name + " is waiting for an opponent...");
                    while (!ok)
                    {
                        Thread.Sleep(1000);
                    }
                    if (Thread.CurrentThread.Name == "T0")
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[0].color));
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes(u[1].color));
                    }
                    while (data.IndexOf("**") == -1)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec).Split('*')[0];
                        if (data == "ok")
                        {
                            break;
                        }
                    }
                }
                match();
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

            public void match()
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
                while (true)
                {
                    if (Thread.CurrentThread.Name == "T0")
                    {
                        while (!turn)
                        {
                            Thread.Sleep(500);
                        }
                        Console.WriteLine("T1 turn");
                        handler.Send(Encoding.ASCII.GetBytes("yt**"));
                        handler.Receive(bytes);
                        Console.WriteLine("Move recived from T1: " + bytes.ToString());
                        refresh(bytes.ToString(), u[0].color);
                        turn = !turn;
                    }
                    if (end)
                    {
                        handler.Send(Encoding.ASCII.GetBytes("end**"));
                        break;
                    }
                    else
                    {
                        if (Thread.CurrentThread.Name == "T1")
                        {
                            while (turn)
                            {
                                Thread.Sleep(500);
                            }
                            Console.WriteLine("T2 turn");
                            handler.Send(Encoding.ASCII.GetBytes("yt**"));
                            handler.Receive(bytes);
                            Console.WriteLine("Move recived from T2: " + bytes.ToString());
                            refresh(bytes.ToString(), u[1].color);
                            turn = !turn;
                        }
                    }
                    if (end)
                    {
                        handler.Send(Encoding.ASCII.GetBytes("end**"));
                        break;
                    }
                    else
                    {
                        handler.Send(Encoding.ASCII.GetBytes(status));
                    }
                }
                handler.Send(Encoding.ASCII.GetBytes(winner + "**"));
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
                                if (f[i][j].l != -1)
                                {
                                    f[i][j].val = f[i][j].l;
                                }
                                else
                                {
                                    f[i][j].val = f[i][j].r;
                                }
                            }
                        }
                        status += f[i][j].ToString();
                    }
                }
                if (checkwin())
                {
                    Console.WriteLine("Game ended");
                    end = true;
                }
            }
            bool checkwin()
            {
                int u1 = 0, u2 = 0;
                bool end = true;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (f[i][j].val == 0)
                        {
                            end = false;
                        }
                        else
                        {
                            if(f[i][j].val == 1)
                            {
                                u1++;
                            }
                            else
                            {
                                u2++;
                            }
                        }
                    }
                }
                if (end)
                {
                    if (u1 > u2)
                    {
                        if (u[0].color == "white**")
                        {
                            winner = u[0].username;
                        }
                        else
                        {
                            winner = u[1].username;
                        }
                    }
                    else if (u1 < u2)
                    {
                        if (u[0].color == "black**")
                        {
                            winner = u[0].username;
                        }
                        else
                        {
                            winner = u[1].username;
                        }
                    }
                    else
                    {
                        winner = "draw!";
                    }
                }
                return end;
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
            global g = new global();
            g.connect();
        }
    }
}
