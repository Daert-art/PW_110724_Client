using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PW_110724_AsyncClient
{
    class AsyncClient
    {
        private IPEndPoint endP;
        private Socket socket;

        public AsyncClient(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
        }

        public void StartClient()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(endP, new AsyncCallback(MyConnectCallbackFunction), socket);
        }

        private void MyConnectCallbackFunction(IAsyncResult ia)
        {
            Socket clientSocket = (Socket)ia.AsyncState;
            clientSocket.EndConnect(ia);
            if (clientSocket.Connected)
            {
                string clientMessage = "Hello, server!";
                byte[] clientMessageBytes = Encoding.ASCII.GetBytes(clientMessage);
                clientSocket.BeginSend(clientMessageBytes, 0, clientMessageBytes.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), clientSocket);
            }
        }

        private void MySendCallbackFunction(IAsyncResult ia)
        {
            Socket clientSocket = (Socket)ia.AsyncState;
            int bytesSent = clientSocket.EndSend(ia);

            byte[] buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MyReceiveCallbackFunction), buffer);
        }

        private void MyReceiveCallbackFunction(IAsyncResult ia)
        {
            byte[] buffer = (byte[])ia.AsyncState;
            int bytesRead = socket.EndReceive(ia);
            string serverMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} from {((IPEndPoint)socket.RemoteEndPoint).Address} а string is received: {serverMessage}");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AsyncClient client = new AsyncClient("127.0.0.1", 11000);
            client.StartClient();
            Console.ReadLine();
        }
    }
}
