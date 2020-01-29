using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace SocketERTramas
{
    class Program
    {

        static void Main(string[] args)
        {
            Comunicacion01 con = new Comunicacion01("10.0.0.2", 2018, "10.0.0.3", 7060);

            Console.WriteLine("{0}->{1}", con.ConnectLocal, con.ConnectRemota);
            con.EnviarRecibirPOS();

            Console.ReadKey();
        }
    }
}