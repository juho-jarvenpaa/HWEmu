using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public enum ChipCreatorMode
    {
        Selecting,
        SelectingAndDragging,
        AskingChipName,
        SavingInProgress
    }

    public static class ChipCreator
    {
        public static ChipCreatorMode Mode { get; set; } = ChipCreatorMode.Selecting;
        public static Dictionary<string, Input> SelectedInputs { get; set; } = new Dictionary<string, Input>();
        public static Dictionary<string, Output> SelectedOutputs { get; set; } = new Dictionary<string, Output>();
        public static Vector2 MouseSelectStartPoint { get; set; } = new Vector2();
        public static Vector2 MouseSelectEndpoint { get; set; } = new Vector2();

        public static string ChipName = "";

        public static void Save()
        {
            string content = "|";

            string path = System.AppDomain.CurrentDomain.BaseDirectory + "chips";

            if (SelectedInputs.Count > 0 && SelectedOutputs.Count > 0)
            {
                // Iterate over all inputs and outputs and form a truth table
                foreach(var input in SelectedInputs.Keys)
                {
                    content += " " + input + " |";
                }

                content += " -> |";

                // Iterate over all inputs and outputs and form a truth table
                foreach (var output in SelectedOutputs.Keys)
                {
                    content += " " + output + " |";
                }
            }

            content += "\n";

            // TODO: Emulate all possible inputs and determine output.
            // Set all inputs to 0
            foreach(var input in SelectedInputs)
            {
                input.Value.State = false;
            }

            int bits = SelectedInputs.Count;
            int max = (1 << bits) - 1;

            // TODO 

            for (int value = 0; value <= max; value++)
            {
                for (int i = bits - 1; i >= 0; i--)
                {
                    content += "| ";
                    content += (value >> i) & 1;
                    content += " ";
                }
                content += "| → |\n";
            }


            string filename = "\\" + ChipName;

            File.WriteAllText(path + filename + ".md", content);

            ChipName = "";
        }

        public static void ProcessChipCreatorModeChanges()
        {
            switch(Mode)
            {
                case ChipCreatorMode.Selecting:
                    if (Raylib.IsMouseButtonDown(MouseButton.Left))
                    {
                        Mode = ChipCreatorMode.SelectingAndDragging;
                        MouseSelectStartPoint = InputHandle.MousePosVec2;
                    }
                    if (Raylib.IsKeyReleased(KeyboardKey.Enter) && (SelectedInputs.Count > 0) && (SelectedOutputs.Count > 0))
                    {
                        Mode = ChipCreatorMode.AskingChipName;
                    }
                    break;
                case ChipCreatorMode.SelectingAndDragging:
                    MouseSelectEndpoint = InputHandle.MousePosVec2;
                    // We need to start drawing from smallest value. Otherwise it will break
                    var x = MathF.Min(MouseSelectStartPoint.X, MouseSelectEndpoint.X);
                    var y = MathF.Min(MouseSelectStartPoint.Y, MouseSelectEndpoint.Y);
                    var width = MathF.Abs(MouseSelectEndpoint.X - MouseSelectStartPoint.X);
                    var height = MathF.Abs(MouseSelectEndpoint.Y - MouseSelectStartPoint.Y);

                    Rectangle selectionRect = new Rectangle(x, y, width, height);

                    if (Raylib.IsMouseButtonDown(MouseButton.Left))
                    {
                        Raylib.DrawRectangleLinesEx(
                            selectionRect,
                            5f,
                            Color.Red);
                    }
                    else
                    {
                        // This part adds the IO's to list
                        // Triggers when left click is released
                        // Select IO's inside the painted area
                        ProcessIO(Program.ProjectChips, selectionRect);
                        ProcessIO(Program.ands, selectionRect);
                        ProcessIO(Program.ors, selectionRect);
                        ProcessIO(Program.inverters, selectionRect);

                        Mode = ChipCreatorMode.Selecting;
                    }
                    break;
                case ChipCreatorMode.AskingChipName:

                    if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                    {
                        Mode = ChipCreatorMode.Selecting;
                    }
                    else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                    {
                        if (!string.IsNullOrEmpty(ChipName))
                        {
                            Mode = ChipCreatorMode.SavingInProgress;
                            Save();
                        }
                    }
                    else if (Raylib.IsKeyPressed(KeyboardKey.Backspace) && ChipName.Length > 0)
                    {
                        ChipName = ChipName.Substring(0, ChipName.Length - 1);
                    }

                    int c = Raylib.GetCharPressed();
                    while (c > 0)
                    {
                        ChipName += (char)c;
                        c = Raylib.GetCharPressed();
                    }

                    break;
                case ChipCreatorMode.SavingInProgress:
                    // After saving return to selecting
                    if(SelectedInputs.Count == 0)
                    {
                        Mode = ChipCreatorMode.Selecting;
                    }
                    break;
            }
        }

        private static void ProcessIO(IEnumerable<Connectable> connectables, Rectangle selectionRect)
        {
            foreach (var chip in connectables)
            {
                foreach (var i in chip.Inputs)
                {
                    if (Raylib.CheckCollisionPointRec(i.Position, selectionRect))
                    {
                        string modifiedInputName = i.Name;

                        int incrementInt = 1;

                        while (!SelectedInputs.TryAdd(modifiedInputName, i))
                        {
                            if(modifiedInputName.Contains("_"))
                            {
                                string[] strings = modifiedInputName.Split("_");
                                incrementInt = int.Parse(strings.Last());
                                incrementInt++;
                                modifiedInputName = strings.First() + incrementInt.ToString();
                            }
                            else
                            {
                                modifiedInputName += "_" + incrementInt.ToString();
                            }
                        }
                    }
                }
                foreach (var o in chip.Outputs)
                {
                    if (Raylib.CheckCollisionPointRec(o.Position, selectionRect))
                    {
                        string modifiedOutputName = o.Name;

                        int incrementInt = 1;

                        while (!SelectedOutputs.TryAdd(modifiedOutputName, o))
                        {
                            if (modifiedOutputName.Contains("_"))
                            {
                                string[] strings = modifiedOutputName.Split("_");
                                incrementInt = int.Parse(strings.Last());
                                incrementInt++;
                                modifiedOutputName = strings.First() + incrementInt.ToString();
                            }
                            else
                            {
                                modifiedOutputName += "_" + incrementInt.ToString();
                            }
                        }
                    }
                }
            }
        }
    }
}
