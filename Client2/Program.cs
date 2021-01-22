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
            String[] array = new string[100]; 
            int count = 0;

                
            while(true)
            {
            
                // client read from server
                byte[] MsgFromServer = new byte[1024];
                int size = ClientSocket.Receive(MsgFromServer);
                msgArray[count] = System.Text.Encoding.ASCII.GetString(MsgFromServer, 0, size);
                Console.WriteLine("Got from server " +  msgArray[count]);

                if(count == 1)
                {
                      
                    int i = BitConverter.ToInt32(MsgFromServer, 0);
                    string v = msgArray[0];
                    array[i] = v;
                }
                

                // client send to server
                if (msgArray[count] == "send")
                {
                    
                    byte[] keyNUM = new byte[1024];
                    int size_1 = ClientSocket.Receive(keyNUM);
                    msgArray[count] = System.Text.Encoding.ASCII.GetString(keyNUM, 0, size_1);
                    int j = BitConverter.ToInt32(keyNUM);
                    Console.WriteLine("Got from server " + msgArray[count]);

                    byte[] bytes = Encoding.ASCII.GetBytes(array[j]);
                    ClientSocket.Send(bytes);

                    count = (-1);

                }
                
                count++;
                
            }
            
        }


    }
}
