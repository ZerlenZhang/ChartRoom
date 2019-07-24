using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class SocketNetWork
{
    public static Socket tcpServerSocket;
    public static IPEndPoint udpServerEnd;
    public static Socket udpClientSocket;

    public static void StartClient()
    {
        try
        {
            tcpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpServerSocket.Connect(new IPEndPoint(IPAddress.Parse(Game.Const.IPandPoint.C_SeverIp), Game.Const.IPandPoint.C_SeverPoint));
            ReceiveTcpMessage.Instance.ReceiveMessages(tcpServerSocket);


            udpClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpServerEnd = new IPEndPoint(IPAddress.Parse(Game.Const.IPandPoint.C_SeverIp), Game.Const.IPandPoint.C_UdpPoint);
            ReceiveUdpMessage.Instance.BeginReceive();
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);   
        }
    }

}
