using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;




namespace TcpServer
{
    class TcpProgram
    {
    private static Socket serverSocket;

    public static List<TcpClient> clients = new List<TcpClient>();

    static void Main(string[] args)
    {
        StartSeverYibu();
        Console.ReadKey();
    }

    private static void BindIPAndPort()
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Game.Const.IPandPoint.C_SeverIp), Game.Const.IPandPoint.C_SeverPoint));
        Console.WriteLine("到现在，服务器，已经启动");
        serverSocket.Listen(0);//这是设置最大连接数，0表示无限多个
    }

    static void StartSeverYibu()
    {
        BindIPAndPort();
        Console.WriteLine("TCP服务器开始接收");
        serverSocket.BeginAccept(AcceptCallBack, serverSocket);
    }

    static void AcceptCallBack(IAsyncResult ar)
    {
        var severSocket = ar.AsyncState as Socket;
        var clientSocket = severSocket.EndAccept(ar);

        var client = new TcpClient(clientSocket);
        clients.Add(client);

        Console.WriteLine("有人接到了我们服务器" + clientSocket.RemoteEndPoint);


        //接受消息
        client.ReceiveMessage();


        //回调
        severSocket.BeginAccept(AcceptCallBack, severSocket);
    }




}
}

