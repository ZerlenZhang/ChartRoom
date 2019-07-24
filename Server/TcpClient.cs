using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TcpServer
{
    public class TcpClient
    {
        public Socket UserSocket { get; private set; }


        public static List<string> mergeStrs = new List<string>();

        private int msgCount = 0;
        private Queue<byte[]> reveiveQueue = new Queue<byte[]>();
        private List<byte[]> dataBufferList = new List<byte[]>();
        
        private const int DataBufferLength = 1024;
        private byte[] dataBuffer = new byte[DataBufferLength];
        private int startIndex = 0;
        public int RemainSize
        {
            get
            {
                return dataBuffer.Length - startIndex;
            }
        }
        public void ReceiveMessage()
        {
            if (RemainSize <= 0)
            {
                //为拆包进行缓存
                var data = new byte[DataBufferLength];
                dataBuffer.CopyTo(data, 0);
                dataBufferList.Add(data);
                UserSocket.BeginReceive(dataBuffer, 0, dataBuffer.Length, SocketFlags.None, ReceiveCallBack, null);
            }
            else
            {
                //一般情况
                UserSocket.BeginReceive(dataBuffer, startIndex, RemainSize, SocketFlags.None, ReceiveCallBack, null);
            }
        }



        private void ReceiveCallBack(IAsyncResult ar)
        {
            //获取接收到消息长度
            int msgLength = UserSocket.EndReceive(ar);
            Console.WriteLine("本次收到数据长度" + msgLength + "，剩余长度：" + RemainSize);
            if (msgLength == 0)
            {
                Console.WriteLine(UserSocket.RemoteEndPoint + "客户端请求离开");
                UserSocket.Close();
                TcpProgram.clients.Remove(this);
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
                        //说明这个大包接收完整
                        var data = dataBufferList[0].Clone() as byte[];
                        for (int i = 1; i < dataBufferList.Count; i++)
                        {
                            //拼接
                            data = data.Concat(dataBufferList[i]).ToArray();
                        }
                        data = data.Concat(dataBuffer).ToArray();
                        var msgdata = data.Skip(4).Take(msgCount).ToArray();

                        Console.WriteLine("解析完成后，共有" + msgdata.Length);

                        Array.Copy(data, msgCount + 4, dataBuffer, 0, data.Length - (4 + msgCount));


                        startIndex -= msgCount + 4;
                        msgCount = 0;
                        dataBufferList.Clear();
                        reveiveQueue.Enqueue(msgdata);

                    }
                    else
                    {
                        //大包还没有接收完整，继续接受
                        break;
                    }
                }
                else
                {
                    msgCount = BitConverter.ToInt32(dataBuffer, 0);
                    Console.WriteLine("消息总长度" + msgCount);
                    if (startIndex - 4 >= msgCount)
                    {
                        var data = dataBuffer.Skip(4).Take(msgCount).ToArray();
                        reveiveQueue.Enqueue(data);
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
            ReceiveMessage();


            Output();
        }


        private void Output()
        {
            while (reveiveQueue.Count > 0)
            {
                var data = reveiveQueue.Dequeue();
                var msg = MessageMgr.DecodeObject<MessageData>(data);
                switch (msg.msgType)
                {
                    case MessageType.Text:
                        BroadObject(msg);
                        break;
                    case MessageType.Login:
                        break;
                    case MessageType.File:
                        BroadObject(msg);
                        break;
                    case MessageType.LogOut:
                        break;
                    case MessageType.Calc:
                        MessageMgr.SendObject(UserSocket, MessageData.Init(new MessageData(), msg.arg1 + msg.arg2, "Server"));
                        break;
                    case MessageType.Sound:
                        BroadObject(msg);
                        break;
                    case MessageType.Merge:
                        mergeStrs.Add(msg.msg);
                        if (mergeStrs.Count == 2)
                        {
                            BroadObject(MessageData.Init(new MessageData(), mergeStrs[0]+mergeStrs[1], "Server"));
                            mergeStrs.Clear();

                        }
                        break;

                }
            }
        }





        void BroadObject<T>(T msg) where T : MessageData
        {
            foreach (var client in TcpProgram.clients)
            {
                MessageMgr.SendObject(client.UserSocket, msg);
            }
        }


        public TcpClient(Socket socket)
        {
            this.UserSocket = socket;
        }
    }
}

