using HWEmu.Gates;
using Raylib_cs;
using System.Numerics;

namespace HWEmu
{
    public class Chip : Connectable
    {
        public required Dictionary<string, string> binaryStateTable { get; set; }
        public Rectangle Rectangle { get; set; } = new Rectangle();
        public int InputCount = 0;
        public int OutputCount = 0;

        public string CurrentBinaryState { get; set; } = "";

        public static void DrawChip(Chip chip)
        {
            Raylib.DrawRectangleRec(chip.Rectangle, Color.White);

            foreach (var input in chip.Inputs)
            {
                if(input.State)
                {
                    Raylib.DrawLineEx(input.Position, chip.Rectangle.Position, 12, Color.DarkBlue);
                }
                else
                {
                    Raylib.DrawLineEx(input.Position, chip.Rectangle.Position, 12, Color.DarkGray);
                }
            }

            foreach (var output in chip.Outputs)
            {
                if (output.State)
                {
                    Raylib.DrawLineEx(output.Position, chip.Rectangle.Position, 12, Color.DarkBlue);
                }
                else
                {
                    Raylib.DrawLineEx(output.Position, chip.Rectangle.Position, 12, Color.DarkGray);
                }
            }
        }

        public override void CheckIfInputShouldChange(Connector connector)
        {
            int inputIndex = 0;

            foreach (var input in Inputs)
            {
                if(input.Guid == connector.NewInput.Guid)
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
                        Inputs[inputIndex].State = connector.State;

                        // Get oldOutput string
                        var oldOutputString = binaryStateTable[CurrentBinaryState];

                        // Replace input in inputString
                        CurrentBinaryState = CurrentBinaryState.Remove(inputIndex, 1).Insert(inputIndex, connector.State ? "0" : "1");

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

                        List<Output> OutputsToChange = new List<Output>();
                        foreach (var indexItem in outputIndexesToChange)
                        {
                            OutputsToChange.Add(Outputs[indexItem]);
                        }

                        foreach (Output output in OutputsToChange)
                        {
                            // Then update connectors that are related to output
                            foreach (Connector c in Program.Connectors)
                            {
                                if (c.OldOutput.Guid == output.Guid)
                                {
                                    // Set connector state to be the opposite that it was
                                    c.State = !c.State;
                                    Program.ConnectorStateQueue.Add(c);
                                }
                            }
                        }
                    }
                }
                inputIndex++;
            }
        }
    }
}
