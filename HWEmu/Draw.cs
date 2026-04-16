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
            foreach (Chip chip in Program.ProjectChips)
            {
                Chip.DrawChip(chip);
            }

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

            if (Program.draggingConnection)
            {
                Raylib.DrawLineEx(InputHandle.MousePosVec2, Program.SelectedOldOutput.Position, 10f, Color.Yellow);
            }

            if(ChipCreator.Mode == ChipCreatorMode.AskingChipName)
            {
                var p = Raylib.GetScreenCenter();
                var w = 800;
                var h = 50;

                string name = "Type Chip Name: " + ChipCreator.ChipName;

                Raylib.DrawRectangle((int)p.X, (int)p.Y, w, h, Color.Yellow);
                Raylib.DrawText(name, (int)p.X, (int)p.Y, 40, Color.Red);
            }
        }
    }
}
