using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// 这个类负责接受服务端发来的消息，会自动解决分包，粘包问题
/// 所有的正确消息，都放在 reveiveQueue 这个public static 的队列中，任何地方都可以访问和处理
/// </summary>
class ReceiveTcpMessage : Game.Singleton<ReceiveTcpMessage>
{

    public static Queue<byte[]> tcpReveiveQueue = new Queue<byte[]>();

    //真实消息的长度
    private int msgCount = 0;
    //缓存大包的各个片段
    private List<byte[]> dataBufferList = new List<byte[]>();
    const int DataBufferLength = 1024;
    //缓存每次接收到的消息
    private byte[] dataBuffer = new byte[DataBufferLength];
    //起始索引
    private int startIndex = 0;
    //剩余长度
    public int RemainSize
    {
        get
        {
            return dataBuffer.Length - startIndex;
        }
    }
    

    public void ReceiveMessages(Socket userSocket)
    {
        if (RemainSize <= 0)
        {
            //为拆包进行缓存
            var data = new byte[DataBufferLength];
            dataBuffer.CopyTo(data, 0);
            dataBufferList.Add(data);
            userSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, userSocket);
        }
        else
        {
            //一般情况
            userSocket.BeginReceive(dataBuffer, startIndex, RemainSize, SocketFlags.None, ReceiveCallBack, userSocket);
        }
    }



    private void ReceiveCallBack(IAsyncResult ar)
    {
        var userSocket = ar.AsyncState as Socket;


        //获取接收到消息长度
        int msgLength = userSocket.EndReceive(ar);
        if (msgLength == 0)
        {
            Console.WriteLine(userSocket.RemoteEndPoint + "客户端请求离开");
            userSocket.Close();
            return;
        }

        //确保startIndex始终指向消息最后
        startIndex += msgLength;

        while (true)
        {
            if (startIndex <= 4)
            {
                //消息没有接受完，继续接受
                break;
            }
            if (msgCount != 0)
            {
                //这不是一个完整的包，而是一个大包中的一部分



                if (startIndex - 4 >= msgCount)
                {
                    //说明这个大包接收完整，拼接list中各个片段
                    var data = dataBufferList[0].Clone() as byte[];
                    for (int i = 1; i < dataBufferList.Count; i++)
                    {
                        //拼接
                        data = data.Concat(dataBufferList[i]).ToArray();
                    }
                    //接受本次获取到的信息
                    data = data.Concat(dataBuffer).ToArray();
                    var msgdata = data.Skip(4).Take(msgCount).ToArray();
                    UnityEngine.Debug.Log("客户端解析：共有：" + msgdata.Length);

                    //把最后接受到的消息的后半部分移到前面去
                    Array.Copy(data, msgCount + 4, dataBuffer, 0, data.Length - (4 + msgCount));

                    //清除处理
                    startIndex -= msgCount + 4;
                    msgCount = 0;
                    dataBufferList.Clear();
                    tcpReveiveQueue.Enqueue(msgdata);

                }
                else
                {
                    //大包还没有接收完整，继续接受
                    break;
                }
            }
            else
            {
                //每次发送消息，前4位是一个int，保存有效信息长度
                msgCount = BitConverter.ToInt32(dataBuffer, 0);
                UnityEngine.Debug.Log("客户端发送的数量：" + msgCount);
                if (startIndex - 4 >= msgCount)
                {
                    var data = dataBuffer.Skip(4).Take(msgCount).ToArray();
                    tcpReveiveQueue.Enqueue(data);
                    Array.Copy(dataBuffer, msgCount + 4, dataBuffer, 0, startIndex - (4 + msgCount));
                    startIndex -= (msgCount + 4);
                    msgCount = 0;
                }
                else
                {
                    //消息没有接受完，继续接受
                    break;
                }
            }
        }
        //回调
        ReceiveMessages(userSocket);
    }


}