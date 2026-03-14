using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<Connector> Connectors = new List<Connector>();

        public static List<Connector> ConnectorStateQueue = new List<Connector>();

        public static List<Psu> psus = new();
        public static List<Inverter> inverters = new();
        public static List<Or> ors = new();
        public static List<And> ands = new();

        public static CancellationTokenSource? _logicCts;
        public static Task? _logicTask;

        public static bool showLinkablePositions = false;
        public static bool draggingConnection = false;
        public static IO SelectedOldOutput = null;

        static async Task Main(string[] args)
        {
            Vector2 psuSize = new(100, 100);

            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1000, 1000, "HWEmu");
            Raylib.SetTargetFPS(60);

            StartStateUpdateLoop();

            // load chips data
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "chips";
            string[] files = Directory.GetFiles(path, "*.md");

            List<Chip> chipList = new List<Chip>();

            int chipfileIterator = 0;

            foreach (string chipFile in files)
            {
                chipList.Add(new Chip{binaryStateTable = new Dictionary<string, string>()});

                var lines = File.ReadAllLines(chipFile);
                int lineIterator = 0;
                foreach (string line in lines)
                {
                    string trimmedLine = line.Replace(" ", String.Empty);
                    
                    // Header line
                    if (lineIterator == 0)
                    {
                        bool inputs = true;
                        string IOName = string.Empty;
                        foreach (char c in trimmedLine)
                        {
                            switch (c)
                            {
                                case '|':
                                    if(IOName != "")
                                    {
                                        if(inputs)
                                        {
                                            chipList[chipfileIterator].IOs.Add(new IO
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
                                                Type = IO.TypeIO.Input,
                                                Parent = chipList[chipfileIterator],
                                            });
                                        }
                                        else
                                        {
                                            chipList[chipfileIterator].IOs.Add(new IO
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
                                                Type = IO.TypeIO.Output,
                                                Parent = chipList[chipfileIterator],
                                            });
                                        }
                                        IOName = string.Empty;
                                    }
                                    break;

                                case '→':
                                    inputs = false;
                                    break;

                                default:
                                    break;
                            }
                        }
                    }
                    // Do notthing
                    //if(lineIterator == 1) { }

                    if(lineIterator > 1)
                    {
                        string inputsBinary = "";
                        string binaryToAdd = "";

                        foreach (char c in trimmedLine)
                        {
                            switch (c)
                            {
                                case '→':
                                    inputsBinary = binaryToAdd;
                                    binaryToAdd = "";
                                    break;
                                case '|':  // Ignore
                                    break;
                                default:
                                    binaryToAdd += c;
                                    break;
                            }
                        }

                        chipList[chipfileIterator].binaryStateTable.Add(inputsBinary, binaryToAdd);
                    }

                    lineIterator++;
                }
                chipfileIterator++;
            }

            while (!Raylib.WindowShouldClose())
            {
                Raylib.ClearBackground(Color.Black);

                var mousePosVec2 = Raylib.GetMousePosition();

                if (Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift))
                {
                    showLinkablePositions = true;

                    if(Raylib.IsMouseButtonDown(MouseButton.Left))
                    {
                        if(SelectedOldOutput == null)
                        {
                            // Iterate over all ui items to see if something is close

                            foreach (var or in ors)
                            {
                                foreach (var io in or.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        if (io.Type == IO.TypeIO.Output)
                                        {
                                            SelectedOldOutput = io;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Select first output");
                                        }
                                    }
                                }
                                if (SelectedOldOutput != null)
                                {
                                    break;
                                }
                            }

                            foreach (var and in ands)
                            {
                                foreach (var io in and.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        if (io.Type == IO.TypeIO.Output)
                                        {
                                            SelectedOldOutput = io;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Select first output");
                                        }
                                    }
                                }
                                if (SelectedOldOutput != null)
                                {
                                    break;
                                }
                            }

                            foreach (var i in inverters)
                            {
                                foreach (var io in i.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        if(io.Type == IO.TypeIO.Output)
                                        {
                                            SelectedOldOutput = io;
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Select first output");
                                        }
                                    }
                                }
                                if(SelectedOldOutput != null)
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
                                        // found connection
                                        if(io.Type == IO.TypeIO.Input)
                                        {
                                            Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                            c.State = SelectedOldOutput.State;
                                            Connectors.Add(c);
                                            ConnectorStateQueue.Add(c);
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Can't connect same types of IO");
                                        }
                                        SelectedOldOutput = null;
                                        draggingConnection = false;

                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }
                            foreach (var or in ors)
                            {
                                foreach (var io in or.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        // found connection
                                        if (io.Type == IO.TypeIO.Input)
                                        {
                                            Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                            c.State = SelectedOldOutput.State;
                                            Connectors.Add(c);
                                            ConnectorStateQueue.Add(c);
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Can't connect same types of IO");
                                        }
                                        SelectedOldOutput = null;
                                        draggingConnection = false;

                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }
                            foreach (var and in ands)
                            {
                                foreach (var io in and.IOs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        // found connection
                                        if (io.Type == IO.TypeIO.Input)
                                        {
                                            Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                            c.State = SelectedOldOutput.State;
                                            Connectors.Add(c);
                                            ConnectorStateQueue.Add(c);
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Can't connect same types of IO");
                                        }
                                        SelectedOldOutput = null;
                                        draggingConnection = false;

                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }
                        }

                        SelectedOldOutput = null;
                        draggingConnection = false;
                    }
                }
                else
                {
                    showLinkablePositions = false;

                    if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                    {
                        StopStateUpdates();

                        Or or = new(new Rectangle(mousePosVec2, psuSize));
                        ors.Add(or);

                        StartStateUpdateLoop();
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.Middle))
                    {
                        StopStateUpdates();

                        And and = new(new Rectangle(mousePosVec2, psuSize));
                        ands.Add(and);

                        StartStateUpdateLoop();
                    }

                    if (Raylib.IsMouseButtonPressed(MouseButton.Right))
                    {
                        StopStateUpdates();
                        Inverter i = new Inverter(mousePosVec2);
                        inverters.Add(i);
                        StartStateUpdateLoop();
                    }
                }

                Raylib.BeginDrawing();

                foreach (var psu in psus)
                    Psu.DrawPSU(psu);

                foreach (var i in inverters)
                    Inverter.DrawInverter(i);

                foreach (var or in ors)
                    Or.DrawOr(or);

                foreach (var and in ands)
                    And.DrawAnd(and);

                foreach (var c in Connectors)
                {
                    if(c.State)
                    {
                        Raylib.DrawLineEx(c.NewInput.Position, c.OldOutput.Position, 10f, Color.Blue);
                    }
                    else
                    {
                        Raylib.DrawLineEx(c.NewInput.Position, c.OldOutput.Position, 10f, Color.Gray);
                    }
                }

                if(draggingConnection)
                {
                    Raylib.DrawLineEx(mousePosVec2, SelectedOldOutput.Position, 10f, Color.Yellow);
                }

                Raylib.EndDrawing();
            }

            StopStateUpdates();
        }

        private static bool PointCloseEnough(Vector2 a, Vector2 b)
        {
            if(Raylib.CheckCollisionPointCircle(a,b,20f))
            { 
                return true;
            }

            return false;
        }

        private static void StartStateUpdateLoop()
        {
            _logicCts = new CancellationTokenSource();
            _logicTask = Task.Run(() => StateCalcLoop(_logicCts.Token));
            Console.WriteLine("Logic started");
        }

        private static void StopStateUpdates()
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

        private static async Task StateCalcLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                StateTick();
                await Task.Delay(300);
            }

            Console.WriteLine("Logic stopped");
        }

        private static void StateTick()
        {
            if(ConnectorStateQueue.Count > 0)
            {
                var connector = ConnectorStateQueue.First();
                connector.NewInput.Parent.CheckIfInputShouldChange(connector);
                ConnectorStateQueue.RemoveAt(0);
            }
        }
    }
}