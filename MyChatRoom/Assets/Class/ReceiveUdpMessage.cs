using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

class ReceiveUdpMessage:Game.Singleton<ReceiveUdpMessage>
{
    public static Queue<byte[]> udpReveiveQueue = new Queue<byte[]>();
    private EndPoint serverEnd;
    private Thread connectT;
    private byte[] sendData = new byte[1024];
    private byte[] receiveData = new byte[1024];

    public void BeginReceive()
    {
        MessageMgr.Q_SendText("Hello");
        connectT = new Thread(SocketReceiver);
        connectT.Start();
    }

    private void SocketReceiver()
    {
        EndPoint serverEnd = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            int length = SocketNetWork.udpClientSocket.ReceiveFrom(receiveData, ref serverEnd);
            Debug.Log("客户端收到消息长度：" + length);
            udpReveiveQueue.Enqueue(receiveData.Take(length).ToArray());
        }
    }

    
}
