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

        public static void ProcessInputs()
        {
            MousePosVec2 = Raylib.GetMousePosition();

            if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
            {
                Program.showLinkablePositions = true;

                if (Raylib.IsMouseButtonDown(MouseButton.Left))
                {
                    if (Program.SelectedOldOutput == null)
                    {
                        // Iterate over all ui items to see if something is close

                        foreach (var chip in Program.ProjectChips)
                        {
                            foreach (var o in chip.Outputs)
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

                        foreach (var or in Program.ors)
                        {
                            foreach (var o in or.Outputs)
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

                        foreach (var and in Program.ands)
                        {
                            foreach (var o in and.Outputs)
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

                        foreach (var i in Program.inverters)
                        {
                            foreach (var io in i.Outputs)
                            {
                                if (Logic.PointCloseEnough(MousePosVec2, io.Position))
                                {
                                    Program.SelectedOldOutput = io;
                                    break;
                                }
                            }
                            if (Program.SelectedOldOutput != null)
                            {
                                break;
                            }
                        }
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
                        // Iterate over all ui items to see if something is close

                        foreach (var chip in Program.ProjectChips)
                        {
                            foreach (var io in chip.Inputs)
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

                        foreach (var i in Program.inverters)
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
                        foreach (var or in Program.ors)
                        {
                            foreach (var io in or.Inputs)
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
                        foreach (var and in Program.ands)
                        {
                            foreach (var io in and.Inputs)
                            {
                                if (Logic.PointCloseEnough(MousePosVec2, io.Position))
                                {
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

                    Program.SelectedOldOutput = null;
                    Program.draggingConnection = false;
                }
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
            }
        }
    }
}
