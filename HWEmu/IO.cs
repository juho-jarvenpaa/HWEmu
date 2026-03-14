using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public class Input
    {
        public required Vector2 Position { get; set; }
        public required string Name { get; set; }
        public required bool State { get; set; }
        public required Guid Guid { get; set; }
        public required Connectable Parent { get; set; }
    }
    public class Output
    {
        public required Vector2 Position { get; set; }
        public required string Name { get; set; }
        public required bool State { get; set; }
        public required Guid Guid { get; set; }
        public required Connectable Parent { get; set; }
    }
}
