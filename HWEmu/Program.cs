using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<(Vector2, Vector2)> Connections = new List<(Vector2, Vector2)>();

        public static List<Psu> psus = new();
        public static List<Inverter> inverters = new();

        public static CancellationTokenSource? _logicCts;
        public static Task? _logicTask;

        public static bool showLinkablePositions = false;
        public static bool draggingConnection = false;
        public static IO SelectedIO = null;

        static async Task Main(string[] args)
        {
            Vector2 psuSize = new(100, 100);

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1000, 1000, "HWEmu");
            Raylib.SetTargetFPS(60);

            StartLogic();

            while (!Raylib.WindowShouldClose())
            {
                Raylib.ClearBackground(Color.Black);

                var mousePosVec2 = Raylib.GetMousePosition();

                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                {
                    showLinkablePositions = true;

                    if(Raylib.IsMouseButtonDown(MouseButton.Left))
                    {
                        if(SelectedIO == null)
                        {
                            // Iterate over all ui items to see if something is close
                            foreach (var i in inverters)
                            {
                                foreach (var io in i.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        SelectedIO = io;
                                        break;
                                    }
                                }
                                if(SelectedIO != null)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            draggingConnection = true;
                        }
                    }
                    else
                    {
                        if(draggingConnection)
                        {
                            // Iterate over all ui items to see if something is close
                            foreach (var i in inverters)
                            {
                                foreach (var io in i.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        if(io != SelectedIO)
                                        {
                                            // found connection
                                            var tuple = (io.Position, SelectedIO.Position);
                                            Connections.Add(tuple);
                                            SelectedIO = null;
                                            break;
                                        }
                                    }
                                }
                                if (SelectedIO == null)
                                {
                                    break;
                                }
                            }
                        }

                        draggingConnection = false;
                    }
                }
                else
                {
                    showLinkablePositions = false;

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        StopLogic();

                        Psu p = new(new Rectangle(mousePosVec2, psuSize));
                        psus.Add(p);

                        StartLogic();
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                    {
                        StopLogic();
                        Inverter i = new Inverter(mousePosVec2);
                        inverters.Add(i);
                        StartLogic();
                    }
                }



                Raylib.BeginDrawing();

                foreach (var psu in psus)
                    Psu.DrawPSU(psu);

                foreach (var i in inverters)
                    Inverter.DrawInverter(i);


                foreach (var c in Connections)
                {
                    Raylib.DrawLineEx(c.Item1, c.Item2, 10f, Color.Blue);
                }

                if(draggingConnection)
                {
                    Raylib.DrawLineEx(mousePosVec2, SelectedIO.Position, 10f, Color.Yellow);
                }

                Raylib.EndDrawing();
            }

            StopLogic();
        }

        private static bool PointCloseEnough(Vector2 a, Vector2 b)
        {
            if(Raylib.CheckCollisionPointCircle(a,b,20f))
            { 
                return true;
            }

            return false;
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
            // Global reset to default values should exist
            // clock's ticking forces state changes
            // order or operation should be designed

            // Initial state
            // Check if there is circuits to connected to direct power.
            // If yes iterate state changes on those first.
            // Then check if nearest touching other components have states to change
            // Note that do not go deeper until all changes as far as others are done
            // Continue this until there is nothing to change
            // After initial check if there is clock/clocks that need to tick
            // If multiple clocks are ticking at the same time. Apply earlier rule of state changes


            // Reset all states to original

            // Go over all components 
            foreach (var psu in psus)
            {

            }
        }
    }
}