using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Script
{
    public class AudioMgr : MonoSingleton<AudioMgr>
    {
        private Dictionary<string, AudioClip> audioclips = new Dictionary<string, AudioClip>();
        private AudioSource audioBg;
        private AudioSource audioEffect;

        public float BgSound
        {
            get { return audioBg.volume; }
            set { audioBg.volume = value; }
        }

        public float EffectVolume
        {
            get { return audioEffect.volume; }
            set { audioEffect.volume = value; }
        }

        protected override void Awake()
        {
            base.Awake();
            //实现从文件夹读取所有文件
            DirectoryInfo source = new DirectoryInfo(Application.dataPath + "//Resources/Audio");
            foreach (FileInfo diSourceSubDir in source.GetFiles())
            {
                if (diSourceSubDir.Name.EndsWith(".meta"))
                    continue;
                var strs = diSourceSubDir.Name.Split('.');
                var res = Resources.Load<AudioClip>("Audio/" + strs[0]);
                if (res == null)
                    Debug.Log("资源加载失败");
                audioclips.Add(strs[0], res);
            }


            //加载组件
            this.audioBg = this.gameObject.AddComponent<AudioSource>();
            this.audioBg.loop = true;
            this.audioBg.playOnAwake = true;

            this.audioEffect = this.gameObject.AddComponent<AudioSource>();
            this.audioBg.loop = false;
            this.audioBg.playOnAwake = false;



        }

        void Start()
        {
            //PlayAuBg("Bg");
            //this.audioBg.volume = 0f;
        }



        public void PlayAuBg(string name, float delayTime = 0)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            this.audioBg.clip = audioclips[name];
            this.audioBg.PlayDelayed(delayTime);
        }

        public void PlayAuEffect(string name, float delayTime = 0)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            this.audioEffect.clip = audioclips[name];
            this.audioEffect.PlayDelayed(delayTime);
        }

        public void PlayAuEffect(string name, Vector3 pos)
        {
            if (!audioclips.ContainsKey(name))
                throw new Exception("没有这个音频文件");
            AudioSource.PlayClipAtPoint(this.audioclips[name], pos);
        }
    }
}
