using System.Net.Sockets;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;

class Program
{
    public static Dictionary<string, OneClient> ClientMap = new Dictionary<string, OneClient>();
    static void Main(string[] args)
    {
        SqlUtil.Init();
        //return;
        Console.WriteLine("Server started, waiting for clients...");
        int port = 10110;
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected");

            Thread clientThread = new Thread(() => new OneClient(client));
            clientThread.Start();
        }
    }


    


}
