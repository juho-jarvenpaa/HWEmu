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

        public static void Save()
        {
            if(SelectedInputs.Count > 0 && SelectedOutputs.Count > 0)
            {
                // Iterate over all inputs and outputs and form a truth table
                foreach(var input in SelectedInputs.Values)
                {
                    
                }
            }
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
                    if (Raylib.IsKeyDown(KeyboardKey.Enter))
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

                        foreach (var chip in Program.ProjectChips)
                        {
                            foreach (var i in chip.Inputs)
                            {
                                if(Raylib.CheckCollisionPointRec(i.Position, selectionRect))
                                {
                                    while(!SelectedInputs.TryAdd(i.Name, i))
                                    {
                                        // Increment the name // TODO
                                        i.ToString().Split("_");
                                    }
                                }
                            }
                            foreach (var o in chip.Outputs)
                            {

                            }
                        }

                        Mode = ChipCreatorMode.Selecting;
                    }
                    break;
                case ChipCreatorMode.AskingChipName:
                    if(Raylib.IsKeyDown(KeyboardKey.Enter))
                    {
                        Mode = ChipCreatorMode.SavingInProgress;
                        Save();
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
    }
}
