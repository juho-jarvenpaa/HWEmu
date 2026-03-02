using Raylib_cs;
using System.Numerics;

namespace HWEmu.Gates
{
    public class Inverter
    {
        public Vector2 TriangleTop { get; set; }
        private Vector2 TriangleBottom { get; set; }
        private Vector2 TriangleRight { get; set; }
        public Rectangle InLine { get; set; }
        public Rectangle OutLine { get; set; }

        public bool InState { get; set; } = false;
        public Enums.State OutState { get; set; } = Enums.State.Off;

        const int scale = 1;

        const float circleRadius = 10 * scale;
        const int commonDimensionValue = 50 * scale;
        const int lineHeight = 10 * scale;

        public Inverter(Vector2 triangleRightPos)
        {
            TriangleTop = triangleRightPos + new Vector2(-commonDimensionValue, -commonDimensionValue);
            TriangleBottom = triangleRightPos + new Vector2(-commonDimensionValue, +commonDimensionValue);
            TriangleRight = triangleRightPos;

            OutLine = new Rectangle(triangleRightPos, new Vector2(commonDimensionValue, lineHeight));
            InLine = new Rectangle(new Vector2(triangleRightPos.X - commonDimensionValue - OutLine.Width,triangleRightPos.Y), new Vector2(commonDimensionValue, lineHeight));
        }

        public static void DrawInverter(Inverter inverter)
        {
            if(inverter.InState)
            {
                Raylib.DrawRectangleRec(inverter.InLine, Color.Blue);
            }
            else
            {
                Raylib.DrawRectangleRec(inverter.InLine, Color.Gray);
            }

            if (inverter.OutState == Enums.State.Off)
            {
                Raylib.DrawRectangleRec(inverter.OutLine, Color.Gray);
            }
            else
            {
                Raylib.DrawRectangleRec(inverter.OutLine, Color.Blue);
            }

            Raylib.DrawTriangle(inverter.TriangleTop, inverter.TriangleBottom, inverter.TriangleRight, Color.White);

            Raylib.DrawCircle((int)inverter.TriangleRight.X, (int)inverter.TriangleRight.Y, circleRadius, Color.White);
        }
    }
}
