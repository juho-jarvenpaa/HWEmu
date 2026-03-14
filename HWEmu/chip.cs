using Raylib_cs;

namespace HWEmu
{
    public class Chip : Connectable
    {
        public required Dictionary<string, string> binaryStateTable { get; set; }
        public Rectangle Rectangle { get; set; } = new Rectangle();
        public int InputCount = 0;
        public int OutputCount = 0;

        public string CurrentBinaryState { get; set; } = "";

        public static void DrawChip()
        {
            //Raylib.DrawLineEx(and.IOs[0].Position, and.RightCircleCenter, lineThick, Color.DarkGray);
        }

        public override void CheckIfInputShouldChange(Connector connector)
        {
            int IOIndex = 0;

            foreach (var io in IOs)
            {
                if(io.Guid == connector.NewInput.Guid)
                {
                    // Check if input state has changed
                    bool ChangeInputState = true;

                    foreach (Connector c in Program.Connectors)
                    {
                        // Check if one of the connectors provides true
                        if (c.State == true)
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
                        IOs[IOIndex].State = connector.State;

                        // Get oldOutput string
                        var oldOutputString = binaryStateTable[CurrentBinaryState];

                        // Replace input in inputString
                        CurrentBinaryState = CurrentBinaryState.Remove(IOIndex, 1).Insert(IOIndex, connector.State ? "0" : "1");

                        // Get newoutput string
                        var newOutputString = binaryStateTable[CurrentBinaryState];

                        // Update outputs
                        // Form list of ouputs that has changed
                        // 001
                        // 100
                        // 0,2

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

                        List<IO> IOOutputs = new List<IO>();

                        foreach(IO o in IOs)
                        {
                            if(o.Type == IO.TypeIO.Output)
                            {
                                IOOutputs.Add(o);
                            }
                        }

                        List<IO> IOsToChange = new List<IO>();

                        foreach (var index in outputIndexesToChange)
                        {
                            IOsToChange.Add(IOOutputs[index]);
                        }

                        foreach (IO ioOutput in IOsToChange)
                        {
                            // Then update connectors that are related to output
                            foreach (Connector c in Program.Connectors)
                            {
                                if (c.OldOutput.Guid == ioOutput.Guid)
                                {
                                    // Set connector state to be the opposite that it was
                                    c.State = !c.State;
                                    Program.ConnectorStateQueue.Add(c);
                                }
                            }
                        }
                    }
                }
                IOIndex++;
            }
        }
    }
}
