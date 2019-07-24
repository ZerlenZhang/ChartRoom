using Game.Script;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Model
{
    /// <summary>
    /// 2D图片基类
    /// </summary>
    public abstract class AbstractSpriteObj
    {
        protected GameObject obj;

        protected AbstractSpriteObj(string path, Vector3 pos, Transform parent = null)
        {
            var res = Resources.Load<GameObject>(path);
            if (res == null)
            {
                throw new Exception("图片路径错误");
            }
            obj = GameObject.Instantiate(res, pos, Quaternion.identity, parent);
            if(obj==null)
            {
                throw new Exception("子弹生成失败");
            }
            MainLoop.Instance.AddUpdateFunc(Update);
        }

        protected virtual void Update()
        {

        }


        public virtual void Release()
        {
            MainLoop.Instance.RemoveUpdateFunc(Update);
        }

        protected void DestoryThis()
        {
            if (this.obj == null)
                return;
            Release();
            //throw new Exception("怎么来的？");
            GameObject.Destroy(this.obj);

        }
    }
}
