using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Game.Serialization;
using System.Net;

/// <summary>
/// 这个类负责发送消息，序列化自定义类，文字转码
/// </summary>
public static class MessageMgr
{
    #region Udp

    public static void Q_SendText(string text)
    {
        MessageMgr.SendObject(SocketNetWork.udpClientSocket,
                        MessageData.Init(new MessageData(), text, ChatRoomPanel.Instance.UserName),
                        SocketNetWork.udpServerEnd);
    }
    public static void SendPureBytes(Socket fromWho,byte[] data,EndPoint towho)
    {
        fromWho.SendTo(data, towho);
    }

    public static void SendMessage(Socket fromWho,string msg,EndPoint toWho)
    {
        SendPureBytes(fromWho, Encode(msg), toWho);
    }

    public static void SendObject<T>(Socket fromWho,T arg,EndPoint toWho) where T : class
    {
        SendPureBytes(fromWho, JsonManager.SerializeObject(arg), toWho);
    }
    
    #endregion

    #region Tcp

    public static void SendPureBytes(Socket toWhom,byte[] data)
    {
        if (data == null || data.Length == 0)
            return;
        
        //将数组长度表示转换成字节数组
        byte[] lengthBytes = BitConverter.GetBytes(data.Length);
        //将表示消息长度的字节连通消息一起发送
        toWhom.Send(lengthBytes.Concat(data).ToArray());
    }

    #region 发送和接受自定义类
    public static void SendObject<T>(Socket toWhom,T obj)where T : class
    {
        SendPureBytes(toWhom,  JsonManager.SerializeObject(obj));
    }

    public static void SendObject<T>(T obj)where T:class
    {
        SendObject(SocketNetWork.tcpServerSocket, obj);
    }
    public static T DecodeObject<T>(byte[] data)where T:class
    {
        return JsonManager.DeserializeObject<T>(data);
    }

    #endregion

    #endregion

    #region 文字转码
    public static byte[] Encode(string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }
    public static string Decode(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }
    #endregion
}
