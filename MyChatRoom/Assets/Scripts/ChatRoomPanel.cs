using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using Game;
using Game.Global;
using Game.Script;
using UnityEngine.EventSystems;
using System.IO;
using Common;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;
using System;
using Classes;
using UI = UnityEngine.UI;
public class ChatRoomPanel : MonoSingleton<ChatRoomPanel>
{
    public string UserName
    {
        get
        {
            return userName.GetComponent<UnityEngine.UI.Text>().text;
        }
    }

    public UI.Toggle toggle;

    //以下GameObject获取按钮，界面拖拽赋值

    public GameObject userName;

    public UnityEngine.UI.Text text;

    public Sprite sprite;

    public GameObject content;

    public GameObject fileBtn;

    [SerializeField]
    public GameObject soundObject;

    [SerializeField]
    public GameObject soundPanel;

    public GameObject fileBubble;

    public GameObject pictureButton;
    
    public GameObject testMessageobj;

    public GameObject spriteMessageObj;
    private AudioSource ass;
    // Start is called before the first frame update
    void Start()
    {
        AddListener();
        ass=this.gameObject.AddComponent<AudioSource>();
        var inp = pictureButton.AddComponent<UIInputer>();
        inp.eventOnPointerClick += ToSendFile;
        var sound = soundObject.AddComponent<UIInputer>();
        sound.eventOnPointerDown += OnBeginTalk;
        sound.eventOnPointerUp += OnEndTalk;

    }


    //添加事件监听
    public void AddListener()
    {
        CEventCenter.AddListener<MessageData>(Game.Const.Message.C_Chat, OnChat);
        CEventCenter.AddListener<MessageData>(Game.Const.Message.C_File , OnPicutre);
        CEventCenter.AddListener<MessageData>(Game.Const.Message.C_Number, OnCalculate);
        CEventCenter.AddListener<MessageData>(Game.Const.Message.C_Sound, OnSound);
        CEventCenter.AddListener<string>(Game.Const.Message.C_DD, OnChat);
    }

    #region  声音

    #region 发送录音

    void OnBeginTalk(PointerEventData f)
    {
        MicrophoneMgr.TryStartRecording();
    }

    void OnEndTalk(PointerEventData f)
    {
        AudioClip ap;
        int length;
        MicrophoneMgr.EndRecording(out length, out ap);

        MessageMgr.SendObject(MessageData.Init(new MessageData(), WavUtility.FromAudioClip(ap), this.UserName));

    }

    #endregion
    
    #region 接受录音

    void OnSound(MessageData data)
    {
        Debug.Log("ui 接收到");
        var obj=Instantiate(soundPanel, content.transform);
        obj.transform.Find("Image_Name/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = this.UserName;
      
        ass.clip=WavUtility.ToAudioClip(data.bytes);
        ass.Play();
    }

    #endregion

    #endregion


    #region 文字

    #region 发送文字
    //回车键触发这个函数
    public void ToSendText(string text)
    {
        text.Trim();
        if(this.toggle.isOn)
        {
            if (text.Length > 0)
            {
                var strs = text.Split(' ');
                if (strs.Length != 2 || int.Parse(strs[1]) == null)
                {
                    //层层封装后，服务器和客户端之间发送消息，就可以这样简单
                    MessageMgr.Q_SendText(text);
                    //MessageMgr.SendObject(MessageData.Init(new MessageData(), text, UserName));

                }
                else
                {
                    MessageMgr.SendMessage(SocketNetWork.udpClientSocket, text, SocketNetWork.udpServerEnd);
                    //MessageMgr.SendObject(MessageData.Init(new MessageData(), Convert.ToInt32(strs[0]), Convert.ToInt32(strs[1]), UserName));

                }
            }
        }
        else
        {
            //MessageMgr.SendMessage(SocketNetWork.udpClientSocket, text, SocketNetWork.udpServerEnd);
            MessageMgr.SendObject(SocketNetWork.tcpServerSocket, MessageData.Init(text, MessageType.Merge, UserName));
        }
    }



    #endregion

    #region 接受文字

    //接收到聊天文字消息时调用
    void OnChat(MessageData msg)
    {
        var panel = Instantiate(testMessageobj, content.transform);
        panel.transform.Find("Image1/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = msg.sender;
        panel.transform.Find("Image2/Text_Content").GetComponent<UnityEngine.UI.Text>().text = msg.msg;
    }

    void OnChat(string msg)
    {
        var panel = Instantiate(testMessageobj, content.transform);
        panel.transform.Find("Image1/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = "";
        panel.transform.Find("Image2/Text_Content").GetComponent<UnityEngine.UI.Text>().text = msg;
    }

    #endregion

    #endregion

    #region 接受图片
    //接受图片时调用这个
    void OnPicutre(MessageData pic)
    {
        if(pic.picName.EndsWith(".png")||pic.picName.EndsWith(".jpg")||pic.picName.EndsWith(".bmp"))
        {
            //图片！


            MemoryStream fs = new MemoryStream();
            fs.Write(pic.bytes, 0, pic.bytes.Length);
            fs.Position = 0;
            //从内存中再次将数据读出来并保存为jpg图片
            //注意 这里 无法保存在C盘
            Image Img = Image.FromStream(fs);
            Img.Save(Application.dataPath + "/Resources/" + pic.picName, ImageFormat.Png);
            //Texture2D t = new Texture2D(Img.Width, Img.Height);
            //t.LoadImage(pic.bytes);
            //t.Apply();

            print(1);
            //UnityEditor.AssetDatabase.Refresh();
            var name = Path.GetFileNameWithoutExtension(Application.dataPath + "/Resources/" + pic.picName);
        
            print(2);
            print(name);
            var t = (Texture2D) Resources.Load(name);
            if(t==null)
            {
                Debug.LogError("我操了，Texure2D为null");
            }
            var sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            if(sprite==null)
            {
                Debug.Log("怎么图片还没有？？");
            }
            var panel = Instantiate(spriteMessageObj, content.transform);
            print(3);
            panel.transform.Find("Image1/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = pic.sender;
            panel.transform.Find("Image_Picture").GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            print(4);
        }

        else//普通文件
        {
            if (!Directory.Exists(Game.Const.DirPath.filePath))
                Directory.CreateDirectory(Game.Const.DirPath.filePath);

            var s=File.Create(Game.Const.DirPath.filePath + "/" + pic.picName);
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write(pic.bytes);
            bw.Close();
            s.Close();
            var panel = Instantiate(fileBubble,content.transform);
            print(3);
            panel.transform.Find("Image1/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = pic.sender;
            panel.transform.Find("Image_FileBk/Text_FileName").GetComponent<UnityEngine.UI.Text>().text = pic.picName;
            print(4);

        }
    }

    #endregion


    #region 计算

    //接收到计算结果类型消息时调用这个
    void OnCalculate(MessageData msg)
    {
        var panel = Instantiate(testMessageobj, content.transform);
        panel.transform.Find("Image_Avator").GetComponent<UnityEngine.UI.Image>().sprite = this.sprite;
        panel.transform.Find("Image1/Image/Text_UserName").GetComponent<UnityEngine.UI.Text>().text = msg.sender;
        panel.transform.Find("Image2/Text_Content").GetComponent<UnityEngine.UI.Text>().text = "计算结果：" + msg.number.ToString();
    }
    #endregion

    #region 发送文件

    //打开文件对话框
    public string OpenProject()
    {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = System.Runtime.InteropServices.Marshal.SizeOf(pth);
        pth.filter = "*.*";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath;  // default path  
        pth.title = "打开项目";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (OpenFileDialog.GetOpenFileName(pth))
        {
            string filepath = pth.file;//选择的文件路径;  
            return filepath;
        }
        return "";
    }

    //发送图片时调用
    void ToSendFile(PointerEventData data)
    {
        var path = OpenProject();
        print("获取路径为"+ path);
        if (string.IsNullOrEmpty(path))
            return;

        
        FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);


        var index=fs.Name.LastIndexOf('\\');
        var str = new string(fs.Name.ToCharArray(), index+1, fs.Name.Length - index-1);
        print("文件名" + str);


        BinaryReader strread = new BinaryReader(fs);
        byte[] byt = new byte[fs.Length];
        Debug.Log("本次发送数据长度：" + byt.Length);
        strread.Read(byt, 0, byt.Length - 1);

        //MessageMgr.SendMessage(SocketNetWork.socket, byt);
        
        
        MessageMgr.SendObject(MessageData.Init(new MessageData(),str, UserName, byt));
        
        strread.Close();

    }

    #endregion

}
