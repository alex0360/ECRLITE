﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SocketERTramas
{
    public class Comunicacion01
    {
        public Socket RemoteEndPoint { get; set; }

        public Socket _Socket { get; set; }

        public EndPoint ConnectRemota { get; set; }

        public EndPoint ConnectLocal { get; set; }

        private int Timeout = 30789;
        byte[] SentAndReceivedBytes = new byte[1024];

        #region Valores
        private string ENQ = ""; // ENQ-> Enquire-> 5 -> \u0005 -> 
        private string ACK = ""; // ACK-> Acknowledge-> 06 \u0006 -> 
        private string EOM = ""; // EM-> End of Medium-> 19 \u0019 -> 
        private string SYN = ""; // SYN-> Synchronous Idle-> 16 \u0016 -> 
        private string EOT = ""; // EOT-> End Of Transmission-> 04 \u0004 -> 
        private string CardConsultation => "CS00";
        private string[] frame = {
            // ventas Normal
            "CN00\u001c000000000118\u001c000000000018\u001c000000000000\u001c000002",
            "CN000000000001184000000000018000000000000000002"
        };

        #endregion
        public Comunicacion01(string ipLocal, int puertoLocal, string ipRemota, int puertoRemoto)
        {
            
            ConnectRemota = new IPEndPoint(IPAddress.Parse(ipRemota), puertoRemoto);
            ConnectLocal = new IPEndPoint(IPAddress.Parse(ipLocal), puertoLocal);
        }

        private string Enquire() {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);

                var buffer = System.Text.Encoding.Default.GetBytes(ENQ);

                Console.WriteLine("Enquire -> Envio {0}", ENQ);
                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);

                var recibedData = System.Text.Encoding.ASCII.GetString(buffer); // Se espera: ACK =  = \u0006
                Console.WriteLine("Enquire <- Recibi: {0}", recibedData.ToString());

                if (recibedData.Equals(ACK))
                    return ACK;
                else
                    return null;
            }
        }

        private string Synchronous(out byte[] buffer) {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);
                

                buffer = System.Text.Encoding.Default.GetBytes(SYN);

                Console.WriteLine("SYN -> Envio {0}", SYN);
                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);

                var recibedData = System.Text.Encoding.ASCII.GetString(buffer);
                Console.WriteLine("SYN <- Recibi: {0} ", recibedData.ToString());

                if (recibedData.Equals(EOM))
                    return EOM;
                else
                    return null;
            }
        }

        private string Acknowledge() {
            using (RemoteEndPoint = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                RemoteEndPoint.ReceiveTimeout = Timeout;
                RemoteEndPoint.SendTimeout = Timeout;
                RemoteEndPoint.Connect(ConnectRemota);

                var buffer = System.Text.Encoding.Default.GetBytes(ACK);

                Console.WriteLine("ACK -> Envio {0}", ACK);
                RemoteEndPoint.Send(buffer);
                RemoteEndPoint.Receive(buffer);

                var recibedData = System.Text.Encoding.ASCII.GetString(buffer); // Se espera: ACK =  = \u0006
                Console.WriteLine("EOT <- Recibi: {0}", recibedData.ToString());

                if (recibedData.Equals(ACK))
                    return ACK;
                else
                    return null;
            }

        }

        public bool EnviarRecibirPOS() {
            try {
                if (Enquire().Equals(ACK))
                {
                    if (Synchronous(out SentAndReceivedBytes).Equals(EOM))
                    {
                        
                        if (ECRPOS()) return true; else return false;
                    }
                    else return false;
                      
                }
                else return false;
            }catch(Exception ex)
            {
                Console.WriteLine("Error: {0}",ex);
                return false;
            }
        }

        private string trama() {

            return frame[0];
        }

        private bool POSECR(int paso = 0) {

            switch (paso)
            {
                case 1:

                default: return true;
            }
        }
        private bool ECRPOS() {
            string data = null;
            char dataChar;
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ConnectLocal.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(ConnectLocal);
                listener.Listen(5);
                listener.ReceiveTimeout = Timeout;
                listener.SendTimeout = Timeout;
                // Start listening for connections.  

                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                   

                    // An incoming connection needs to be processed.  
                    while (true)
                    {
                        //int bytesRec = handler.Receive(bytes);
                        handler.LingerState = new LingerOption(true, 10);
                        handler.Receive(SentAndReceivedBytes);
                        data += Encoding.ASCII.GetString(SentAndReceivedBytes);

                        Console.WriteLine("", data);

                        if (data.Equals(ENQ))
                        {
                            SentAndReceivedBytes = (new ASCIIEncoding()).GetBytes(trama());
                            handler.Send(SentAndReceivedBytes);
                            var port = ((IPEndPoint)handler.RemoteEndPoint).Port;

                            Console.WriteLine("envia Trama", Encoding.ASCII.GetString(SentAndReceivedBytes));
                            
                            handler.Receive(SentAndReceivedBytes);

                            dataChar = Convert.ToChar(SentAndReceivedBytes[0]);

                            Console.WriteLine("recibo ACK {0}", dataChar);
                            if (dataChar.ToString().Equals(ACK))
                            {
                                do
                                {
                                    Console.WriteLine("Muestra el Handler");
                                    if (handler.Connected)
                                    {
                                        SentAndReceivedBytes = new byte[1024];
                                        handler.Receive(SentAndReceivedBytes);

                                        dataChar = Convert.ToChar(SentAndReceivedBytes[0]);
                                        Console.WriteLine("EOT {0}", dataChar);
                                        if (!dataChar.ToString().Equals(EOT))
                                        {

                                            data = Encoding.ASCII.GetString(SentAndReceivedBytes).Replace("\0", "");


                                            Console.WriteLine("recibe Trama", data);
                                            SentAndReceivedBytes = (new ASCIIEncoding()).GetBytes(ACK.ToString());
                                            handler.Send(SentAndReceivedBytes);
                                        }
                                    }
                                } while (!dataChar.ToString().Equals(EOT));
                                Console.WriteLine("antes del Break: {0}", data);
                            }
                            handler.Send((new ASCIIEncoding()).GetBytes(ACK));
                            data = handler.Receive(SentAndReceivedBytes).ToString();
                        Console.WriteLine(data);
                        break;
                        }
                    };

                    // Show the data on the console.  
                    Console.WriteLine("Final del todo : {0}", data);

                    // Echo the data back to the client.  
                    // byte[] msg = Encoding.ASCII.GetBytes(data);

                    // handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                    return true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            finally { listener.Close(); }
                
        }
    }
}