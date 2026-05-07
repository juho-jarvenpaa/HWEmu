using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu.Gates
{
    public class And : Connectable
    {
        public Rectangle Rectangle { get; set; }
        private Vector2 RightCircleCenter { get; set; }

        const float circleRadius = 50;
        const float lineThick = 10;

        public And(Rectangle rectangle)
        {
            Rectangle = rectangle;
            RightCircleCenter = new Vector2(Rectangle.Position.X + Rectangle.Width, Rectangle.Position.Y + (Rectangle.Height / 2));

            Inputs.Add(new Input
            {
                Position = new Vector2(Rectangle.Position.X - 30, Rectangle.Position.Y + 20),
                State = false,
                Name = "InA",
                Guid = Guid.NewGuid(),
                Parent = this,
            });
            Inputs.Add(new Input
            {
                Position = new Vector2(Rectangle.Position.X - 30, Rectangle.Position.Y + 70),
                State = false,
                Name = "InB",
                Guid = Guid.NewGuid(),
                Parent = this,
            });
            Outputs.Add(new Output
            {
                Position = new Vector2(Rectangle.Position.X + Rectangle.Width + 80, Rectangle.Position.Y + (Rectangle.Height / 2)),
                State = false,
                Guid = Guid.NewGuid(),
                Name = "Out",
                Parent = this
            });
        }

        public static void DrawAnd(And and)
        {
            if (and.Inputs[0].State)
            {
                Raylib.DrawLineEx(and.Inputs[0].Position, and.RightCircleCenter, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(and.Inputs[0].Position, and.RightCircleCenter, lineThick, Color.DarkGray);
            }

            if (and.Inputs[1].State)
            {
                Raylib.DrawLineEx(and.Inputs[1].Position, and.RightCircleCenter, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(and.Inputs[1].Position, and.RightCircleCenter, lineThick, Color.DarkGray);
            }

            Raylib.DrawRectangleRec(and.Rectangle, Color.White);


            // output
            if (and.Outputs[0].State)
            {
                Raylib.DrawLineEx(and.RightCircleCenter, and.Outputs[0].Position, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(and.RightCircleCenter, and.Outputs[0].Position, lineThick, Color.DarkGray);
            }

            Raylib.DrawCircle((int)and.RightCircleCenter.X, (int)and.RightCircleCenter.Y, circleRadius, Color.White);

            if (Program.showLinkablePositions)
            {
                DrawConnectionPoints(and.Inputs, and.Outputs);
            }
        }

        public override bool UpdateInputIfItShouldChange(Connector connector, bool ChangeInputState = false)
        {
            bool changed = false;

            // First check which input we are changing
            int index = 0;
            if (connector.NewInput.Guid == Inputs[1].Guid)
            {
                index++;
            }

            // Check if state is different
            if (ChangeInputState || Inputs[index].State != connector.State)
            {
                ChangeInputState = true;

                foreach (Connector c in Program.Connectors)
                {
                    // Check if one of the connectors provides true
                    if (c.State == true)
                    {
                        // Just make sure it is not the same connector
                        // So make sure that they have same input, different old output
                        if (c.NewInput.Guid == connector.NewInput.Guid && c.OldOutput != connector.OldOutput)
                        {
                            ChangeInputState = false;
                        }
                    }
                }
                // if false constant true input is provide by another connector
                if (ChangeInputState)
                {
                    // Set the input state
                    Inputs[index].State = connector.State;

                    // Update output
                    // actual or logic

                    if (Inputs[0].State == true && Inputs[1].State == true)
                    {
                        Outputs[0].State = true;
                    }
                    else
                    {
                        Outputs[0].State = false;
                    }

                    // then update connectors that are related to output
                    foreach (Connector c in Program.Connectors)
                    {
                        if (c.OldOutput.Guid == Outputs[0].Guid)
                        {
                            // set connector state
                            c.State = Outputs[0].State;
                            Logic.ConnectorStateQueue.Add(c);
                            changed = true;
                        }
                    }
                }
            }
            return changed;
        }
    }
}
