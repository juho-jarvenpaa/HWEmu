using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;

namespace HWEmu
{
    public class Connector
    {
        public IO NewInput;
        public IO OldOutput;
        public bool State = false;
    }
}
