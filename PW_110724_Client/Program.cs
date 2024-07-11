using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PW_110724_Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint ep = new IPEndPoint(ip, 11000);
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                s.Connect(ep);
                if (s.Connected)
                {
                    string clientMessage = "Hello, server!";
                    s.Send(Encoding.ASCII.GetBytes(clientMessage));

                    byte[] buffer = new byte[1024];
                    int receivedBytes = s.Receive(buffer);
                    string serverMessage = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                    Console.WriteLine($"{DateTime.Now.ToLongTimeString()} from {((IPEndPoint)s.RemoteEndPoint).Address} а string is received: {serverMessage}");
                }
                else
                    Console.WriteLine("Error");
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
    }
}
