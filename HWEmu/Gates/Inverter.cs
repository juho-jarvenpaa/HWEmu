using Raylib_cs;
using System.Numerics;

namespace HWEmu.Gates
{
    public class Inverter : Connectable
    {
        public Vector2 TriangleTop { get; set; }
        private Vector2 TriangleBottom { get; set; }
        private Vector2 TriangleRight { get; set; }

        const int scale = 1;

        const float circleRadius = 10 * scale;
        const int commonDimensionValue = 50 * scale;
        const float lineThick = 10 * scale;

        public Inverter(Vector2 triangleRightPos)
        {
            TriangleTop = triangleRightPos + new Vector2(-commonDimensionValue, -commonDimensionValue);
            TriangleBottom = triangleRightPos + new Vector2(-commonDimensionValue, +commonDimensionValue);
            TriangleRight = triangleRightPos;

            Inputs.Add(new Input {
                Position = new Vector2(triangleRightPos.X - commonDimensionValue * 2, triangleRightPos.Y),
                State = false,
                Name = "In",
                Guid = Guid.NewGuid(),
                Parent = this,
            });
            Outputs.Add(new Output {
                Position = new Vector2(triangleRightPos.X + commonDimensionValue, triangleRightPos.Y),
                State = true,
                Guid = Guid.NewGuid(),
                Name = "Out",
                Parent = this
            });
        }

        public static void DrawInverter(Inverter inverter)
        {
            if (inverter.Inputs[0].State)
            {
                Raylib.DrawLineEx(inverter.Inputs[0].Position, inverter.TriangleRight, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(inverter.Inputs[0].Position, inverter.TriangleRight, lineThick, Color.DarkGray);
            }

            if (inverter.Outputs[0].State == false)
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.Outputs[0].Position, lineThick, Color.DarkGray);
            }
            else
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.Outputs[0].Position, lineThick, Color.DarkBlue);
            }

            Raylib.DrawTriangle(inverter.TriangleTop, inverter.TriangleBottom, inverter.TriangleRight, Color.White);

            Raylib.DrawCircle((int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y, circleRadius, Color.White);

            // debug
            //Raylib.DrawText(inverter.IOs[1].Guid.ToString(), (int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y - 20, 20, Color.Yellow);
            //Raylib.DrawText(inverter.IOs[0].Guid.ToString(), (int)inverter.IOs[0].Position.X, (int)inverter.IOs[0].Position.Y + 20, 20, Color.Yellow);

            if (Program.showLinkablePositions)
            {
                DrawConnectionPoints(inverter.Inputs, inverter.Outputs);
            }
        }

        public override void CheckIfInputShouldChange(Connector connector)
        {
            // Check if state is different
            if(Inputs[0].State != connector.State)
            {
                bool ChangeInputState = true;

                foreach (Connector c in Program.Connectors)
                {
                    // Check if one of the connectors provides true
                    if(c.State == true)
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
                    Inputs[0].State = connector.State;

                    // Update output
                    Outputs[0].State = !Inputs[0].State;

                    // Then update connectors that are related to output
                    foreach (Connector c in Program.Connectors)
                    {
                        if (c.OldOutput.Guid == Outputs[0].Guid)
                        {
                            // Set connector state
                            c.State = Outputs[0].State;
                            Program.ConnectorStateQueue.Add(c);
                        }
                    }
                }
            }
        }
    }
}
