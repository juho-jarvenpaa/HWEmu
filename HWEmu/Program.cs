using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<Psu> psus = new List<Psu>();
        public static List<Inverter> inverters = new List<Inverter>();

        static void Main(string[] args)
        {
            Vector2 psuSize = new Vector2(100, 100);

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1000, 1000, "HWEmu");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {

                if(Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    var mousePosVec2 = Raylib.GetMousePosition();
                    Psu p = new Psu(new Rectangle(mousePosVec2, psuSize));
                    psus.Add(p);
                    CalculateStates();
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    Inverter i = new Inverter(Raylib.GetMousePosition());
                    inverters.Add(i);
                    CalculateStates();
                }

                Raylib.BeginDrawing();

                foreach (var psu in psus)
                {
                    Psu.DrawPSU(psu);
                }

                foreach (var i in inverters)
                {
                    Inverter.DrawInverter(i);
                }

                Raylib.EndDrawing();
            }
        }

        private static void CalculateStates()
        {
            foreach (var psu in psus)
            {
                // Determine where is the shortest path and calculate the state change there
                // This needs a little bit more thought...
                // Making inverter input and output to meet should not cause any odd cases

                //foreach(var i in inverters)
                //{
                //    if(Raylib.CheckCollisionRecs(psu.Bounds, i.InLine))
                //    {
                //        i.InState = true;
                //        // set outState false if not forced to stay true
                //        if(i.OutState != Enums.State.ForcedOn)
                //        {
                //            i.OutState = Enums.State.Off;
                //        }
                //    }
                //}
            }
        }
    }
}
