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

            int totalSuperNOdes = 20;

            var msgArray = new string[totalSuperNOdes];
            int count = 0;

            while (true)
            {

                // client read from server
                byte[] MsgFromServer = new byte[1024];
                int size = ClientSocket.Receive(MsgFromServer);
                msgArray[count] = System.Text.Encoding.ASCII.GetString(MsgFromServer, 0, size);
                Console.WriteLine("Got from server " +  msgArray);
                

                // client send to server
                if (msgArray[1] == "send")
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(msgArray[0]);
                    ClientSocket.Send(bytes);

                }

                count++;
                
            }
            
        }


    }
}
