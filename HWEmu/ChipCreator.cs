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
                    if(Raylib.IsMouseButtonDown(MouseButton.Left))
                    {

                    }
                    else
                    {
                        // This part adds the IO's to list
                        // Triggers when left click is released
                        // Select IO's inside the painted area
                        MouseSelectEndpoint = InputHandle.MousePosVec2;
                        
                        // TODO rethink how to deal with And, Or etc.
                        //foreach (var item in )
                        //{

                        //}

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
