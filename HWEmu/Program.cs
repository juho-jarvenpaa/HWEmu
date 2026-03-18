using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    internal class Program
    {
        public static List<Connector> Connectors = new List<Connector>();
        public static List<Chip> Chips = new List<Chip>();

        public static List<Connector> ConnectorStateQueue = new List<Connector>();

        public static List<Psu> psus = new();
        public static List<Inverter> inverters = new();
        public static List<Or> ors = new();
        public static List<And> ands = new();

        public static CancellationTokenSource? _logicCts;
        public static Task? _logicTask;

        public static bool showLinkablePositions = false;
        public static bool draggingConnection = false;
        public static Output SelectedOldOutput = null;

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
                chipList.Add(new Chip { binaryStateTable = new Dictionary<string, string>(), ChipName = Path.GetFileNameWithoutExtension(chipFile) });

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
                                    if (IOName != "")
                                    {
                                        if (inputs)
                                        {
                                            chipList[chipfileIterator].Inputs.Add(new Input
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
                                                Parent = chipList[chipfileIterator],
                                            });
                                        }
                                        else
                                        {
                                            chipList[chipfileIterator].Outputs.Add(new Output
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
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
                                    IOName += c;
                                    break;
                            }
                        }
                    }
                    // Do notthing
                    //if(lineIterator == 1) { }

                    if (lineIterator > 1)
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

                        // Add default state for the chip // TODO move out of the loop
                        if (lineIterator == 2)
                        {
                            chipList[chipfileIterator].CurrentBinaryInputState = inputsBinary;
                        }
                    }

                    lineIterator++;
                }

                // Check the default output values
                var newOutputString = chipList[chipfileIterator].binaryStateTable[chipList[chipfileIterator].CurrentBinaryInputState];

                string oldOutputString = "";

                foreach (var item in chipList[chipfileIterator].Outputs)
                {
                    oldOutputString += "0";
                }

                //////////////////////////////
                // Update values
                List<int> outputIndexesToChange = new List<int>();

                int outputIndex = 0;
                foreach (char output in oldOutputString)
                {
                    if (newOutputString[outputIndex] == oldOutputString[outputIndex])
                    {
                        // No need to change this index
                    }
                    else
                    {
                        outputIndexesToChange.Add(outputIndex);
                    }
                    outputIndex++;
                }

                List<Output> OutputsToChange = new List<Output>();
                foreach (var indexItem in outputIndexesToChange)
                {
                    OutputsToChange.Add(chipList[chipfileIterator].Outputs[indexItem]);
                }

                foreach (Output output in OutputsToChange)
                {
                    output.State = !output.State;
                }

                ///////////////////////////////////////

                // Add positions for IO's
                var inputCount = chipList[chipfileIterator].Inputs.Count();
                var outputCount = chipList[chipfileIterator].Outputs.Count();

                if(inputCount + outputCount > 4)
                {
                    chipList[chipfileIterator].Rectangle = new Rectangle(0f, 0f, 400f, 800f);
                }
                else
                {
                    chipList[chipfileIterator].Rectangle = new Rectangle(0f, 0f, 400f, 400f);
                }



                float yPositionInputs = 0f;
                float yPositionOutputs = 0f;

                float yPositionIncrementInputs = chipList[chipfileIterator].Rectangle.Height / (inputCount + 1);
                float yPositionIncrementOutputs = chipList[chipfileIterator].Rectangle.Height / (outputCount + 1);

                foreach (var input in chipList[chipfileIterator].Inputs)
                {
                    yPositionInputs += yPositionIncrementInputs;
                    input.Position = new Vector2(chipList[chipfileIterator].Rectangle.X - 100, yPositionInputs);
                }

                foreach (var output in chipList[chipfileIterator].Outputs)
                {
                    yPositionOutputs += yPositionIncrementInputs;
                    output.Position = new Vector2(chipList[chipfileIterator].Rectangle.X + chipList[chipfileIterator].Rectangle.Width + 100, yPositionOutputs);
                }

                chipfileIterator++;
            }

            Chips.Add(chipList[0]);
            Chips[0].Rectangle = new Rectangle(400f, 400f, Chips[0].Rectangle.Width, Chips[0].Rectangle.Height);
            Chip.RecalculateIOPositions(Chips[0]);

            Chips.Add(chipList[1]);
            Chips[1].Rectangle = new Rectangle(1200f, 400f, Chips[1].Rectangle.Width, Chips[1].Rectangle.Height);
            Chip.RecalculateIOPositions(Chips[1]);

            Chips.Add(chipList[2]);
            Chips[2].Rectangle = new Rectangle(400f, 1200f, Chips[2].Rectangle.Width, Chips[2].Rectangle.Height);
            Chip.RecalculateIOPositions(Chips[2]);

            Chips.Add(chipList[3]);
            Chips[3].Rectangle = new Rectangle(1800f, 1200f, Chips[3].Rectangle.Width, Chips[3].Rectangle.Height);
            Chip.RecalculateIOPositions(Chips[3]);

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

                            foreach (var chip in Chips)
                            {
                                foreach (var o in chip.Outputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, o.Position))
                                    {
                                        SelectedOldOutput = o;
                                        break;
                                    }
                                }
                                if (SelectedOldOutput != null)
                                {
                                    break;
                                }
                            }

                            foreach (var or in ors)
                            {
                                foreach (var o in or.Outputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, o.Position))
                                    {
                                        SelectedOldOutput = o;
                                        break;
                                    }
                                }
                                if (SelectedOldOutput != null)
                                {
                                    break;
                                }
                            }

                            foreach (var and in ands)
                            {
                                foreach (var o in and.Outputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, o.Position))
                                    {
                                        SelectedOldOutput = o;
                                        break;
                                    }
                                }
                                if (SelectedOldOutput != null)
                                {
                                    break;
                                }
                            }

                            foreach (var i in inverters)
                            {
                                foreach (var io in i.Outputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        SelectedOldOutput = io;
                                        break;
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

                            foreach (var chip in Chips)
                            {
                                foreach (var io in chip.Inputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        // found connection
                                        Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                        c.State = SelectedOldOutput.State;
                                        Connectors.Add(c);
                                        ConnectorStateQueue.Add(c);
                                        break;
                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }

                            foreach (var i in inverters)
                            {
                                foreach (var io in i.Inputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        // found connection
                                        Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                        c.State = SelectedOldOutput.State;
                                        Connectors.Add(c);
                                        ConnectorStateQueue.Add(c);
                                        break;
                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }
                            foreach (var or in ors)
                            {
                                foreach (var io in or.Inputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        // found connection
                                        Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                        c.State = SelectedOldOutput.State;
                                        Connectors.Add(c);
                                        ConnectorStateQueue.Add(c);
                                        break;
                                    }
                                }
                                if (SelectedOldOutput == null)
                                {
                                    break;
                                }
                            }
                            foreach (var and in ands)
                            {
                                foreach (var io in and.Inputs)
                                {
                                    if (PointCloseEnough(mousePosVec2, io.Position))
                                    {
                                        Connector c = new Connector() { OldOutput = SelectedOldOutput, NewInput = io };
                                        c.State = SelectedOldOutput.State;
                                        Connectors.Add(c);
                                        ConnectorStateQueue.Add(c);
                                        break;
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

                foreach(Chip chip in Chips)
                {
                    Chip.DrawChip(chip);
                }

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