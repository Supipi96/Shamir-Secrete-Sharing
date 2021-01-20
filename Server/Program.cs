using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;


namespace Server
{
	

    class Program
    {
        static void Main(string[] args)
        {
            int port = 13000;
            string IpAddress = "127.0.0.1";
            Socket ServerListener = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAddress),port);
            ServerListener.Bind(ep);
            ServerListener.Listen(100);
            Console.WriteLine("Server is Listening ...");
            Socket ClientSocket = default(Socket);
            Program p = new Program();

            Socket[] clientArray = new Socket[10];

            while(true)
            {
            String command = null;
            Console.WriteLine("split/combine?");
            command = Console.ReadLine();

            // Generate Key 

                int polynomsCount = 3;
                var byteKey = KeyGenerator.GenerateKey(polynomsCount * 16);
                var key = KeyGenerator.GenerateDoubleBytesKey(byteKey);
                Console.WriteLine("Key Generated");

            if (String.Equals(command, "split"))
            {

                Console.WriteLine("Number of SuperNodes: ");
                string playerStr = Console.ReadLine();

                Console.WriteLine("Required SuperNodes for sharing: ");
                string requiredStr = Console.ReadLine();

                int players = int.Parse(playerStr);
                int required = int.Parse(requiredStr);

                // Split key

                var splitted = SharesManager.SplitKey(key, players, required);
            
                // share splited key parts with clients

                

                for (int j = 0; j < splitted.Length; j++)
                {

                    int x = j+1;
                    ClientSocket = ServerListener.Accept();
                    clientArray[j] = ClientSocket;
                    Console.WriteLine(x+"Clients connected");
                    byte[] msg = new byte[1024];
                    msg = Encoding.Default.GetBytes(splitted[j]);
                    int size = splitted[j].Length;
                    clientArray[j].Send(msg, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent to client ");
                        

                }
            }

            /*............................COMBINE KEY............................................. */

            
            if (String.Equals(command, "combine"))
            {
                Console.WriteLine("Number of shares to generate key: ");
                string sharesCountStr = Console.ReadLine();
                int sharesCount = int.Parse(sharesCountStr);

                string order = null;
                Console.WriteLine("Enter your order");
                order = Console.ReadLine();

                var shares = new string[sharesCount];
            
                for (int i = 0; i < sharesCount; i++)
                {
                

                    byte[] bytes = Encoding.ASCII.GetBytes(order);
                    int size = bytes.Length;
                    clientArray[i].Send(bytes, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent your order");

                    byte[] msgFromCilent = new byte[1024];
                    int size2 = clientArray[i].Receive(msgFromCilent);
                    shares[i] = System.Text.Encoding.ASCII.GetString(msgFromCilent, 0, size2);
                    Console.WriteLine("Msg received from client : " + shares[i]);

                    if (i == sharesCount-1)
                    {
           
                        var generatedKey = SharesManager.CombineKey(shares);
            
                        Console.WriteLine("\nGenerated key:",generatedKey);
                        Console.WriteLine();

                    }
                    
                   
                }
            }

        }   

        } 

    }
}
