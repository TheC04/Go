using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    internal class Program
    {

        public class global
        {
            public static string data = null;
            public bool on = true, match = false;
            public Thread[] tpool = new Thread[2];

            public void listen()
            {
                byte[] bytes = new Byte[1024];

                IPAddress ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
                IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 5000);

                Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    if (on)
                    {
                        listener.Bind(localEndPoint);
                        listener.Listen(10);
                    }

                    while (on)
                    {
                        Socket handler = listener.Accept();
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
                        //semaforo incrociato così quando arriva all'altro parte anche qui
                        //byte[] msg = Encoding.ASCII.GetBytes();
                        //handler.Send(msg);
                        handler.Shutdown(SocketShutdown.Both);
                        handler.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        static void Main(string[] args)
        {
            global g = new global();
            g.tpool[0] = new Thread(new ThreadStart(g.listen));
            g.tpool[1] = new Thread(new ThreadStart(g.listen));
            g.tpool[0].Start();
            g.tpool[1].Start();
            /*while (g.match)
            {

            }*/
        }
    }
}
