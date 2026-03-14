using Raylib_cs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HWEmu
{
    public abstract class Connectable
    {
        public List<Input> Inputs { get; set; } = new List<Input>();
        public List<Output> Outputs { get; set; } = new List<Output>();

        public static void DrawConnectionPoints(List<Input> inputs, List<Output> outputs)
        {
            foreach (Input i in inputs)
            {
                Raylib.DrawCircleV(i.Position, 10f, Color.Red);
            }
            foreach (Output o in outputs)
            {
                Raylib.DrawCircleV(o.Position, 10f, Color.Red);
            }
        }

        public abstract void CheckIfInputShouldChange(Connector c);
    }
}
