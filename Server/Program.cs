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

	    // array to craete clientarray
            Socket[] clientArray = new Socket[10];
            int count = 0;

	    // keep recording of random generated clients
            int[,] records = new int[100,100];

            while(true)
            {

            // ask whether to split or generate key
            String command = null;
            Console.WriteLine("split/combine?");
            command = Console.ReadLine();

            // ask number of key parts to combine
            Console.WriteLine("Required SuperNodes to combine: ");
            string requiredStr = Console.ReadLine();
            int required = int.Parse(requiredStr);

            // Put key to array

            if (String.Equals(command, "split"))
            {


                // connect client to receive key
                ClientSocket = ServerListener.Accept();
                
		// send "send" request to client to get the key
                byte[] request = new byte[1024];
                String message = "send";
                request = Encoding.Default.GetBytes(message);
                int size_1 = message.Length;
                ClientSocket.Send(request, 0,size_1, SocketFlags.None);

		// receive key from the client
                byte[] receivedKey = new byte[1024];
                int size2 = ClientSocket.Receive(receivedKey);
                ushort[] key = new ushort[receivedKey.Length / 2];
                // Buffer.BlockCopy(receivedKey, 0, key, 0, receivedKey.Length);
                Console.WriteLine("Key received from client");
		    
                // ask number of clients to share the key
                Console.WriteLine("Number of SuperNodes to share: ");
                string playerStr = Console.ReadLine();
                int players = int.Parse(playerStr);

                // Split key

                var splitted = SharesManager.SplitKey(key, players, required);

                Random randNum = new Random();
                List<int> randomList = new List<int>();

                int number = 0;

                // share splited key parts with clients

                for (int j = 0; j < splitted.Length; j++)
                {   
                    // generate random numbers to generate random clients     
                    do {
                        number = randNum.Next(splitted.Length);
                        
                    } while (randomList.Contains(number));
                    randomList.Add(number);
                    records[count,j] = number;
                    
	            // generate clients in random
                    ClientSocket = ServerListener.Accept();
                    int clientNum = records[count,j];
                    clientArray[clientNum] = ClientSocket;
                    Console.WriteLine(clientNum +"Clients connected");

		    // send key parts to random generated clients
                    byte[] msg = new byte[1024];
                    msg = Encoding.Default.GetBytes(splitted[j]);
                    int size = splitted[j].Length;
                    clientArray[clientNum ].Send(msg, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent key part to client ");

		    // send key numbers to each client
                    String numOfKey =  count.ToString();             
                    byte[] intBytes = BitConverter.GetBytes(count);
                    int size_2 = numOfKey.Length;
                    clientArray[clientNum].Send(intBytes, 0, size_2 , SocketFlags.None);
                    Console.WriteLine("Sent key number");;

                }

                //count for key number
                count++;
            }

            /*............................COMBINE KEY............................................. */

            
            if (String.Equals(command, "combine"))
            {

                Console.WriteLine("Number of shares to generate key: "+ required);
                Console.WriteLine("Number of key: ");
                string numOfKey = Console.ReadLine();
                int num = int.Parse(numOfKey);

		// to send "send" order to each clinets
                string order = null;
                Console.WriteLine("Enter your order");
                order = Console.ReadLine();

                var shares = new string[required];
            
                for (int i = 0; i < required; i++)
                {                
                    // send order to each clients
                    byte[] bytes = Encoding.ASCII.GetBytes(order);
                    int size = bytes.Length;
                    int clientNum = records[num,i];
                    clientArray[clientNum].Send(bytes, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent your order");

		    // send key number
                    byte[] intBytes = BitConverter.GetBytes(num);
                    int size_2 = numOfKey.Length;
                    clientArray[clientNum].Send(intBytes, 0, size_2 , SocketFlags.None);
                    Console.WriteLine("Sent key number");

		    // receive key parts from clients
                    byte[] msgFromCilent = new byte[1024];
                    int size2 = clientArray[clientNum].Receive(msgFromCilent);
                    shares[i] = System.Text.Encoding.ASCII.GetString(msgFromCilent, 0, size2);
                    Console.WriteLine("Msg received from client : " + shares[i]);

		    // generate key from key parts
                    if (i == required-1)
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
