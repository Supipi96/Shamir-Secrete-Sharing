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
            int count = 0;

            int[,] records = new int[100,100];

            while(true)
            {

            String command = null;
            Console.WriteLine("split/combine?");
            command = Console.ReadLine();

            Console.WriteLine("Required SuperNodes to combine: ");
            string requiredStr = Console.ReadLine();
            int required = int.Parse(requiredStr);

            // Put key to array

            if (String.Equals(command, "split"))
            {


                // Recieve key 
                ClientSocket = ServerListener.Accept();
                
                byte[] request = new byte[1024];
                String message = "send";
                request = Encoding.Default.GetBytes(message);
                int size_1 = message.Length;
                ClientSocket.Send(request, 0,size_1, SocketFlags.None);

                byte[] receivedKey = new byte[1024];
                int size2 = ClientSocket.Receive(receivedKey);
                ushort[] key = new ushort[receivedKey.Length / 2];
                // Buffer.BlockCopy(receivedKey, 0, key, 0, receivedKey.Length);
                Console.WriteLine("Key received from client");

               /* int polynomsCount = 3;
                var byteKey = KeyGenerator.GenerateKey(polynomsCount * 16);
                var key = KeyGenerator.GenerateDoubleBytesKey(byteKey);
                //byte[] target = new byte[key.Length * 2]; 
                Console.WriteLine("Key Generated");*/

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
                         
                    do {
                        number = randNum.Next(splitted.Length);
                        
                    } while (randomList.Contains(number));
                    randomList.Add(number);
                    records[count,j] = number;
                    
                    ClientSocket = ServerListener.Accept();
                    int clientNum = records[count,j];
                    clientArray[clientNum] = ClientSocket;
                    Console.WriteLine(clientNum +"Clients connected");

                    byte[] msg = new byte[1024];
                    msg = Encoding.Default.GetBytes(splitted[j]);
                    int size = splitted[j].Length;
                    clientArray[clientNum ].Send(msg, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent key part to client ");

                    String numOfKey =  count.ToString();             
                    byte[] intBytes = BitConverter.GetBytes(count);
                    int size_2 = numOfKey.Length;
                    clientArray[clientNum].Send(intBytes, 0, size_2 , SocketFlags.None);
                    Console.WriteLine("Sent key number");;

                }


                count++;
            }

            /*............................COMBINE KEY............................................. */

            
            if (String.Equals(command, "combine"))
            {

                Console.WriteLine("Number of shares to generate key: "+ required);
                Console.WriteLine("Number of key: ");
                string numOfKey = Console.ReadLine();
                int num = int.Parse(numOfKey);

                string order = null;
                Console.WriteLine("Enter your order");
                order = Console.ReadLine();

                var shares = new string[required];
            
                for (int i = 0; i < required; i++)
                {                

                    byte[] bytes = Encoding.ASCII.GetBytes(order);
                    int size = bytes.Length;
                    int clientNum = records[num,i];
                    clientArray[clientNum].Send(bytes, 0, size, SocketFlags.None);
                    Console.WriteLine("Sent your order");

                    byte[] intBytes = BitConverter.GetBytes(num);
                    int size_2 = numOfKey.Length;
                    clientArray[clientNum].Send(intBytes, 0, size_2 , SocketFlags.None);
                    Console.WriteLine("Sent key number");

                    byte[] msgFromCilent = new byte[1024];
                    int size2 = clientArray[clientNum].Receive(msgFromCilent);
                    shares[i] = System.Text.Encoding.ASCII.GetString(msgFromCilent, 0, size2);
                    Console.WriteLine("Msg received from client : " + shares[i]);

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
