using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.View.Panels
{
    /// <summary>
    /// 总UI管理者
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        public UIManager()
        {

        }

        private Stack<IStackPanel> panelStack = new Stack<IStackPanel>();

        public IStackPanel CurrentPanel
        {
            get { return panelStack.Peek(); }
        }

        public void PushPanel(string name)
        {
            if (panelStack.Count != 0)
                panelStack.Peek().Disable();

            var panel = AbstractPanel.GetPanel(name);
            panel.Enable();
            panelStack.Push(panel);
        }


        public void PopPanel()
        {
            panelStack.Peek().Destory();
            panelStack.Pop();
            if (panelStack.Count >= 0)
                panelStack.Peek().Enable();
        }


    }
}
