using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<Connector> Connectors = new List<Connector>();
        public static List<Chip> ProjectChips = new List<Chip>();

        public static List<Inverter> inverters = new();
        public static List<Or> ors = new();
        public static List<And> ands = new();

        public static bool showLinkablePositions = false;
        public static bool draggingConnection = false;
        public static Output SelectedOldOutput = null;

        public static Vector2 GateSize = new(100, 100);

        static async Task Main(string[] args)
        {
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1000, 1000, "HWEmu");
            Raylib.SetTargetFPS(60);

            Logic.StartStateUpdateLoop();
            HWEmu.Chips.LoadChips();
            HWEmu.Chips.AddChips();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.ClearBackground(Color.Black);
                HWEmu.InputHandle.ProcessInputs();
                Raylib.BeginDrawing();
                Draw.DrawChipsAndConnecters();
                Raylib.EndDrawing();
            }
            Logic.StopStateUpdates();
        }
    }
}