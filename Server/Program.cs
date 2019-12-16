using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {
        static private byte[] buffer = new byte[8192];
        static private Socket serverSocket;
        static private List<Socket> clientSockets = new List<Socket>(2);
        static void Main()
        {
            Console.WriteLine("Setting up server.");
            int port;
            using (WebClient wc = new WebClient())
            {
                port = Convert.ToInt32(wc.DownloadString("http://docred.ml/ip.txt").Split(':')[1]);
            }
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            serverSocket.Listen(2);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            while (true)
            {
                string data = Console.ReadLine();
                if (data == "close")
                {
                    foreach (Socket client in clientSockets)
                    {
                        client.Send(new byte[] { 9 });
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                    clientSockets.Clear();
                    serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
                }
                else if (data == "exit")
                {
                    foreach (Socket client in clientSockets)
                    {
                        client.Send(new byte[] { 9 });
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                    Environment.Exit(0);
                }
            }
        }
        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = serverSocket.EndAccept(ar);
            clientSockets.Add(socket);
            if (clientSockets.Count == 1)
            {
                Console.WriteLine("First client connected: {0}", socket.RemoteEndPoint);
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
            else
                Console.WriteLine("Second client connected: {0}", socket.RemoteEndPoint);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        private static void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            try
            {
                int received = socket.EndReceive(ar);
                byte[] data = new byte[received];
                Array.Copy(buffer, data, received);
                foreach (Socket client in clientSockets)
                    if (client.RemoteEndPoint != socket.RemoteEndPoint)
                        client.Send(data);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch
            {
                Console.WriteLine(socket.RemoteEndPoint + " has been disconnected.");
                clientSockets.Remove(socket);
                socket.Close();
                serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            }
        }
    }
}
