using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HWEmu
{
    public abstract class Connectable
    {
        public List<IO> IOs { get; set; } = new List<IO>();

        public static void DrawConnectionPoints(List<IO> list)
        {
            foreach (IO io in list)
            {
                Raylib.DrawCircleV(io.Position, 10f, Color.Red);
            }
        }
    }
}
