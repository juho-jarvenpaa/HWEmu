using HWEmu.Gates;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public static class InputHandle
    {
        public static Vector2 MousePosVec2;

        public static void SelectInputForGates(IEnumerable<Connectable> connectables)
        {
            foreach (var i in connectables)
            {
                foreach (var io in i.Inputs)
                {
                    if (Logic.PointCloseEnough(MousePosVec2, io.Position))
                    {
                        // found connection
                        Connector c = new Connector() { OldOutput = Program.SelectedOldOutput, NewInput = io };
                        c.State = Program.SelectedOldOutput.State;
                        Program.Connectors.Add(c);
                        Logic.ConnectorStateQueue.Add(c);
                        break;
                    }
                }
                if (Program.SelectedOldOutput == null)
                {
                    break;
                }
            }
        }

        public static void TrySelectOutputFor(IEnumerable<Connectable> connectables)
        {
            foreach (var c in connectables)
            {
                foreach (var o in c.Outputs)
                {
                    if (Logic.PointCloseEnough(MousePosVec2, o.Position))
                    {
                        Program.SelectedOldOutput = o;
                        break;
                    }
                }
                if (Program.SelectedOldOutput != null)
                {
                    break;
                }
            }
        }

        public static void TrySelectInput()
        {
            // Iterate over all ui items to see if something is close
            SelectInputForGates(Program.ProjectChips);
            SelectInputForGates(Program.inverters);
            SelectInputForGates(Program.ors);
            SelectInputForGates(Program.ands);
        }

        public static void TrySelectOutput()
        {
            // Iterate over all ui items to see if something is close
            TrySelectOutputFor(Program.ProjectChips);
            TrySelectOutputFor(Program.ors);
            TrySelectOutputFor(Program.ands);
            TrySelectOutputFor(Program.inverters);
        }

        public static void ProcessInputsWithShift()
        {
            Program.showLinkablePositions = true;

            if (Raylib.IsMouseButtonDown(MouseButton.Left))
            {
                if (Program.SelectedOldOutput == null)
                {
                    TrySelectOutput();
                }
                else
                {
                    Program.draggingConnection = true;
                }
            }
            else
            {
                if (Program.draggingConnection)
                {
                    TrySelectInput();
                }

                // Reset
                Program.SelectedOldOutput = null;
                Program.draggingConnection = false;
            }
        }

        public static void ProcessInputs()
        {
            MousePosVec2 = Raylib.GetMousePosition();

            if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
            {
                ProcessInputsWithShift();
            }
            else if(Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl) || ChipCreator.Mode == ChipCreatorMode.AskingChipName)
            {
                ChipCreator.ProcessChipCreatorModeChanges();
            }
            else
            {
                Program.showLinkablePositions = false;

                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    Logic.StopStateUpdates();

                    Or or = new(new Rectangle(MousePosVec2, Program.GateSize));
                    Program.ors.Add(or);

                    Logic.StartStateUpdateLoop();
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.Middle))
                {
                    Logic.StopStateUpdates();

                    And and = new(new Rectangle(MousePosVec2, Program.GateSize));
                    Program.ands.Add(and);

                    Logic.StartStateUpdateLoop();
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    Logic.StopStateUpdates();
                    Inverter i = new Inverter(MousePosVec2);
                    Program.inverters.Add(i);
                    Logic.StartStateUpdateLoop();
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.Extra))
                {
                    Logic.StopStateUpdates();

                    var og = HWEmu.Chips.ChipList[0];

                    Chip newChip = new Chip
                    {
                        binaryStateTable = og.binaryStateTable,
                        ChipName = og.ChipName,
                        CurrentBinaryInputState = og.CurrentBinaryInputState,
                        Inputs = new List<Input>(),
                        Outputs = new List<Output>(),
                        Rectangle = new Rectangle(MousePosVec2, og.Rectangle.Width, og.Rectangle.Height)
                    };

                    foreach (var item in og.Inputs)
                    {
                        newChip.Inputs.Add(new Input(){
                            Guid = Guid.NewGuid(), Name = item.Name, Parent = null, Position = new Vector2(), State = item.State});
                    }
                    foreach (var item in og.Outputs)
                    {
                        newChip.Outputs.Add(new Output()
                        {
                            Guid = Guid.NewGuid(),
                            Name = item.Name,
                            Parent = null,
                            Position = new Vector2(),
                            State = item.State
                        });
                    }

                    newChip.InputCount = newChip.Inputs.Count();
                    newChip.OutputCount = newChip.Outputs.Count();

                    Program.ProjectChips.Add(newChip);

                    // TODO
                    // Set positions first!
                    Chip.IOPositionOffsetSetFromChip(newChip);

                    Logic.StartStateUpdateLoop();
                }
            }
        }
    }
}
