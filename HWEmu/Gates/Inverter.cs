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
                Parent = this,
            });
            IOs.Add(new IO {
                Position = new Vector2(triangleRightPos.X + commonDimensionValue, triangleRightPos.Y),
                State = true,
                Name = "Out",
                Parent = this
            });
        }

        public static void DrawInverter(Inverter inverter)
        {
            if (inverter.IOs[0].State)
            {
                Raylib.DrawLineEx(inverter.IOs[0].Position, inverter.TriangleRight, lineThick, Color.Blue);
            }
            else
            {
                Raylib.DrawLineEx(inverter.IOs[0].Position, inverter.TriangleRight, lineThick, Color.Gray);
            }

            if (inverter.IOs[1].State == false)
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.IOs[1].Position, lineThick, Color.Gray);
            }
            else
            {
                Raylib.DrawLineEx(inverter.TriangleRight, inverter.IOs[1].Position, lineThick, Color.Blue);
            }

            Raylib.DrawTriangle(inverter.TriangleTop, inverter.TriangleBottom, inverter.TriangleRight, Color.White);

            Raylib.DrawCircle((int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y, circleRadius, Color.White);

            if(Program.showLinkablePositions)
            {
                DrawConnectionPoints(inverter.IOs);
            }
        }

        public override void IOStateChanged(IO io)
        {
            // Input
            if(io.Name == "In")
            {
                IOs[1].State = !IOs[0].State;
            }
            else
            {
                // Update Connector states
                var connections = Program.Connections.FindAll(x => x.Connectable1.Guid == io.Guid || x.Connectable2.Guid == io.Guid);
                Console.WriteLine("Connection count: " + connections.Count);
                foreach(var connection in connections)
                {
                    connection.State = !IOs[1].State;
                }
            }
        }
    }
}
