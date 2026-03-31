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
        TypeName,
        Saving
    }

    public static class ChipCreator
    {
        public static ChipCreatorMode Mode { get; set; } = ChipCreatorMode.Selecting;
        public static List<Input> SelectedInputs { get; set; } = new List<Input>();
        public static List<Output> SelectedOutputs { get; set; } = new List<Output>();
        public static Vector2 SelectStartPoint { get; set; } = new Vector2();

        public static void Save()
        {
            if(SelectedInputs.Count > 0 && SelectedOutputs.Count > 0)
            {
                // Iterate over all inputs and outputs and form a truth table
            }
        }

        public static void ProcessChipCreatorModeChanges()
        {
            switch(Mode)
            {
                case ChipCreatorMode.Selecting:
                    if (Raylib.IsMouseButtonDown(MouseButton.Left))
                    {
                        Mode = ChipCreatorMode.Selecting;
                        SelectStartPoint = InputHandle.MousePosVec2;
                    }
                    if (Raylib.IsKeyDown(KeyboardKey.Enter))
                    {
                        Mode = ChipCreatorMode.TypeName;
                    }
                    break;
                case ChipCreatorMode.SelectingAndDragging:
                    if(Raylib.IsMouseButtonReleased(MouseButton.Left))
                    {
                        // Select IO's inside the painted area
                        Mode = ChipCreatorMode.Selecting;
                    }
                    break;
                case ChipCreatorMode.TypeName:
                    if(Raylib.IsKeyDown(KeyboardKey.Enter))
                    {
                        Mode = ChipCreatorMode.Saving;
                        Save();
                    }
                    break;
                case ChipCreatorMode.Saving:
                    if(SelectedInputs.Count == 0)
                    {
                        Mode = ChipCreatorMode.Selecting;
                    }
                    break;
            }
        }
    }
}
