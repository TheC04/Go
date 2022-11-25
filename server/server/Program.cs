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

namespace server
{
    internal class Program
    {

        public class tp
        {
            public Thread[] tpool = new Thread[2];
        }

        public class pawn
        {
            public int t, b, l, r, val = 0;
        }

        public class global
        {
            private static string data = null, t1, t2, u1, u2;
            private bool on = true, match = false, turn;
            byte[] bytes = new Byte[1024];
            tp threads = new tp();
            Socket sok;
            Socket handler;
            SemaphoreSlim semaphore = new SemaphoreSlim(0, 1);
            //matrice di pedine

            public void connect()
            {
                int i = 0;
                try
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
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    if (data.IndexOf("**") > -1)
                    {
                        break;
                    }
                }
                //perché arrivano byte?
                Console.WriteLine(bytes.ToString());
                if (Thread.CurrentThread == threads.tpool[0])
                {
                    if (semaphore.CurrentCount == 1)
                    {
                        Console.WriteLine("Thread 1 arrived");
                        color();
                        Console.WriteLine("Color chosen");
                        semaphore.Release();
                        while (bytes.ToString() != "ok**")
                        {
                            handler.Send(Encoding.ASCII.GetBytes(t2));
                            handler.Receive(bytes);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Thread 1 is waiting for an opponent");
                        semaphore.Wait();
                        while (bytes.ToString() != "ok**")
                        {
                            handler.Send(Encoding.ASCII.GetBytes(t1));
                            handler.Receive(bytes);
                        }
                    }
                    startmatch();
                }
                else if (Thread.CurrentThread == threads.tpool[1])
                {
                    if (semaphore.CurrentCount == 1)
                    {
                        Console.WriteLine("Thread 2 arrived");
                        color();
                        Console.WriteLine("Color chosen");
                        semaphore.Release();
                        while (bytes.ToString() != "ok**")
                        {
                            handler.Send(Encoding.ASCII.GetBytes(t2));
                            handler.Receive(bytes);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Thread 2 is waiting for an opponent");
                        semaphore.Wait();
                        while (bytes.ToString() != "ok**")
                        {
                            handler.Send(Encoding.ASCII.GetBytes(t1));
                            handler.Receive(bytes);
                        }
                    }
                    startmatch();
                }
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
                        t1 = "white**";
                        t2 = "black**";
                        ok = true;
                    }
                    else if (r.Next(0, 100) > 50)
                    {
                        t1 = "black**";
                        t2 = "white**";
                        ok = true;
                    }
                }
            }

            public void startmatch()
            {
                byte[] bytes = new Byte[1024];
                if (t1 == "white**")
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
                        refresh(bytes.ToString());
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
                        refresh(bytes.ToString());
                        semaphore.Release();
                    }
                    turn = !turn;
                }
            }
            private void refresh(string move)
            {
                //metto pedina e elimino eventuali mangiate
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
