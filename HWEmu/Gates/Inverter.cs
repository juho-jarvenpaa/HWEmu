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

            IOs.Add(new IO {
                Position = new Vector2(triangleRightPos.X - commonDimensionValue * 2, triangleRightPos.Y),
                State = false,
                Name = "In",
                Guid = Guid.NewGuid(),
                Type = IO.TypeIO.Input,
                Parent = this,
            });
            IOs.Add(new IO {
                Position = new Vector2(triangleRightPos.X + commonDimensionValue, triangleRightPos.Y),
                State = true,
                Guid = Guid.NewGuid(),
                Name = "Out",
                Type = IO.TypeIO.Output,
                Parent = this
            });
        }

        public static void DrawInverter(Inverter inverter)
        {
            if (inverter.IOs[0].State)
            {
                Raylib.DrawLineEx(inverter.IOs[0].Position, inverter.TriangleRight, lineThick, Color.DarkBlue);
            }
            else
            {
                Raylib.DrawLineEx(inverter.IOs[0].Position, inverter.TriangleRight, lineThick, Color.DarkGray);
            }

            if (inverter.IOs[1].State == false)
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.IOs[1].Position, lineThick, Color.DarkGray);
            }
            else
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.IOs[1].Position, lineThick, Color.DarkBlue);
            }

            Raylib.DrawTriangle(inverter.TriangleTop, inverter.TriangleBottom, inverter.TriangleRight, Color.White);

            Raylib.DrawCircle((int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y, circleRadius, Color.White);

            // debug
            //Raylib.DrawText(inverter.IOs[1].Guid.ToString(), (int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y - 20, 20, Color.Yellow);
            //Raylib.DrawText(inverter.IOs[0].Guid.ToString(), (int)inverter.IOs[0].Position.X, (int)inverter.IOs[0].Position.Y + 20, 20, Color.Yellow);

            if (Program.showLinkablePositions)
            {
                DrawConnectionPoints(inverter.IOs);
            }
        }

        public override void CheckIfInputShouldChange(IO io, bool state)
        {
            // Check if state is different
            if(IOs[0].State != state)
            {
                bool trueFound = false;

                // Check if one of the connectors provides true
                foreach (Connector c in Program.Connectors)
                {
                    if (c.NewInput.Guid == IOs[0].Guid)
                    {
                        // Just make sure it is not the same connector
                        if((c.NewInput.Guid != io.Guid))
                        {
                            if (c.State == true)
                            {
                                trueFound = true;
                            }
                        }
                    }
                }

                // Do nothing if state did not change
                if(trueFound)
                {

                }
                else
                {
                    // Set the input state
                    IOs[0].State = state;

                    // Update output
                    IOs[1].State = !IOs[0].State;

                    // Then update connectors that are related to output
                    foreach (Connector c in Program.Connectors)
                    {
                        if (c.OldOutput.Guid == IOs[1].Guid)
                        {
                            Program.ConnectorStateQueue.Add(c);
                        }
                    }
                }
            }
        }
    }
}
