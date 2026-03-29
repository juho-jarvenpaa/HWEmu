using HWEmu.Gates;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Text;

namespace HWEmu
{
    public static class Draw
    {
        public static void DrawChipsAndConnecters()
        {
            foreach (Chip chip in Program.Chips)
            {
                Chip.DrawChip(chip);
            }

            foreach (var psu in Program.psus)
                Psu.DrawPSU(psu);

            foreach (var i in Program.inverters)
                Inverter.DrawInverter(i);

            foreach (var or in Program.ors)
                Or.DrawOr(or);

            foreach (var and in Program.ands)
                And.DrawAnd(and);

            foreach (var c in Program.Connectors)
            {
                if (c.State)
                {
                    Raylib.DrawLineEx(c.NewInput.Position, c.OldOutput.Position, 10f, Color.Blue);
                }
                else
                {
                    Raylib.DrawLineEx(c.NewInput.Position, c.OldOutput.Position, 10f, Color.Gray);
                }
            }
        }
    }
}
