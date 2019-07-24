using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
[System.Serializable]
public enum MessageType
{
    Text = 0,//聊天
    Login = 1,//登陆
    LogOut = 2,//登出
    File=3,
    Calc=4,
    Sound=5,
    Number,
    Merge

}
/// <summary>
/// 消息体
/// 这是自定义的消息类，服务器和客户端通信全靠传输这个类对象通信
/// </summary>
[System.Serializable]
public class MessageData
{
    public string sender;
    public MessageType msgType;
    public string msg;
    public string picName;
    public byte[] bytes;
    public int arg1;
    public int arg2;
    public int number;
    

    void Init(string sender,MessageType type)
    {
        this.sender = sender;
        this.msgType = type;
    }

    /// <summary>
    /// 因为，有些序列化方式要求只能使用无参构造函数，所以，这几个静态函数是用来构造的
    /// </summary>
    /// <param name="m"></param>
    /// <param name="number"></param>
    /// <param name="sender"></param>
    /// <returns></returns>
    public static MessageData Init(MessageData m,int number,string sender)
    {
        m.Init(sender, MessageType.Number);

        m.number = number;
        return m;
    }

    public static MessageData Init(string msg,MessageType type,string sender)
    {
        var m = new MessageData();
        m.Init(sender, type);
        m.msg = msg;
        return m;
    }

    public static MessageData Init(MessageData m,byte[] sound,string sender)
    {
        m.Init(sender, MessageType.Sound);
        m.bytes = sound;
        return m;
    }
    public static MessageData Init(MessageData m,int p1,int p2,string sender)
    {
        m.Init(sender, MessageType.Calc);
        m.arg1 = p1;
        m.arg2 = p2;
        return m;
    }

    public static MessageData Init(MessageData m,string str,string sender)
    {
        m.Init(sender, MessageType.Text);
        m.msg = str;
        return m;
    }

    public static MessageData Init(MessageData m, string picName,string sender,byte[] bytes)
    {
        m.Init(sender, MessageType.File);
        m.picName = picName;
        m.bytes = bytes;
        return m;
    }
    
}


//[System.Serializable]
//public class TextMessge:MessageData
//{
//    public string msg;
//}

//[System.Serializable]
//public class PictureMessage:MessageData
//{
//    public string picName;
//    public byte[] bytes;
//}
//[System.Serializable]
//public class CalculateMessage : MessageData
//{
//    public int arg1;
//    public int arg2;
//    public int GetAnswer()
//    {
//        return arg1 + arg2;
//    }
//}
//[System.Serializable]
//public class NumberMessage:MessageData
//{
//    public int number;
//}

//public class PictureMessage:MessageData, IXmlSerializable
//{

//    public byte[] bytes;

//    public new XmlSchema GetSchema()
//    {
//        return null;
//    }

//    public new void ReadXml(XmlReader reader)
//    {
//        var stringSer = new XmlSerializer(typeof(string));
//        var typeSer = new XmlSerializer(typeof(MessageType));

//        reader.Read();
//        reader.ReadStartElement("sender");
//        this.sender = stringSer.Deserialize(reader) as string;
//        reader.ReadEndElement();

//        reader.ReadStartElement("message");
//        this.msg = stringSer.Deserialize(reader) as string;
//        reader.ReadEndElement();

//        reader.ReadStartElement("type");
//        this.msgType = (MessageType)typeSer.Deserialize(reader);
//        reader.ReadEndElement();

//        reader.ReadStartElement("picture");
//        this.bytes = MessageMgr.Encode((string)stringSer.Deserialize(reader));
//        reader.ReadEndElement();

//        reader.ReadEndElement();
//    }

//    public new void WriteXml(XmlWriter writer)
//    {
//        var stringSer = new XmlSerializer(typeof(string));
//        var typeSer = new XmlSerializer(typeof(MessageType));

//        writer.WriteStartElement("sender");
//        stringSer.Serialize(writer, this.sender);
//        writer.WriteEndElement();

//        writer.WriteStartElement("message");
//        stringSer.Serialize(writer, this.msg);
//        writer.WriteEndElement();

//        writer.WriteStartElement("type");
//        typeSer.Serialize(writer, this.msgType);
//        writer.WriteEndElement();

//        writer.WriteStartElement("picture");
//        stringSer.Serialize(writer, MessageMgr.Decode(this.bytes));
//        writer.WriteEndElement();
//    }
//}
