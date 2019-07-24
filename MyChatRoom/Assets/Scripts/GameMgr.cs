using System.Collections;
using System.Collections.Generic;
using Game.Script;
using UnityEngine;
using Game;
using Game.Const;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Message = Game.Const.Message;

/// <summary>
/// 
/// 声明：
///     Game命名空间下的所有内容是我之前写好的一些基础的，可以复用的功能，与聊天室逻辑无关，所以没有太多注释
///     由于，我对于unity掌握不是足够深入，所以，我的程序，只有在unity编辑器中跑才可以发送和接受图片，这是由于unity发行后resources文件夹
///     不可访问的原因，与网络编程无关。老师不要误会
/// 
/// 
/// 
/// </summary>



public class GameMgr : MonoBehaviour
{
    void Awake()
    {

        print(Application.persistentDataPath);
        this.gameObject.AddComponent<MainLoop>();
        Game.Global.GlobalVar.Refresh();
    }

    private void Start()
    {
        
        SocketNetWork.StartClient();
    }
    private Sprite s;
    private void Update()
    {


        while(ReceiveUdpMessage.udpReveiveQueue.Count>0)
        {
            var data = ReceiveUdpMessage.udpReveiveQueue.Dequeue();
            var msg = MessageMgr.DecodeObject<MessageData>(data);
            switch(msg.msgType)
            {
                case MessageType.Text:
                    if(!string.IsNullOrEmpty(msg.sender))
                        CEventCenter.BroadMessage(Message.C_Chat, msg);
                    break;
                default:
                    break;
            }
        }


        while (ReceiveTcpMessage.tcpReveiveQueue.Count > 0)
        {
            //print("？？？");
            var data = ReceiveTcpMessage.tcpReveiveQueue.Dequeue();
            //print("收到消息总长度：" + data.Length);
            //FileStream f = File.Create(Application.dataPath + "/Resources/jb.png");
            ////BinaryWriter bw = new BinaryWriter(f);
            //f.Write(data, 0, data.Length);
            //f.Close();

            var msg = MessageMgr.DecodeObject<MessageData>(data);

            if (msg == null)
            {
                Debug.LogError("msg为空");
            }

            switch (msg.msgType)
            {
                case MessageType.Text:
                    if (!string.IsNullOrEmpty(msg.sender))
                        CEventCenter.BroadMessage(Message.C_Chat, msg);
                    break;
                case MessageType.File:
                    CEventCenter.BroadMessage(Message.C_File, msg);
                    break;
                case MessageType.Number:
                    CEventCenter.BroadMessage(Message.C_Number, msg);
                    break;
                case MessageType.Sound:
                    Debug.Log("客户端接收到!!!");
                    CEventCenter.BroadMessage(Message.C_Sound, msg);
                    break;
                default:
                    break;
            }
        }
    }
}
