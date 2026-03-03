using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<Psu> psus = new();
        public static List<Inverter> inverters = new();

        public static CancellationTokenSource? _logicCts;
        public static Task? _logicTask;

        static async Task Main(string[] args)
        {
            Vector2 psuSize = new(100, 100);

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1000, 1000, "HWEmu");
            Raylib.SetTargetFPS(60);

            StartLogic();

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    StopLogic();

                    var mousePosVec2 = Raylib.GetMousePosition();
                    Psu p = new(new Rectangle(mousePosVec2, psuSize));
                    psus.Add(p);

                    StartLogic();
                }

                if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                {
                    StopLogic();
                    Inverter i = new(Raylib.GetMousePosition());
                    inverters.Add(i);
                    StartLogic();
                }

                Raylib.BeginDrawing();

                foreach (var psu in psus)
                    Psu.DrawPSU(psu);

                foreach (var i in inverters)
                    Inverter.DrawInverter(i);

                Raylib.EndDrawing();
            }

            StopLogic();
        }

        private static void StartLogic()
        {
            _logicCts = new CancellationTokenSource();
            _logicTask = Task.Run(() => LogicLoop(_logicCts.Token));
            Console.WriteLine("Logic started");
        }

        private static void StopLogic()
        {
            var cts = _logicCts;
            var task = _logicTask;

            if (cts == null || task == null)
                return;

            _logicCts = null;
            _logicTask = null;

            cts.Cancel();
            Console.WriteLine("Logic cancel signal sent");
            task.GetAwaiter().GetResult();
            cts.Dispose();
        }

        private static async Task LogicLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine("Logic happening");
                CalculateStates();
                await Task.Delay(100);
            }

            Console.WriteLine("Logic stopped");
        }

        private static void CalculateStates()
        {

        }
    }
}