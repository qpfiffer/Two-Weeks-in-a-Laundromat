using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Delve_Engine.DataTypes;

namespace Delve_Engine.Interfaces
{
    interface IInputHandler
    {
        void handleInput(ref InputInfo info);
    }
}
