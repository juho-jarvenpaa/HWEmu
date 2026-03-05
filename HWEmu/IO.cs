using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public class IO
    {
        public required Vector2 Position { get; set; }
        public required string Name { get; set; }
        public required bool State { get; set; }
        public Guid Guid { get; set; } = new Guid();
        public required Connectable Parent { get; set; }
    }
}
