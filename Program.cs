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
            //Comunicacion01 con = new Comunicacion01("10.0.0.21", 2018, "10.0.0.133", 7060);
            // Comunicacion01 con = new Comunicacion01("192.168.1.100", 2018, "192.168.1.110", 7060);
           Comunicacion01 con = new Comunicacion01("192.168.137.1", 2018, "192.168.137.2", 7060);

            Console.WriteLine("{0}->{1}", con.ConnectLocal, con.ConnectRemota);
            con.EnviarRecibirPOS();

            Console.ReadKey();
        }
    }
}