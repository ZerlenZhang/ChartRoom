using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.View.Panels
{
    public interface IStackPanel
    {
        //        string panelName { get; set; }
        string PanelName { get; }
        void Enable();
        void Disable();
        void Destory();
    }
}
