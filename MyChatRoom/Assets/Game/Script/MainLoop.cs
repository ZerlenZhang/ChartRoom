using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Game.Script
{
    /// <summary>
    /// 主循环
    /// 在外部调用，实现脱离MonoBehavior进行更新
    /// 封装多种协程便于调用
    /// </summary>
	public class MainLoop : MonoSingleton<MainLoop>
    {

        #region 协程（延时调用，间隔调用，一段时间内每帧调用）

        /// <summary>
        /// 开始关闭协程
        /// </summary>
        /// <param name="Coroutine"></param>
        /// <returns></returns>
        public Coroutine StartCoroutines(IEnumerator Coroutine)
        {
            return StartCoroutine(Coroutine);
        }

        public void StopCoroutines(Coroutine Coroutine)
        {
            StopCoroutine(Coroutine);
        }


        /// <summary>
        /// 运行直到为真
        /// </summary>
        /// <param name="method"></param>
        /// <param name="endCall"></param>
        /// <returns></returns>
        public Coroutine ExecuteUntilTrue(Func<bool> method, Action endCall = null)
        {
            return StartCoroutine(_ExecuteUntilTrue(method, endCall));
        }



        /// <summary>
        /// 延时调用
        /// </summary>
        /// <param name="method"></param>          c
        /// <param name="seconds"></param>
        public Coroutine ExecuteLater(Action method, float seconds)
        {
            return StartCoroutine(_ExecuteLater(method, seconds));
        }

        public Coroutine ExecuteLater<T>(Action<T> method, float seconds, T args)
        {
            return StartCoroutine(_ExecuteLater_T<T>(method, seconds, args));
        }

        /// <summary>
        /// 间隔调用
        /// </summary>
        /// <param name="method"></param>
        /// <param name="times"></param>
        /// <param name="duringTime"></param>
        public Coroutine ExecuteEverySeconds(Action method, float times, float duringTime)
        {
            return StartCoroutine(_ExecuteSeconds(method, times, duringTime));
        }

        public Coroutine ExecuteEverySeconds<T>(Action<T> method, float times, float duringTime, T args)
        {
            return StartCoroutine(_ExecuteSeconds_T(method, times, duringTime, args));
        }

        public Coroutine ExecuteEverySeconds(Action method, float times, float duringTime, Action endCall)
        {
            return StartCoroutine(_ExecuteSeconds_Action(method, times, duringTime, endCall));
        }

        public Coroutine ExecuteEverySeconds<T>(Action<T> method, float times, float duringTime, T args,
            Action<T> endCall)
        {
            return StartCoroutine(_ExecuteSeconds_Action_T(method, times, duringTime, args, endCall));
        }

        /// <summary>
        /// 一段时间内每帧调用
        /// </summary>
        /// <param name="method"></param>
        /// <param name="seconds"></param>
        public Coroutine UpdateForSeconds(Action method, float seconds, Action endCall)
        {
            return StartCoroutine(_UpdateForSeconds_Action(method, seconds, endCall));
        }

        public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
        {
            return StartCoroutine(_UpdateForSeconds_Action_T(method, seconds, arg, endCall));
        }

        public Coroutine UpdateForSeconds(Action method, float seconds, float delay = 0f)
        {
            return StartCoroutine(_UpdateForSeconds(method, seconds, delay));
        }

        public Coroutine UpdateForSeconds<T>(Action<T> method, float seconds, T args, float delay = 0f)
        {
            return StartCoroutine(_UpdateForSeconds_T(method, seconds, args, delay));
        }

        #region 内部调用

        private IEnumerator _ExecuteLater(Action mathdem, float time)
        {
            yield return new WaitForSeconds(time);
            mathdem();
        }

        private IEnumerator _ExecuteLater_T<T>(Action<T> mathdom, float time, T args)
        {
            yield return new WaitForSeconds(time);
            mathdom(args);
        }

        private IEnumerator _ExecuteSeconds(Action mathdom, float times, float duringTime)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
                    yield return 0;
                mathdom();
            }
        }

        private IEnumerator _ExecuteSeconds_T<T>(Action<T> mathdom, float times, float duringTime, T args)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < duringTime; timer += Time.deltaTime)
                    yield return 0;
                mathdom(args);
            }
        }

        private IEnumerator _ExecuteSeconds_Action(Action method, float times, float dur, Action endCall)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < dur; timer += Time.deltaTime)
                    yield return 0;
                method();
            }

            endCall();
        }

        private IEnumerator _ExecuteSeconds_Action_T<T>(Action<T> method, float times, float dur, T args,
            Action<T> endCall)
        {
            for (int i = 0; i < times; i++)
            {
                for (var timer = 0f; timer < dur; timer += Time.deltaTime)
                    yield return 0;
                method(args);
            }

            endCall(args);
        }

        private IEnumerator _UpdateForSeconds(Action mathdom, float seconds, float start)
        {
            for (var d = 0f; d < start; d += Time.deltaTime)
                yield return 0;
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                mathdom();
            }
        }

        private IEnumerator _UpdateForSeconds_T<T>(Action<T> mathdom, float seconds, T args, float start)
        {
            for (var d = 0f; d < start; d += Time.deltaTime)
                yield return 0;
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                mathdom(args);
            }

        }

        private IEnumerator _UpdateForSeconds_Action(Action method, float time, Action endcall)
        {
            for (var timer = 0f; timer < time; timer += Time.deltaTime)
            {
                yield return 0;
                method();
            }

            yield return 0;
            endcall();
        }


        private IEnumerator _UpdateForSeconds_Action_T<T>(Action<T> method, float seconds, T arg, Action<T> endCall)
        {
            for (var timer = 0f; timer < seconds; timer += Time.deltaTime)
            {
                yield return 0;
                method(arg);
            }

            yield return 0;
            endCall(arg);
        }


        private IEnumerator _ExecuteUntilTrue(Func<bool> method, Action endCall)
        {
            while (true)
            {
                if (method())
                {
                    endCall();
                    break;
                }

                yield return 0;
            }
        }

        #endregion


        #endregion

        private event Action updateEvent;
        private event Action fixedUpdateEvent;
        private event Action guiEvent;
        private event Action startEvent;

        private List<UpdateTestPair> callBackPairs = new List<UpdateTestPair>();

        void Start()
        {
            if (startEvent != null)
                startEvent();
        }

        void Update()
        {
            if (updateEvent != null)
                updateEvent();
            foreach (var pair in callBackPairs)
            {
                if (pair.IsOk())
                    pair.func();
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdateEvent != null)
                fixedUpdateEvent();
        }

        void OnGUI()
        {
            if (guiEvent != null)
                guiEvent();
        }

        public void AddStartFunc(Action func)
        {
            startEvent += func;
        }

        public void RemoveStartFunc(Action func)
        {
            startEvent -= func;
        }

        public void AddUpdateTest(UpdateTestPair pair)
        {
            if (callBackPairs.Contains(pair))
                Debug.Log("注意！已经包含这个UpdateTestPair！");
            callBackPairs.Add(pair);
        }

        public void RemoveUpdateTest(UpdateTestPair pair)
        {
            callBackPairs.Remove(pair);
        }


        public void AddUpdateFunc(Action func)
        {
            updateEvent += func;
        }

        public void RemoveUpdateFunc(Action func)
        {
            updateEvent -= func;

        }

        public void AddFixedUpdateFunc(Action func)
        {
            fixedUpdateEvent += func;
        }

        public void RemoveFixedUpdateFunc(Action func)
        {
            fixedUpdateEvent -= func;
        }

        public void AddGUIFunc(Action func)
        {
            guiEvent += func;
        }

        public void RemoveGUIFunc(Action func)
        {
            guiEvent -= func;
        }



    }
}
