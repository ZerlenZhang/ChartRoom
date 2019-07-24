using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
namespace Game.Global
{
    public static class GlobalVar
    {
        //public static TcpClient G_CurruentClient;
        public static GameObject G_Canvas;

        public static void Refresh()
        {
            G_Canvas = GameObject.Find("Canvas");
        }
    }
}

namespace Game.Const
{
    public static class Message
    {
        public static readonly string C_Chat = "gt";
        public static readonly string C_File = "gp";
        public static readonly string C_Number = "n";
        public static readonly string C_Sound = "sb";
        public static readonly string C_DD = "udp";
    }

    public static class DirPath
    {
        public static readonly string filePath = Application.persistentDataPath + "/Files";
    }

    public static class Number
    {
        public static readonly int DataBufferLength = 1024;
    }



    public static class IPandPoint
    {
        public static readonly string C_SeverIp = "127.0.0.1";
        public static readonly int C_SeverPoint = 5566;
        public static readonly int C_UdpPoint = 5567;
    }
}
