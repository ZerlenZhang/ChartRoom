using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.Script
{
    public class UpdateTestPair
    {
        public Func<bool> IsOk;
        public Action func;

        public UpdateTestPair(Func<bool> isok, Action func)
        {
            this.IsOk = isok;
            this.func = func;
        }
    }
}
