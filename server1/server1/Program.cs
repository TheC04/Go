using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;

namespace server1
{
    internal class Program
    {

        public class slot
        {
            public int t, b, l, r, val;
            public slot()
            {
                t = b = l = r = val = 0;
            }
        }

        public class program
        {
            bool turn = false, end;
            byte[] bytes = new byte[1024];
            string data = "", user, server, status = "";
            static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);
            Socket sok = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Socket handler;
            slot[][] f = new slot[9][];

            public program()
            {
                createm();
            }

            void createm()
            {
                for (int i = 0; i < 9; i++)
                {
                    f[i] = new slot[9];
                }
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
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

            public void connect()
            {
                try
                {
                    
                    sok.Bind(localEndPoint);
                    sok.Listen(1);
                    handler = sok.Accept();
                    Console.WriteLine("Connection started");
                    while (data.IndexOf("**") == -1)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    data = data.Split('*')[0];
                    Console.WriteLine(data);
                    //
                    if(data=="bianco" || data == "white")
                    {
                        user = "white";
                        server = "black";
                    }
                    else
                    {
                        user = "black";
                        server = "white";
                    }
                    handler.Send(Encoding.ASCII.GetBytes("ok**"));
                    Array.Clear(bytes, 0, bytes.Length);
                    data = "";
                    while (data.IndexOf("**") == -1)
                    {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    }
                    Console.WriteLine(data);
                    //
                    if (server == "black")
                    {
                        turn = true;
                    }
                    match();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            void match()
            {
                while (!end)
                {
                    if (turn)
                    {
                        string s = chmove() + "**";
                        Console.WriteLine(s);
                        //
                        handler.Send(Encoding.ASCII.GetBytes(s));
                    }
                    else
                    {
                        Array.Clear(bytes, 0, bytes.Length);
                        data = "";
                        while (data.IndexOf("**") == -1)
                        {
                            int bytesRec = handler.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        }
                        data = data.Split('*')[0];
                        Console.WriteLine(data);
                        //
                        refresh(data, user);
                    }
                    turn = !turn;
                }
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
                Console.WriteLine("Connection closed");
                Thread.Sleep(1000);
                Console.Clear();
            }
            string chmove()
            {
                int col, row;
                bool ok = false;
                string s = "";
                Random c = new Random(), r = new Random();
                while (!ok)
                {
                    col = c.Next(0, 9);
                    row = r.Next(0, 9);
                    if (f[col][row].val == 0)
                    {
                        if (server == "black")
                        {
                            f[col][row].val = 2;
                        }
                        else
                        {
                            f[col][row].val = 1;
                        }
                        ok = true;
                        s = col.ToString() + row.ToString();
                    }
                }
                refresh(s, server);
                return s;
            }
            void refresh(string move, string sender)
            {
                int c, r, u;
                c = int.Parse(move.ElementAt(0).ToString());
                r = int.Parse(move.ElementAt(1).ToString());
                if (sender == "white")
                {
                    u = 1;
                }
                else
                {
                    u = 2;
                }
                f[c][r].val = u;
                if (c > 0)
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
                status = "";
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (f[i][j].t != 0 && f[i][j].b != 0 && f[i][j].l != 0 && f[i][j].r != 0)
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
                        status += f[i][j].val.ToString();
                    }
                }
                Console.WriteLine(status);
                if (checkwin())
                {
                    Console.WriteLine("Game ended");
                    end = true;
                }
                bool checkwin()
                {
                    int u1 = 0, u2 = 0;
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
                                if (f[i][j].val == 1)
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
                            if (server == "white")
                            {
                                handler.Send(Encoding.ASCII.GetBytes("server**"));
                            }
                            else
                            {
                                handler.Send(Encoding.ASCII.GetBytes("user**"));
                            }
                        }
                        else if (u1 < u2)
                        {
                            if (server == "white")
                            {
                                handler.Send(Encoding.ASCII.GetBytes("user**"));
                            }
                            else
                            {
                                handler.Send(Encoding.ASCII.GetBytes("server**"));
                            }
                        }
                        else
                        {
                            handler.Send(Encoding.ASCII.GetBytes("draw!**"));
                        }
                    }
                    return end;
                }
            }
        }

        static void Main(string[] args)
        {
            program p = new program();
            p.connect();
        }
    }
}
