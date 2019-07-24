using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpServer
{
    class UdpProgram
    {
        private static Socket serverSocket;
        const int DataBufferLength = 1024;
        public static List<EndPoint> clients = new List<EndPoint>();

        static void Main(string[] args)
        {
            StartSeverTongbu();
        }
        private static void BindIPAndPort()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(Game.Const.IPandPoint.C_SeverIp), Game.Const.IPandPoint.C_UdpPoint));
            Console.WriteLine("到现在，服务器，已经启动");
            //serverSocket.Listen(0);//这是设置最大连接数，0表示无限多个
        }

        static void StartSeverTongbu()
        {
            BindIPAndPort();
            Console.WriteLine("UDP服务器开始接收");
            SocketReceive();
        }



        private static void SocketReceive()
        {
            var receiveData = new byte[DataBufferLength];
            EndPoint clientEnd =new IPEndPoint(IPAddress.Any,0);
            while (true)
            {
                int length;
                try
                {

                    length = serverSocket.ReceiveFrom(receiveData, ref clientEnd);

                }
                catch
                {
                    continue;
                }
                if (clientEnd == null)
                {
                    Console.WriteLine("cilentEnd is null");
                    continue;
                }
                if (length == 0)
                {
                    Console.WriteLine(clientEnd + " 将要离开");
                    clients.Remove(clientEnd);
                }
                if (!clients.Contains(clientEnd))
                {
                    Console.WriteLine(clientEnd+" 加入我们");
                    clients.Add(clientEnd);
                }
                BroadMessage(receiveData.Take(length).ToArray());
            }
        }



        static void BroadMessage(byte[] msg)
        {
            foreach (var client in UdpProgram.clients)
            {

                MessageMgr.SendPureBytes(serverSocket, msg, client);
            }
        }

    }
}
