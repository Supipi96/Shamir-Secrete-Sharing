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

            // to keep record from one clients
            var msgArray = new string[totalSuperNOdes];
            
            // to keep key parts which are relevant to the key number
            String[] array = new string[100]; 
            
            // variable count to count number from msg from one client
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
                    // i is the key number in integer  
                    int i = BitConverter.ToInt32(MsgFromServer, 0);
                    
                    string v = msgArray[0];
                    // keep key part into array
                    array[i] = v;
                }
                

                // check whether the msg is send. If it is send then send tha key part back to the server
                if (msgArray[count] == "send")
                {
                    
                    byte[] keyNUM = new byte[1024];
                    int size_1 = ClientSocket.Receive(keyNUM);
                    msgArray[count] = System.Text.Encoding.ASCII.GetString(keyNUM, 0, size_1);
                    int j = BitConverter.ToInt32(keyNUM);
                    Console.WriteLine("Got from server " + msgArray[count]);

                    byte[] bytes = Encoding.ASCII.GetBytes(array[j]);
                    ClientSocket.Send(bytes);

                    //renew for new user
                    count = (-1);

                }
                
                count++;
                
            }
            
        }


    }
}
