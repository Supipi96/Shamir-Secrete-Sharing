using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = 13000;
            string IpAddress = "127.0.0.1";
            Socket ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAddress), port);
            ClientSocket.Connect(ep);
            Console.WriteLine("Client is connected!");

            
            while(true)
            {
                // receive msg from server
                byte[] MsgFromServer = new byte[1024];
                int size = ClientSocket.Receive(MsgFromServer);
                String msg = System.Text.Encoding.ASCII.GetString(MsgFromServer, 0, size);
                
                // check whether the msg is "send". If it is send, create the key and send it to the server
                if(msg == "send")
                {
                    int polynomsCount = 3;
                    var byteKey = KeyGenerator.GenerateKey(polynomsCount * 16);
                    var key = KeyGenerator.GenerateDoubleBytesKey(byteKey);
                    byte[] target = new byte[key.Length * 2]; 
                    Console.WriteLine("Key Generated");
                    // Buffer.BlockCopy(key, 0, target, 0, key.Length * 2);
                    ClientSocket.Send(target);
                    Console.WriteLine("Key send");
                }
  
            }     
            
        }

    }
}
